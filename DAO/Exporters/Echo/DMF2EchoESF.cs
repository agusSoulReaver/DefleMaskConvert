/*
 * This class it's based in the project dmf2esf (https://github.com/BigEvilCorporation/dmf2esf).
 * */

using DefleMaskConvert.DAO.DefleMask;
using DefleMaskConvert.DAO.Importers.DMF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.Exporters.Echo
{
	public class DMF2EchoESF
	{
		private const double RTD = 180 / Math.PI;

		static private int _waitCounter, _nextRow, NextPattern;
		static private bool LoopFound, LoopFlag, SkipPattern, DACEnabled, PSGNoiseFreq, PSGWhiteNoise;
		static private ProcessingChannel[] _channels;
		static private byte TickTimeEvenRow, TickTimeOddRow, TickBase;

		static private int CalculateRowTicks(int row)
		{
			//Calculate number of ticks per row - Deflemask exports 1 tick time for even rows, and another for odd rows
			return (row % 2 != 0) ? (TickTimeOddRow * (TickBase + 1)) : (TickTimeEvenRow * (TickBase + 1));
		}

		static private int Clamp(int value, int min, int max)
		{
			return value <= min ? min : value >= max ? max : value;
		}

		static public EchoESF Convert(DMFData data, List<InstrumentData> activeInstruments)
		{
			_waitCounter = 0;
			DACEnabled = false;
			PSGNoiseFreq = PSGWhiteNoise = true;

			EchoESF esf = new EchoESF();
			TickBase = data.TimeBase;
			TickTimeEvenRow = data.TickTimeEven;
			TickTimeOddRow = data.TickTimeOdd;

			_channels = new ProcessingChannel[MDChannels.Length];
			for (int i = 0; i < MDChannels.Length; i++)
			{
				var channel = MDChannels[i];
				_channels[i] = new ProcessingChannel(channel.Id, channel.Type, channel.Channel, data.Channels[i].Export);
			}

			SetHeader(data, esf);
			ParsePatternPages(data, esf, activeInstruments);
			
			if (data.LoopWholeTrack) GoToLoopEvent(esf.Footer);
			StopPlaybackEvent(esf.Footer);

			return esf;
		}

		static private void ParsePatternPages(DMFData data, EchoESF output, List<InstrumentData> activeInstruments)
		{
			_nextRow = NextPattern = 0;
			LoopFlag = SkipPattern = false;
			int loopStartPattern, loopStartRow, loopEndPattern, loopEndRow;
			LoopFound = FindLoopBeginning(data, out loopStartPattern, out loopStartRow, out loopEndPattern, out loopEndRow);

			//Determine first effect (in case any effects start before the first Note On)
			for (int CurrChannel = 0; CurrChannel < _channels.Length; CurrChannel++)
			{
				ProcessingChannel channel = _channels[(int)ChannelProcessOrder[CurrChannel]];
				byte firstOctave = 0;
				byte firstNote = 0;
				bool mustSetPSGDefaultInstrument = true;

				for (int CurrPattern = 0; CurrPattern < data.PatternPages && firstNote == 0; CurrPattern++)
				{
					for (int CurrRow = 0; CurrRow < data.TotalRowsPerPattern && firstNote == 0; CurrRow++)
					{
						var note = data.Channels[CurrChannel].Pages[CurrPattern].Notes[CurrRow];
						firstNote = (byte)note.Note;
						firstOctave = (byte)note.Octave;

						if (mustSetPSGDefaultInstrument && channel.Type == ChannelType.PSG && !note.IsEmpty && note.Instrument < 0)
						{
							mustSetPSGDefaultInstrument = false;
							note.Instrument = (short)data.Instruments.IndexOf(PSGInstrumentData.DEFAULT_PSG_INSTRUMENT);
						}
					}
				}

				if (firstNote != 0)
				{
					if (firstNote == Constants.LAST_NOTE)
					{
						firstOctave++;
						firstNote = 0;
					}

					channel.EffectNote = firstNote;
					channel.EffectOctave = firstOctave;
				}
			}

			for (int CurrPattern = 0; CurrPattern < data.PatternPages; CurrPattern++)
			{
				EchoPatternPage page = new EchoPatternPage(CurrPattern);
				output.Pages.Add(page);
				int CurrRow = _nextRow;
				_nextRow = 0;

				for ( ; CurrRow < data.TotalRowsPerPattern; CurrRow++)
				{
					EchoPatternRow patternRow = new EchoPatternRow(CurrRow);
					page.Rows.Add(patternRow);

					/* Set loop if we're at the loop start */
					if (loopStartPattern == CurrPattern && loopStartRow == CurrRow)
						SetLoopEvent(patternRow);

					/* Parse pattern data */
					for (int CurrChannel = 0; CurrChannel < _channels.Length; CurrChannel++)
					{
						ParseChannelRow(_channels[(int)ChannelProcessOrder[CurrChannel]], CurrPattern, CurrRow, data, patternRow, activeInstruments);
					}

					int ticksPerRow = CalculateRowTicks(CurrRow);
					//Increment delay counter, will be used and cleared on next command
					_waitCounter += ticksPerRow;

					//Process at least one active effect tick, and continue whilst idle (waitCounter > 0)
					int waitCounterPrev = _waitCounter;
					int numEffectWaits = 0;
					int numEffectsProcessed = 0;
					do
					{
						numEffectsProcessed = 0;

						//Check if any effects need processing
						bool processEffects = false;
						bool wholeDelay = false;
						bool noteOn = false;
						for (int CurrChannel = 0; CurrChannel < _channels.Length; CurrChannel++)
						{
							ProcessingChannel channel = _channels[CurrChannel];
							if (!channel.Export) continue;

							EffectStage stage = GetActiveEffectStage(channel);
							if (stage != EffectStage.Off)
							{
								processEffects = true;

								if (stage == EffectStage.Initialise)
								{
									//First use of effect, delay treated as note on
									wholeDelay = true;
								}

								if (channel.m_effectPortmento.Stage == EffectStage.Initialise && channel.m_effectPortmento.NoteOnthisTick)
								{
									noteOn = true;
								}
							}
						}

						if (processEffects)
						{
							//If note on this row, don't process any delay
							if (!noteOn)
							{
								if (wholeDelay)
								{
									//First tick for at least one of the effects, process whole delay
									numEffectWaits += _waitCounter;
									WaitEvent(patternRow.Events);
								}
								else
								{
									//Subsequent tick, delay by 1
									numEffectWaits++;
									WaitEvent(1, patternRow.Events);
								}
							}

							for (int CurrChannel = 0; CurrChannel < _channels.Length; CurrChannel++)
							{
								numEffectsProcessed += ProcessActiveEffects(_channels[CurrChannel], patternRow);
							}

							if (numEffectsProcessed > 0)
							{
								_waitCounter = Clamp(_waitCounter - 1, 0, _waitCounter);
							}
						}
					} while (numEffectWaits < waitCounterPrev && numEffectsProcessed > 0);

					/* Are we at the loop end? If so, start playing the loop row */
					if (LoopFound && CurrPattern == loopEndPattern && CurrRow == loopEndRow)
					{
						for (int CurrChannel = 0; CurrChannel < _channels.Length; CurrChannel++)
						{
							ParseChannelRow(_channels[CurrChannel], CurrPattern, CurrRow, data, patternRow, activeInstruments);
						}
						GoToLoopEvent(patternRow);
						return;
					}

					/* Do pattern jumps */
					if (SkipPattern)
					{
						SkipPattern = false;
						CurrPattern = NextPattern - 1;
						break;
					}
				}
			}
		}

		static private void ParseChannelRow(ProcessingChannel channel, int CurrPattern, int CurrRow, DMFData data, EchoPatternRow patternRow, List<InstrumentData> activeInstruments)
		{
			if (!channel.Export) return;

			byte ticksPerRow = (byte)CalculateRowTicks(CurrRow);
			NoteData noteData = data.Channels[(int)channel.Id].Pages[CurrPattern].Notes[CurrRow];

			byte effectCounter;
			EffectType effectType;
			byte effectParam;

			byte panning = channel.lastPanning;
			byte FMS = channel.lastFMS;
			byte AMS = channel.lastAMS;

			//Clean up effects
			channel.m_effectNoteCut.NoteCut = EffectMode.Off;
			channel.m_effectNoteDelay.NoteDelay = EffectMode.Off;

			//Get row data
			channel.Note = (byte)noteData.Note;
			channel.Octave = (byte)noteData.Octave;
			channel.NewVolume = (byte)noteData.Volume;
			channel.NewInstrument = (byte)noteData.Instrument;

			byte nextNote = 0;
			byte nextOctave = 0;

			if(CurrRow < data.TotalRowsPerPattern - 1)
			{
				NoteData nextNoteData = data.Channels[(int)channel.Id].Pages[CurrPattern].Notes[CurrRow + 1];
				nextNote = (byte)nextNoteData.Note;
				nextOctave = (byte)nextNoteData.Octave;
			}

			/* Volume updated? */
			if(channel.NewVolume != channel.Volume && channel.NewVolume != 0xff)
			{
				channel.Volume = channel.NewVolume;
				channel.LastVolume = channel.NewVolume;
				if(channel.Type == ChannelType.FM || channel.Type == ChannelType.FM6)
					SetVolumeEvent(channel.ESFId, channel.Volume, patternRow);
				else if(channel.Type == ChannelType.PSG || channel.Type == ChannelType.PSG4)
					SetVolumeEvent(channel.ESFId, channel.Volume, patternRow);
			}

			/* Instrument updated? */
			if (channel.Type != ChannelType.FM6 || !DACEnabled)	// PCM is an Echo instrument, but uses NoteOn to set+play
			{
				if (channel.NewInstrument != channel.Instrument && channel.NewInstrument != 0xff)
				{
					channel.Instrument = channel.NewInstrument;

					byte instrumentIdx = (byte)activeInstruments.IndexOf(data.Instruments[channel.Instrument]);

					SetInstrumentEvent(channel.ESFId, instrumentIdx, patternRow);

					/* Echo resets the volume if the instrument is changed */
					if (channel.Type == ChannelType.FM || channel.Type == ChannelType.FM6)
						SetVolumeEvent(channel.ESFId, channel.LastVolume, patternRow);
					else if (channel.Type == ChannelType.PSG || channel.Type == ChannelType.PSG4)
						SetVolumeEvent(channel.ESFId, channel.LastVolume, patternRow);

					//Set LFO and AMS
					FMInstrumentData fm = data.Instruments[channel.Instrument] as FMInstrumentData;
					FMS = (fm != null) ? fm.LFO : (byte)0;
					AMS = (fm != null) ? fm.LFO2 : (byte)0;
				}
			}

		//#if 0
		//	//Process PSG noise mode envelope
		//	if (instrument && (instrument->m_mode == DMFFile::INSTRUMENT_PSG) && (instrument->m_paramsPSG.envelopeDutyNoise.envelopeSize > 0))
		//	{
		//		if (channel.Note == NOTE_OFF)
		//		{
		//			//End envelope
		//			channel.m_effectPSGNoise.Mode = EffectMode.Off;
		//			PSGNoiseFreq = 0;
		//			PSGPeriodicNoise = 0;
		//		}
		//		else
		//		{
		//			//Begin envelope
		//			channel.m_effectPSGNoise.Mode = EffectMode.NORMAL;
		//			PSGPeriodicNoise = 1;
		//			channel.m_effectPSGNoise.EnvelopeIdx = 0;
		//			channel.m_effectPSGNoise.EnvelopeSize = instrument->m_paramsPSG.envelopeDutyNoise.envelopeSize;
		//			channel.m_effectPSGNoise.EnvelopeData = instrument->m_paramsPSG.envelopeDutyNoise.envelopeData;
		//		}
		//	}
		//#endif

		//#if 0
		//	//Process PSG noise mode "envelope"
		//	if (instrument && (instrument->m_mode == DMFFile::INSTRUMENT_PSG) && (instrument->m_paramsPSG.envelopeDutyNoise.envelopeSize > 0))
		//	{
		//		if (channel.Note == NOTE_OFF)
		//		{
		//			//End noise mode
		//			PSGNoiseFreq = 0;
		//			PSGPeriodicNoise = 0;
		//		}
		//		else
		//		{
		//			//Begin noise mode
		//			int noiseMode = instrument->m_paramsPSG.envelopeDutyNoise.envelopeData[0];
		//			if (noiseMode == 0 || noiseMode == 2)
		//			{
		//				PSGNoiseFreq = 1;
		//				PSGPeriodicNoise = 1;
		//			}
		//			else if (noiseMode == 1 || noiseMode == 3)
		//			{
		//				PSGNoiseFreq = 1;
		//				PSGPeriodicNoise = 0;
		//			}
		//		}
		//	}
		//#endif

			/* Parse some effects before any note ons */
			for(effectCounter=0;effectCounter<noteData.Effects.Length;effectCounter++)
			{
				effectType = (EffectType)noteData.Effects[effectCounter].Type;
				effectParam = (byte)noteData.Effects[effectCounter].Value;

				if(effectType == EffectType.DACOn) // DAC enable
				{
					DACEnabled = effectParam > 0;
				}
				else if(effectType == EffectType.PSGNoise) // Set noise mode
				{
					PSGNoiseFreq = (effectParam & 0xF0) != 0;
					PSGWhiteNoise = (effectParam & 0x0F) != 0;
				}
				else if (effectType == EffectType.Pan) // Set panning
				{
					if (channel.Type == ChannelType.FM || channel.Type == ChannelType.FM6)
					{
						panning = effectParam;
					}
				}
			}

			// Update pan/AMS/FMS register
			if (panning != channel.lastPanning || AMS != channel.lastAMS || FMS != channel.lastFMS)
			{
				SetPan_AMS_FMSEvent(channel.ESFId, panning, AMS, FMS, patternRow);

				channel.lastPanning = panning;
				channel.lastAMS = AMS;
				channel.lastFMS = FMS;
			}

			/* Is this a note off? */
			if(channel.Note == (byte)Notes.Off)
			{
				NoteOffEvent(channel.ESFId, patternRow);

				channel.ToneFreq = 0;
				channel.LastFreq = 0;
				channel.NewFreq = 0;
				channel.lastPanning = 0x11;

				//Turn off effects which stop at note off
				channel.m_effectPortaNote.PortaNote = EffectMode.Off;
				channel.m_effectPortmento.Porta = EffectMode.Off;
				channel.m_effectVibrato.mode = EffectMode.Off;
				channel.m_effectVolSlide.VolSlide = EffectMode.Off;
				channel.m_effectPSGNoise.Mode = EffectMode.Off;
				channel.m_effectPSGNoise.EnvelopeSize = 0;
				channel.FineTune.Mode = EffectMode.Off;
			}

			/* Note on? */
			else if(channel.Note != 0)
			{
				//Notes were 1-based, now 0-based from here
				if (channel.Note == Constants.LAST_NOTE)
				{
					channel.Octave++;
					channel.Note = 0;
				}

				//Save last note/octave for effects in subsequent rows
				channel.m_effectPortaNote.PortaNoteCurrentNote = channel.EffectNote;
				channel.m_effectPortaNote.PortaNoteCurrentOctave = channel.EffectOctave;
				channel.EffectNote = channel.Note;
				channel.EffectOctave = channel.Octave;

				//If PSG3, and PSG4 is in noise mode, take PSG4 octave/note
				if(channel.Id == ChannelId.PSG3 && PSGNoiseFreq)
				{
					ProcessingChannel psg4 = _channels[(int)ChannelId.PSG4];
					byte octave = psg4.EffectOctave;
					byte note = psg4.EffectNote;
					channel.EffectSemitone = PSGFreqs[note][octave];
					channel.EffectOctave = octave;
				}
				else if(channel.Id >= ChannelId.PSG1)
				{
					channel.EffectSemitone = PSGFreqs[channel.EffectNote][channel.EffectOctave];
				}
				else
				{
					channel.EffectSemitone = FMFreqs[channel.EffectNote];
				}

				//Turn off effects which stop at next note
				channel.m_effectPortaNote.PortaNote = EffectMode.Off;
				//channel.m_effectPortmento.Porta = EffectMode.Off;
				//channel.m_effectVibrato.mode = EffectMode.Off;
				//channel.m_effectVolSlide.VolSlide = EffectMode.Off;

				/* Parse some effects that will affect the note on */
				for (effectCounter = 0; effectCounter < noteData.Effects.Length; effectCounter++)
				{
					effectType = (EffectType)noteData.Effects[effectCounter].Type;
					effectParam = (byte)noteData.Effects[effectCounter].Value;

					switch(effectType)
					{
						case EffectType.PortmentoToNote:
							channel.m_effectPortaNote.PortaNote = EffectMode.Schedule;
							break;

						case EffectType.NoteDelay:
							channel.m_effectNoteDelay.NoteDelay = EffectMode.Schedule;
							channel.m_effectNoteDelay.NoteDelayOffset = effectParam;
							break;

						case EffectType.SetLFO:
							bool LFOEnabled = (effectParam & 0xF0) != 0;
							SetLFOEvent(LFOEnabled, (byte)(effectParam & 0x07), patternRow);
							break;

						case EffectType.FineTune:
							channel.FineTune.Mode = EffectMode.Off;
							float offset = effectParam - 0x80;
							if (offset != 0)
							{
								offset = offset < 0 ? offset / 128f : offset / 127f;
								channel.FineTune.Mode = offset > 0 ? EffectMode.Up : EffectMode.Down;
							}
							channel.FineTune.NoteOffset = offset;
							break;
					}
				}

				/* If note delay or tone portamento is off, send the note on command already! */
				if(channel.m_effectNoteDelay.NoteDelay == EffectMode.Off && channel.m_effectPortaNote.PortaNote == EffectMode.Off)
					NoteOn(channel, patternRow, data, activeInstruments);
			}
			/* Note column is empty */
			else
			{
			}

			if(channel.m_effectVolSlide.VolSlide != EffectMode.Off)
			{
				int i = 0;

				for(i = 0; i < ticksPerRow && channel.m_effectVolSlide.VolSlide != EffectMode.Off; i++)
				{
					//Calc current volume
					channel.m_effectVolSlide.CurrVol = (ushort)Clamp(channel.m_effectVolSlide.CurrVol + channel.m_effectVolSlide.VolSlideValue, 0, 0x7f);

					//Set volume (includes current delay)
					SetVolumeEvent(channel.ESFId, (byte)channel.m_effectVolSlide.CurrVol, patternRow);

					//Delay 1 tick
					_waitCounter += 1;

					//Set channel volume history
					channel.LastVolume = (byte)channel.m_effectVolSlide.CurrVol;

					//If hit the volume limits, finished
					if(channel.m_effectVolSlide.CurrVol == 0 || channel.m_effectVolSlide.CurrVol == 0x7f)
					{
						channel.m_effectVolSlide.VolSlide = EffectMode.Off;
					}
				}

				//Decrease existing tick count
				_waitCounter -= i;
				if (_waitCounter < 0)
					_waitCounter = 0;
			}

			//Process new effects
			for(effectCounter=0;effectCounter<noteData.Effects.Length;effectCounter++)
			{
				effectType = (EffectType)noteData.Effects[effectCounter].Type;
				effectParam = (byte)noteData.Effects[effectCounter].Value;
				//fprintf(stdout, "%02x %02x, ",(int)EffectType,(int)EffectParam);

				switch(effectType)
				{
						/* Normal effects */
					case EffectType.Arpeggio: // Arpeggio
						if(effectParam != 0)
						{
							channel.m_effectArpeggio.Arp = EffectMode.Normal;
							byte ArpOct;
							byte ArpNote;

							/* do first freq */
							ArpOct = channel.Octave;
							ArpNote = (byte)(channel.Note + (effectParam & 0xf0 >> 4));
							while (ArpNote > Constants.LAST_NOTE)
							{
								ArpOct++;
								ArpNote -= (byte)Constants.LAST_NOTE;
							}

							/* PSG */
							if(channel.Type == ChannelType.PSG)
								channel.m_effectArpeggio.Arp1 = PSGFreqs[ArpNote][ArpOct - 2];
							else
								channel.m_effectArpeggio.Arp1 = (ushort)((channel.Octave << 11) | FMFreqs[channel.Note]);

							channel.m_effectArpeggio.Arp2 = 0;

							/* do second freq */
							if((effectParam & 0x0f) > 0)
							{
								ArpOct = channel.Octave;
								ArpNote = (byte)(channel.Note + (effectParam & 0xf0 >> 4));
								while (ArpNote > Constants.LAST_NOTE)
								{
									ArpOct++;
									ArpNote -= (byte)Constants.LAST_NOTE;
								}

								/* PSG */
								if(channel.Type == ChannelType.PSG)
									channel.m_effectArpeggio.Arp2 = PSGFreqs[ArpNote][ArpOct - 2];
								else
									channel.m_effectArpeggio.Arp2 = (ushort)((channel.Octave << 11) | FMFreqs[channel.Note]);
							}

						}
						else
						{
							channel.m_effectArpeggio.Arp = EffectMode.Off;
						}
						break;

					case EffectType.Vibrato:
					{
						byte speed = (byte)(effectParam >> 4);
						byte amplitude = (byte)(effectParam & 0xF);

						speed = (byte)Clamp(speed, (byte)1, (byte)15);
						amplitude = (byte)Clamp(amplitude, (byte)0, (byte)12);

						if(speed == 0 || amplitude == 0)
						{
							if(channel.m_effectVibrato.stage == EffectStage.Continue)
							{
								channel.m_effectVibrato.stage = EffectStage.End;
							}
						}
						else
						{
							if(channel.m_effectVibrato.mode == EffectMode.Off)
							{
								channel.m_effectVibrato.sineTime = 0;
							}

							if(channel.m_effectVibrato.stage != EffectStage.Continue)
							{
								channel.m_effectVibrato.stage = EffectStage.Initialise;
								channel.m_effectVibrato.mode = EffectMode.Normal;
							}

							channel.m_effectVibrato.sineSpeed = speed;
							channel.m_effectVibrato.sineAmplitude = amplitude;
						}
						break;
					}
					case EffectType.PortmentoUp: // Portamento up
					case EffectType.PortmentoDown: // Portamento down
						//TODO: Fix for PSG
						//if(chan < CHANNEL_PSG1)
						{
							if(effectParam == 0)
							{
								channel.m_effectPortmento.Porta = EffectMode.Off;
								channel.m_effectPortmento.Stage = EffectStage.Off;
							}
							else
							{
								channel.m_effectPortmento.NoteOnthisTick = (channel.Note != 0 || channel.Octave != 0);

								//If note on this tick or effect was off, start from last note/octave
								if(channel.m_effectPortmento.NoteOnthisTick || channel.m_effectPortmento.Porta == EffectMode.Off)
								{
									channel.m_effectPortmento.Stage = EffectStage.Initialise;
								}

								channel.m_effectPortmento.Porta = effectType == EffectType.PortmentoUp ? EffectMode.Up : EffectMode.Down;
								channel.m_effectPortmento.PortaSpeed = (byte)Clamp(effectParam, (byte)0, (byte)0x7F);

								//Cancel vibrato
								//channel.m_effectVibrato.mode = EffectMode.OFF;
								//channel.m_effectVibrato.stage = EffectStage.OFF;
							}
						}
						break;
					case EffectType.PortmentoToNote: // Tone portamento
						channel.m_effectPortaNote.PortaNoteSpeed = effectParam;
						//channel.m_effectPortaNote.PortaNoteCurrentNote = channel.Note;
						//channel.m_effectPortaNote.PortaNoteCurrentOctave = channel.Octave;
						channel.m_effectPortaNote.PortaNoteTargetNote = channel.Note;
						channel.m_effectPortaNote.PortaNoteTargetOctave = channel.Octave;

						//If PSG3, and PSG4 is in noise mode, take PSG4 octave/note
						if (channel.Id == ChannelId.PSG3 && PSGNoiseFreq)
						{
							ProcessingChannel psg4 = _channels[(int)ChannelId.PSG4];
							byte octave = psg4.m_effectPortaNote.PortaNoteCurrentOctave;
							byte note = psg4.m_effectPortaNote.PortaNoteCurrentNote;
							channel.EffectSemitone = PSGFreqs[note][octave];
							channel.EffectOctave = octave;
						}
						else if (channel.Id >= ChannelId.PSG1)
						{
							channel.EffectSemitone = PSGFreqs[channel.m_effectPortaNote.PortaNoteCurrentNote][channel.m_effectPortaNote.PortaNoteCurrentOctave];
						}
						else
						{
							channel.EffectSemitone = FMFreqs[channel.m_effectPortaNote.PortaNoteCurrentNote];
						}

						bool isOctaveUp = (sbyte)channel.m_effectPortaNote.PortaNoteTargetOctave - (sbyte)channel.m_effectPortaNote.PortaNoteCurrentOctave > 0;
						bool isSameOctave = (sbyte)channel.m_effectPortaNote.PortaNoteTargetOctave - (sbyte)channel.m_effectPortaNote.PortaNoteCurrentOctave == 0;
						bool isNoteUp = (sbyte)channel.m_effectPortaNote.PortaNoteTargetNote - (sbyte)channel.m_effectPortaNote.PortaNoteCurrentNote >= 0;
						channel.m_effectPortaNote.PortaNote = isOctaveUp || (isSameOctave && isNoteUp) ? EffectMode.Up : EffectMode.Down;
						channel.m_effectPortaNote.Stage = EffectStage.Initialise;
						break;

					case EffectType.SetSpeed1: // Set speed 1
						TickTimeEvenRow = effectParam;
						break;
					case EffectType.SetSpeed2: // Set speed 2
						TickTimeOddRow = effectParam;
						break;

					case EffectType.VolumeSlide: // Volume slide
						int upSlide = (effectParam & 0xF0) >> 4;
						int downSlide = -(effectParam & 0x0F);
						channel.m_effectVolSlide.VolSlideValue = (sbyte)(upSlide + downSlide);
						channel.m_effectVolSlide.VolSlide = (channel.m_effectVolSlide.VolSlideValue > 0) ? EffectMode.Up : EffectMode.Down;
						channel.m_effectVolSlide.CurrVol = channel.LastVolume;
						break;
				
					case EffectType.Break: // Pattern break
						SkipPattern = true;
						if(CurrPattern != data.PatternPages)
							NextPattern = CurrPattern+1;
						else // should actually be the same as loop
							NextPattern = 0;

						_nextRow = effectParam;
						break;
						/* Unknown/unsupported effects */
					default:
						break;
				}
			}
		}

		static private void NoteOn(ProcessingChannel channel, EchoPatternRow patternRow, DMFData data, List<InstrumentData> activeInstruments)
		{
			/* Is this the PSG noise channel? */
			if (channel.Type == ChannelType.PSG4)
			{
				byte NoiseMode = 0;

				//if (PSGPeriodicNoise)
				//	NoiseMode = 3;		//Perodic noise at PSG3's frequency
				//else if (PSGNoiseFreq)
				//	NoiseMode = 7;		//White noise at PSG3's frequency

				if (PSGWhiteNoise)
					NoiseMode = 4;

				/* Are we using the PSG3 frequency? */
				if(PSGNoiseFreq)
				{
					// if C-8 change it to B-7
					if(channel.Note == 0 && channel.Octave == 8)
					{
						channel.Octave--;
						channel.Note = 11;
					}
					//channel.Octave--;

					ProcessingChannel psg3 = _channels[(int)ChannelId.PSG3];
					/* Get the frequency value */
					psg3.ToneFreq = PSGFreqs[channel.Note][channel.Octave];

					/* Only update frequency if it's not the same as the last */
					if (psg3.LastFreq != psg3.ToneFreq)
						SetFrequencyEvent(psg3.ESFId, psg3.ToneFreq, patternRow);

					psg3.LastFreq = psg3.ToneFreq;

					NoiseMode += 3;
					if (channel.FineTune.Mode != EffectMode.Off)
						Console.WriteLine("PSNoise Detune Not Implemented!");
				}
				else
				{
					switch(channel.Note)
					{
						default:
						case 3: // D
							NoiseMode += 2;
							break;
						case 2: // C#
							NoiseMode++;
							break;
						case 1: // C
							// do nothing
							break;
					}
				}

				NoteOnEvent(channel.ESFId, NoiseMode, 0, patternRow);
			}

			/* Skip if this is the PSG3 channel and its frequency value is already used for the noise channel */
			else if(!(channel.Id == ChannelId.PSG3 && PSGNoiseFreq))
			{
				/* Calculate frequency */
				channel.ToneFreq = 0;    // Reset if this is for a channel where this doesn't make much sense.

				/* Is this an FM channel? */
				if(channel.Type == ChannelType.FM || (channel.Type == ChannelType.FM6 && DACEnabled == false))
				{
					channel.ToneFreq = (ushort)(channel.Octave << 11 | FMFreqs[channel.Note]);
					if (channel.FineTune.Mode != EffectMode.Off)
						Console.WriteLine("FM Detune Not Implemented!");
				}
				/* PSG */
				else if(channel.Type == ChannelType.PSG)
				{
					channel.ToneFreq = PSGFreqs[channel.Note][channel.Octave];

					if (channel.FineTune.Mode != EffectMode.Off)
					{
						int detuneNote = channel.Note;
						int detuneOctave = channel.Octave;
						switch (channel.FineTune.Mode)
						{
							case EffectMode.Up:
								if (++detuneNote >= Constants.LAST_NOTE)
								{
									detuneNote = 0;
									detuneOctave = Math.Min(detuneOctave + 1, MaxOctave);
								}
								break;
							case EffectMode.Down:
								if (--detuneNote < 0)
								{
									detuneNote = Constants.LAST_NOTE - 1;
									detuneOctave = Math.Max(detuneOctave - 1, 0);
								}
								break;
						}
						float anchor = channel.ToneFreq;
						float detuneFreq = PSGFreqs[detuneNote][detuneOctave];
						channel.FineTune.NoteFrequency = channel.ToneFreq;
						channel.FineTune.NoteFrequency += (ushort)((detuneFreq - anchor) * channel.FineTune.NoteOffset);
					}
				}

				/* Reset last tone / new tone freqs */
				channel.NoteFreq = channel.ToneFreq;
				channel.LastFreq = 0;
				channel.NewFreq = 0;

				if((channel.Type == ChannelType.FM6 && DACEnabled == true))
				{
					byte sampleInstrumentIdx = (byte)activeInstruments.IndexOf(data.Samples[channel.Note]);

					//Set PCM rate
					//SetPCMRateEvent(PCM_FREQ_DEFAULT, patternRow);

					//PCM note on
					NoteOnEvent(ESFChannel.DAC, sampleInstrumentIdx, 0, patternRow);
				}
				else
				{
					NoteOnEvent(channel.ESFId, channel.Note, channel.Octave, patternRow);
					if (channel.FineTune.Mode != EffectMode.Off)
						SetFrequency(channel.Id, channel.FineTune.NoteFrequency, patternRow, false);
				}
			}
			return;
		}

		static private int ProcessActiveEffects(ProcessingChannel channel, EchoPatternRow patternRow)
		{
			if (!channel.Export) return 0;

			int numEffectsProcess = 0;
			//Process active effects
			if(channel.m_effectPortmento.Porta != EffectMode.Off)
			{
				if(channel.m_effectPortmento.NoteOnthisTick)
				{
					//Had note on this row, skip
					channel.m_effectPortmento.NoteOnthisTick = false;
				}
				else
				{
					//Sign extend
					ushort speed = (ushort)channel.m_effectPortmento.PortaSpeed;

					//Calc delta
					short delta = (short)((channel.m_effectPortmento.Porta == EffectMode.Up) ? speed : -speed);

					byte prevOctave = channel.EffectOctave;
					uint prevSemitone = (uint)channel.EffectSemitone;

					if (channel.Id < ChannelId.PSG1)
					{
						SlideFM(ref channel.EffectOctave, ref channel.EffectSemitone, delta);
					}
					else
					{
						SlidePSG(ref channel.EffectOctave, ref channel.EffectSemitone, delta);
					}

					//Set frequency
					if(prevOctave != channel.EffectOctave || prevSemitone != channel.EffectSemitone)
					{
						channel.Octave = channel.EffectOctave;
						var chan = channel.Id;

						//If PSG4 in noise mode, set on PSG3 instead
						if (chan == ChannelId.PSG4 && PSGNoiseFreq)
						{
							chan = ChannelId.PSG3;
						}

						SetFrequency(chan, (ushort)channel.EffectSemitone, patternRow, false);
					}
				}

				//Continue until next note off
				numEffectsProcess++;
				channel.m_effectPortmento.Stage = EffectStage.Continue;
			}

			//if (channel.m_effectPortaNote.PortaNote != EffectMode.Off)
			//{
			//	//TODO: handle octave change
			//	byte currentNote = channel.m_effectPortaNote.PortaNoteCurrentNote;
			//	byte currentOctave = channel.m_effectPortaNote.PortaNoteCurrentOctave;
			//	byte targetNote = channel.m_effectPortaNote.PortaNoteTargetNote;
			//	byte targetOctave = channel.m_effectPortaNote.PortaNoteTargetOctave;
			//	byte speed = channel.m_effectPortaNote.PortaNoteSpeed;

			//	//Lerp towards target at speed
			//	if ((targetNote | targetOctave << 8) > (currentNote | currentOctave << 8))
			//	{
			//		currentNote += speed;
			//		if (currentNote > targetNote)
			//			currentNote = targetNote;
			//	}
			//	else if ((targetNote | targetOctave << 8) < (currentNote | currentOctave << 8))
			//	{
			//		currentNote -= speed;
			//		if (currentNote < targetNote)
			//			currentNote = targetNote;
			//	}

			//	//If target reached, effect finished
			//	if (currentNote == targetNote)
			//	{
			//		channel.m_effectPortaNote.PortaNote = EffectMode.Off;
			//	}

			//	//Note on
			//	channel.Note = currentNote;
			//	NoteOn(channel, patternRow, data, activeInstruments);

			//	//Update effect history data
			//	channel.m_effectPortaNote.PortaNoteCurrentNote = currentNote;
			//	channel.m_effectPortaNote.PortaNoteCurrentOctave = currentOctave;
			//}
			if (channel.m_effectPortaNote.PortaNote != EffectMode.Off)
			{
				byte currentNote = channel.m_effectPortaNote.PortaNoteCurrentNote;
				byte targetNote = channel.m_effectPortaNote.PortaNoteTargetNote;
				byte targetOctave = channel.m_effectPortaNote.PortaNoteTargetOctave;
				//Sign extend
				ushort speed = (ushort)channel.m_effectPortaNote.PortaNoteSpeed;

				bool isGoingUp = channel.m_effectPortaNote.PortaNote == EffectMode.Up;
				//Calc delta
				short delta = (short)(isGoingUp? speed : -speed);

				byte prevOctave = channel.m_effectPortaNote.PortaNoteCurrentOctave;
				uint prevSemitone = (uint)channel.EffectSemitone;
				bool completed = false;

				if (channel.Id < ChannelId.PSG1)
				{
					SlideFM(ref channel.m_effectPortaNote.PortaNoteCurrentOctave, ref channel.EffectSemitone, delta);

					if ((isGoingUp && ((channel.m_effectPortaNote.PortaNoteCurrentOctave == targetOctave && channel.EffectSemitone >= FMFreqs[targetNote]) || channel.m_effectPortaNote.PortaNoteCurrentOctave > targetOctave)) ||
						(!isGoingUp && ((channel.m_effectPortaNote.PortaNoteCurrentOctave == targetOctave && channel.EffectSemitone <= FMFreqs[targetNote]) || channel.m_effectPortaNote.PortaNoteCurrentOctave < targetOctave)))
					{
						channel.m_effectPortaNote.PortaNoteCurrentOctave = targetOctave;
						channel.EffectSemitone = FMFreqs[targetNote];
						completed = true;
					}
				}
				else
				{
					SlidePSG(ref channel.m_effectPortaNote.PortaNoteCurrentOctave, ref channel.EffectSemitone, delta);

					ushort frequency = PSGFreqs[targetNote][targetOctave];
					if ((isGoingUp && channel.EffectSemitone <= frequency) ||
						(!isGoingUp && channel.EffectSemitone >= frequency))
					{
						channel.m_effectPortaNote.PortaNoteCurrentOctave = targetOctave;
						channel.EffectSemitone = frequency;
						completed = true;
					}
				}

				//Set frequency
				if (completed || prevOctave != channel.m_effectPortaNote.PortaNoteCurrentOctave || prevSemitone != channel.EffectSemitone)
				{
					channel.Octave = channel.m_effectPortaNote.PortaNoteCurrentOctave;
					var chan = channel.Id;

					//If PSG4 in noise mode, set on PSG3 instead
					if (chan == ChannelId.PSG4 && PSGNoiseFreq)
					{
						chan = ChannelId.PSG3;
					}

					SetFrequency(chan, (ushort)channel.EffectSemitone, patternRow, false);
				}

				//Continue until next note off
				numEffectsProcess++;
				if (completed) channel.m_effectPortaNote.PortaNote = EffectMode.Off;
				channel.m_effectPortaNote.Stage = completed ? EffectStage.Off : EffectStage.Continue;
			}

		//#if 0
		//	if (channel.m_effectPSGNoise.Mode == EffectMode.Normal)
		//	{
		//		//Get noise frequency
		//		ushort noiseFreq = (ushort)(channel.m_effectPSGNoise.EnvelopeData[channel.m_effectPSGNoise.EnvelopeIdx % channel.m_effectPSGNoise.EnvelopeSize]);

		//		//Set frequency
		//		SetFrequency(channel.ESFId, noiseFreq, patternRow, false);

		//		//Tick effect
		//		channel.m_effectPSGNoise.EnvelopeIdx++;
		//	}
		//#endif

			if(channel.m_effectVibrato.mode != EffectMode.Off)
			{
				if(channel.m_effectVibrato.stage == EffectStage.End)
				{
					channel.Octave = channel.EffectOctave;
					SetFrequency(channel.Id, (ushort)channel.EffectSemitone, patternRow, false);
					channel.m_effectVibrato.mode = EffectMode.Off;
					channel.m_effectVibrato.stage = EffectStage.Off;
					channel.m_effectVibrato.sineTime = 0;
				}
				else
				{
					float sine = (float)(Math.Sin((float)channel.m_effectVibrato.sineTime / 10.0f));

					if (channel.Id < ChannelId.PSG1)
					{
						sine *= 2f;
					}

					short pitchOffset = (short)(sine * (float)channel.m_effectVibrato.sineAmplitude);

					channel.m_effectVibrato.sineTime += channel.m_effectVibrato.sineSpeed;

					byte prevOctave = channel.EffectOctave;
					uint prevSemitone = (uint)channel.EffectSemitone;
					byte newOctave = channel.EffectOctave;
					int newSemitone = channel.EffectSemitone;

					if(channel.Id < ChannelId.PSG1)
					{
						SlideFM(ref newOctave, ref newSemitone, pitchOffset);
					}
					else
					{
						SlidePSG(ref newOctave, ref newSemitone, pitchOffset);
					}

					if(newOctave != prevOctave || newSemitone != prevSemitone)
					{
						//Set frequency
						channel.Octave = newOctave;
						var chan = channel.Id;

						//If PSG4 in noise mode, set on PSG3 instead
						if (chan == ChannelId.PSG4 && PSGNoiseFreq)
						{
							chan = ChannelId.PSG3;
						}

						SetFrequency(chan, (ushort)newSemitone, patternRow, false);
					}

					channel.m_effectVibrato.stage = EffectStage.Continue;
				}
			
				numEffectsProcess++;
			}

			return numEffectsProcess;
		}

		static private void SetFrequency(ChannelId chan, ushort FMSemitone, EchoPatternRow patternRow, bool processDelay)
		{
			ProcessingChannel channel = _channels[(int)chan];
			/* Is this the PSG noise channel? */
			if (channel.Type == ChannelType.PSG4)
			{
				byte NoiseMode = 0;

				/* Is periodic noise active? */
				if (PSGWhiteNoise)
					NoiseMode = 4;

				/* Are we using the PSG3 frequency? */
				if (PSGNoiseFreq)
				{
					// if C-8 change it to B-7
					if (channel.Note == 0 && channel.Octave == 8)
					{
						channel.Octave--;
						channel.Note = 11;
					}
					//channel.Octave--;

					ProcessingChannel psg3 = _channels[(int)chan-1];
					/* Get the frequency value */
					psg3.ToneFreq = PSGFreqs[channel.Note][channel.Octave];

					/* Only update frequency if it's not the same as the last */
					if (psg3.LastFreq != psg3.ToneFreq)
						SetFrequencyEvent(psg3.ESFId, psg3.ToneFreq, patternRow, processDelay);

					psg3.LastFreq = psg3.ToneFreq;

					NoiseMode += 3;

				}
			}

			/* Skip if this is the PSG3 channel and its frequency value is already used for the noise channel */
			else //if(!(channel.Id == CHANNEL_PSG3 && PSGNoiseFreq))
			{
				/* Calculate frequency */
				channel.ToneFreq = 0;    // Reset if this is for a channel where this doesn't make much sense.

				/* Is this an FM channel? */
				if (channel.Type == ChannelType.FM || (channel.Type == ChannelType.FM6 && !DACEnabled))
				{
					channel.ToneFreq = (ushort)((ushort)(channel.Octave << 11) | FMSemitone); // FMFreqs[channel.Note];
				}
				/* PSG */
				else if (channel.Type == ChannelType.PSG)
				{
					channel.ToneFreq = (ushort)FMSemitone;
				}

				if (!(channel.Type == ChannelType.FM6 && DACEnabled))
				{
					if (channel.LastFreq != channel.ToneFreq)
					{
						SetFrequencyEvent(channel.ESFId, channel.ToneFreq, patternRow, processDelay);
					}
				}

				/* Reset last tone / new tone freqs */
				channel.NoteFreq = channel.ToneFreq;
				channel.LastFreq = 0;
				channel.NewFreq = 0;
			}
		}

		static private void SlideFM(ref byte octave, ref int semitone, short delta)
		{
			//Increment/decrement frequency
			semitone += delta;

			if (delta > 0)
			{
				//Clamp to max octave+freq
				if (octave == MaxOctave && semitone > FMFreqs[MaxFMFreqs - 1])
				{
					semitone = FMFreqs[MaxFMFreqs - 1];
				}
			}
			else if (delta < 0)
			{
				//Clamp to min octave+freq
				if (octave == 0 && semitone < FMFreqs[0])
				{
					semitone = FMFreqs[0];
				}
			}

			//Wrap around octave
			if (semitone < FMSlideFreqs[0])
			{
				ushort diff = (ushort)(FMSlideFreqs[0] - semitone);
				semitone = (ushort)(FMSlideFreqs[MaxFMSlideFreqs - 1] - diff);
				octave--;
			}

			if (semitone > FMSlideFreqs[MaxFMSlideFreqs - 1])
			{
				ushort diff = (ushort)((short)semitone - FMSlideFreqs[MaxFMSlideFreqs - 1]);
				semitone = (ushort)(FMSlideFreqs[0] + diff);
				octave++;
			}
		}

		static private void SlidePSG(ref byte octave, ref int semitone, short delta)
		{
			//Increment/decrement frequency (reverse for PSG)
			semitone -= delta;

			if(semitone < PSGFreqs[MaxPSGFreqs - 1][MaxOctave])
				semitone = PSGFreqs[MaxPSGFreqs - 1][MaxOctave];

			if(semitone > PSGFreqs[0][0])
				semitone = PSGFreqs[0][0];

		//#if 0
		//	if (delta > 0)
		//	{
		//		//Clamp to max octave+freq
		//		if (octave == MaxOctave && semitone < PSGFreqs[MaxPSGFreqs - 1][MaxOctave - 1])
		//		{
		//			semitone = PSGFreqs[MaxPSGFreqs - 1][MaxOctave - 1];
		//		}

		//		//Wrap around octave
		//		if(semitone > PSGFreqs[MaxPSGFreqs - 1][octave])
		//		{
		//			octave++;
		//			semitone = PSGFreqs[0][octave];
		//		}
		//	}
		//	else if (delta < 0)
		//	{
		//		//Clamp to min octave+freq
		//		if (octave == 0 && semitone > PSGFreqs[0][0])
		//		{
		//			semitone = PSGFreqs[0][0];
		//		}

		//		//Wrap around octave
		//		if (semitone < PSGFreqs[0][octave])
		//		{
		//			octave--;
		//			semitone = PSGFreqs[MaxPSGFreqs - 1][octave];
		//		}
		//	}
		//#endif
		}

		static private EffectStage GetActiveEffectStage(ProcessingChannel channel)
		{
			//TODO: Other effects
			if (channel.m_effectPortmento.Porta != EffectMode.Off)
			{
				return channel.m_effectPortmento.Stage;
			}

			if (channel.m_effectVibrato.mode != EffectMode.Off)
			{
				return channel.m_effectVibrato.stage;
			}

			if (channel.m_effectPortaNote.PortaNote != EffectMode.Off)
			{
				return channel.m_effectPortaNote.Stage;
			}

			if (channel.m_effectPSGNoise.Mode != EffectMode.Off)
			{
				return EffectStage.Continue;
			}

			return EffectStage.Off;
		}

		#region Commands
		static private void SetPCMRateEvent(ESF_PCMRate rate, EchoPatternRow patternRow)
		{
			SetPCMRateEvent(rate, patternRow.Events);
		}

		static private void SetPCMRateEvent(ESF_PCMRate rate, List<IEchoEvent> events)
		{
			byte rateHigh = (byte)(((byte)rate ^ 0x3FF) >> 2);
			byte rateLow = (byte)(((byte)rate ^ 0x3FF) & 3);

			events.Add(new SetBankRegisterEvent(SetBankRegisterEvent.Banks.Bank0, FMRegister.TimerA_MSBs, rateHigh, string.Format("Set PCM Rate= {0}. Set param MSBs", rate.ToString())));
			events.Add(new SetBankRegisterEvent(SetBankRegisterEvent.Banks.Bank0, FMRegister.TimerA_LSBs, rateLow, "Set PCM Rate. Set param LSBs"));
		}

		static private void SetFrequencyEvent(ESFChannel channel, ushort freq, EchoPatternRow patternRow, bool processDelay=true)
		{
			if(processDelay) WaitEvent(patternRow.Events);
			patternRow.Events.Add(new SetFrequencyEvent(channel, freq));
		}

		static private void NoteOnEvent(ESFChannel channel, byte note, byte octave, EchoPatternRow patternRow)
		{
			WaitEvent(patternRow.Events);
			patternRow.Events.Add(new NoteOnEvent(channel, ESF_CHANNEL_TYPES[(int)channel], note, octave));
		}

		static private void NoteOffEvent(ESFChannel channel, EchoPatternRow patternRow)
		{
			WaitEvent(patternRow.Events);
			patternRow.Events.Add(new NoteOffEvent(channel));
		}

		static private void SetVolumeEvent(ESFChannel channel, byte volume, EchoPatternRow patternRow)
		{
			WaitEvent(patternRow.Events);

			if (ESF_CHANNEL_TYPES[(int)channel] == ChannelType.PSG || ESF_CHANNEL_TYPES[(int)channel] == ChannelType.PSG4)
				volume = (byte)(~volume & 0x0F);
			else
				volume = (byte)(~volume & 0x7F);

			patternRow.Events.Add(new SetVolumeEvent(channel, volume));
		}

		static private void SetInstrumentEvent(ESFChannel channel, byte index, EchoPatternRow patternRow)
		{
			WaitEvent(patternRow.Events);
			patternRow.Events.Add(new SetInstrumentEvent(channel, index));
		}

		static private void SetLoopEvent(EchoPatternRow patternRow)
		{
			SetLoopEvent(patternRow.Events);
		}

		static private void SetLoopEvent(List<IEchoEvent> events)
		{
			WaitEvent(events);
			events.Add(new PlaybackEvent(PlaybackEvent.Actions.SetLoop));
		}

		static private void GoToLoopEvent(EchoPatternRow patternRow)
		{
			GoToLoopEvent(patternRow.Events);
		}

		static private void GoToLoopEvent(List<IEchoEvent> events)
		{
			WaitEvent(events);
			events.Add(new PlaybackEvent(PlaybackEvent.Actions.GoToLoop));
		}

		static private void StopPlaybackEvent(List<IEchoEvent> events)
		{
			WaitEvent(events);
			events.Add(new PlaybackEvent(PlaybackEvent.Actions.Stop));
		}

		static private void SetPan_AMS_FMSEvent(ESFChannel channel, byte pan, byte AMS, byte FMS, EchoPatternRow patternRow)
		{
			WaitEvent(patternRow.Events);
			patternRow.Events.Add(new SetFMParametersEvent(channel, pan, AMS, FMS));
		}

		static private void WaitEvent(List<IEchoEvent> output)
		{
			WaitEvent(_waitCounter, output);
			_waitCounter = 0;
		}

		static private void WaitEvent(int ticks, List<IEchoEvent> output)
		{
			while(ticks > byte.MaxValue)
			{
				output.Add(new DelayEvent(0, false));
				ticks -= byte.MaxValue + 1;
			}

			if (ticks > 0)
			{
				if(ticks <= 0x10) output.Add(new DelayEvent((byte)ticks, true));
				else output.Add(new DelayEvent((byte)ticks, false));
			}
		}

		static private void SetLFOEvent(bool enable, byte frequencyIndex, EchoPatternRow patternRow)
		{
			WaitEvent(patternRow.Events);

			byte data = (byte)(enable ? 0x08 | frequencyIndex : 0);
			patternRow.Events.Add(new SetBankRegisterEvent(SetBankRegisterEvent.Banks.Bank0, FMRegister.LFO, data, enable ? string.Format("Turn On LFO. Frequency index: {0}", frequencyIndex) : "Turn Off LFO"));
		}
		#endregion

		static private void SetHeader(DMFData data, EchoESF output)
		{
			if (data.PCMRate != ESF_PCMRate.NotChange) SetPCMRateEvent(data.PCMRate, output.Header);
			if (data.LockChannels) LockChannels(data, output);
			if (data.LoopWholeTrack) SetLoopEvent(output.Header);
		}

		static private void LockChannels(DMFData data, EchoESF output)
		{
			bool psgNoiseUsed = false;
			HashSet<ESFChannel> usedChannels = new HashSet<ESFChannel>();

			//Determine used channels
			for (int channelIndex = 0; channelIndex < _channels.Length; channelIndex++)
			{
				if (!data.Channels[channelIndex].Export) continue;
				bool used = false;

				for (int patternIndex = 0; patternIndex < data.PatternPages && !used; patternIndex++)
				{
					for (int row = 0; row < data.TotalRowsPerPattern && !used; row++)
					{
						NoteData noteData = data.Channels[channelIndex].Pages[patternIndex].Notes[row];
						ushort note = noteData.Note;

						if (note != 0 && note != (ushort)Notes.Off)
						{
							usedChannels.Add(MDChannels[channelIndex].Channel);
							used = true;
						}

						for (int effectIndex = 0; effectIndex < noteData.Effects.Length; effectIndex++)
						{
							short effectType = noteData.Effects[effectIndex].Type;
							psgNoiseUsed |= (effectType == (short)EffectType.PSGNoise);
						}
					}
				}
			}

			if (psgNoiseUsed)
			{
				if (!usedChannels.Contains(ESFChannel.PSG3))
					usedChannels.Add(ESFChannel.PSG3);

				if (!usedChannels.Contains(ESFChannel.PSG4))
					usedChannels.Add(ESFChannel.PSG4);
			}

			foreach(var channel in usedChannels)
			{
				output.Header.Add(new LockChannelEvent(channel));
			}
		}

		static private bool FindLoopBeginning(DMFData data, out int loopStartPatternIndex, out int loopStartRowIndex, out int loopEndPatternIndex, out int loopEndRowIndex)
		{
			loopStartPatternIndex = -1;
			loopStartRowIndex = -1;
			loopEndPatternIndex = -1;
			loopEndRowIndex = -1;

			for (int channelIndex = 0; channelIndex < data.Channels.Count; channelIndex++)
			{
				for (int patternIndex = 0; patternIndex < data.PatternPages; patternIndex++)
				{
					for (int row = 0; row < data.TotalRowsPerPattern; row++)
					{
						NoteData noteData = data.Channels[channelIndex].Pages[patternIndex].Notes[row];

						for (int effectIndex = 0; effectIndex < noteData.Effects.Length; effectIndex++)
						{
							short effectType = noteData.Effects[effectIndex].Type;
							if ((EffectType)effectType == EffectType.Jump)
							{
								loopStartPatternIndex = noteData.Effects[effectIndex].Value;
								loopStartRowIndex = 0;
								loopEndPatternIndex = patternIndex;
								loopEndRowIndex = row;
								return true;
							}
						}
					}
				}
			}

			return false;
		}

		public enum ChannelId
		{
			FM1 = 0,
			FM2,
			FM3,
			FM4,
			FM5,
			FM6,
			PSG1,
			PSG2,
			PSG3,
			PSG4,
		};

		static public readonly ChannelType[] ESF_CHANNEL_TYPES = new ChannelType[]
		{
			ChannelType.FM,        //0
			ChannelType.FM,        //1
			ChannelType.FM,        //2
			ChannelType.Invalid,   //3
			ChannelType.FM,        //4
			ChannelType.FM,        //5
			ChannelType.FM,        //6
			ChannelType.Invalid,   //7
			ChannelType.PSG,       //8
			ChannelType.PSG,       //9
			ChannelType.PSG,       //a
			ChannelType.PSG4,      //b
			ChannelType.FM6,       //c
		};

		private struct ChannelArray
		{
			public readonly ChannelId Id;
			public readonly ChannelType Type;
			public readonly ESFChannel Channel;

			public ChannelArray(ChannelId id, ChannelType type, ESFChannel channel)
				: this()
			{
				this.Id = id;
				this.Type = type;
				this.Channel = channel;
			}
		};

		static private readonly ChannelArray[] MDChannels = new ChannelArray[]
		{
			new ChannelArray(ChannelId.FM1, ChannelType.FM, ESFChannel.FM1),
			new ChannelArray(ChannelId.FM2, ChannelType.FM, ESFChannel.FM2),
			new ChannelArray(ChannelId.FM3, ChannelType.FM, ESFChannel.FM3),
			new ChannelArray(ChannelId.FM4, ChannelType.FM, ESFChannel.FM4),
			new ChannelArray(ChannelId.FM5, ChannelType.FM, ESFChannel.FM5),
			new ChannelArray(ChannelId.FM6, ChannelType.FM6, ESFChannel.FM6),
			new ChannelArray(ChannelId.PSG1, ChannelType.PSG, ESFChannel.PSG1),
			new ChannelArray(ChannelId.PSG2, ChannelType.PSG, ESFChannel.PSG2),
			new ChannelArray(ChannelId.PSG3, ChannelType.PSG, ESFChannel.PSG3),
			new ChannelArray(ChannelId.PSG4, ChannelType.PSG4, ESFChannel.PSG4)
		};

		static private readonly ChannelId[] ChannelProcessOrder = new ChannelId[]
		{
			ChannelId.FM1,
			ChannelId.FM2,
			ChannelId.FM3,
			ChannelId.FM4,
			ChannelId.FM5,
			ChannelId.FM6,
			ChannelId.PSG1,
			ChannelId.PSG2,
			ChannelId.PSG4,	// Process PSG 4 before 3, so PSG3 noise mode can take PSG4's last used note/octave
			ChannelId.PSG3
		};

		private const int MaxPSGFreqs = 12;
		private const int MaxFMFreqs = 12;
		private const int MaxOctave = 7;

		static private readonly ushort[][] PSGFreqs = new ushort[][] // [semitone][octave]
		{
						// 0   1   2   3   4   5,  6,  7,  8
			new ushort[]{ 851,851,425,212,106, 53, 26, 13, 1 }, // c
			new ushort[]{ 851,803,401,200,100, 50, 25, 12, 0 }, // c#
			new ushort[]{ 851,758,379,189, 94, 47, 23, 11, 0 }, // d
			new ushort[]{ 851,715,357,178, 89, 44, 22, 10, 0 }, // d#
			new ushort[]{ 851,675,337,168, 84, 42, 21, 9,  0 }, // e
			new ushort[]{ 851,637,318,159, 79, 39, 19, 8,  0 }, // f
			new ushort[]{ 851,601,300,150, 75, 37, 18, 7,  0 }, // f#
			new ushort[]{ 851,568,284,142, 71, 31, 15, 6,  0 }, // g
			new ushort[]{ 851,536,268,134, 67, 33, 16, 5,  0 }, // g#
			new ushort[]{ 851,506,253,126, 63, 31, 15, 4,  0 }, // a
			new ushort[]{ 851,477,238,119, 59, 29, 14, 3,  0 }, // a#
			new ushort[]{ 851,450,225,112, 56, 28, 14, 2,  0 }, // b
		};

		static private readonly ushort[] FMFreqs = new ushort[]
		{
			644,681,722,765,810,858,910,964,1021,1081,1146,1214
		};

		private const int MaxFMSlideFreqs = 13;
		static private readonly ushort[] FMSlideFreqs = new ushort[]
		{
			644,681,722,765,810,858,910,964,1021,1081,1146,1214,1288
		};

	}
}
