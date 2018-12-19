using DefleMaskConvert.DAO.DefleMask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.Importers.DMF
{
	public class DMFImporter : ZlibFileImporter
	{
		private const string HEADER = ".DelekDefleMask.";

		private const byte MAX_OPERATORS = 4;
		private const byte INSTRUMENT_FM = 1;
		private const byte INSTRUMENT_PSG = 0;


		public enum Result
		{
			OK,
			UnableToOpenFile,
			UnsupportedFileVersion,
			UnsupportedSystem,
			UnknowFileFormat,
			UnknowInstrumentType,
			UnsupportedSampleBits,
		}

		public DMFImporter(string path)
			: base(path)
		{
		}

		public Result ParseData(out DMFData output)
		{
			this.Reset();
			output = new DMFData();

			if (ReadString(16) != HEADER) return Result.UnknowFileFormat;
			output.FileVersion = ReadNumber8();
			if (!Constants.CheckIsSupportedVersion(output.FileVersion)) return Result.UnsupportedFileVersion;

			output.System = (DefleMaskConvert.Constants.Systems)ReadNumber8();
			if (output.System != Constants.Systems.Genesis) return Result.UnsupportedSystem;

			output.SongName = ReadString();
			output.SongAuthor = ReadString();

			output.HighlightA = ReadNumber8();
			output.HighlightB = ReadNumber8();

			output.TimeBase = ReadNumber8();
			output.TickTimeEven = ReadNumber8();
			output.TickTimeOdd = ReadNumber8();

			DMFData.FrameModes frameMode = (DMFData.FrameModes)ReadNumber8();
			if (ReadNumber8() == 1) frameMode = DMFData.FrameModes.Custom;
			output.FrameMode = frameMode;
			output.CustomHz1 = ReadNumber8();
			output.CustomHz2 = ReadNumber8();
			output.CustomHz3 = ReadNumber8();

			output.TotalRowsPerPattern = (output.FileVersion <= Constants.VER_21) ? ReadNumber8() : ReadNumber32();
			output.PatternPages = ReadNumber8();

			if (output.FileVersion <= Constants.VER_19) ReadNumber8(); // ignore data

			int channelCount = Constants.GetSystemChannels(output.System);
			output.PatternMatrix = new byte[channelCount][];
			for (int i = 0; i < channelCount; i++)
			{
				output.PatternMatrix[i] = new byte[output.PatternPages];
				for (int j = 0; j < output.PatternPages; j++)
				{
					output.PatternMatrix[i][j] = ReadNumber8();
				}
			}

			// INSTRUMENTS
			byte amount = ReadNumber8();
			output.Instruments = new List<InstrumentData>();
			for (int i = 0; i < amount; i++)
			{
				string instrumentName = ReadString();
				byte type = ReadNumber8();
				switch(type)
				{
					case INSTRUMENT_FM:
						var fm = ParseFMInstrument(instrumentName, output.FileVersion);
						fm = DMFGlobalInstruments.TrySaveInstrument<FMInstrumentData>(fm);
						output.Instruments.Add(fm);
						break;
					case INSTRUMENT_PSG:
						output.Instruments.Add(DMFGlobalInstruments.TrySaveInstrument<PSGInstrumentData>(ParsePSGInstrument(instrumentName)));
						break;

					default:
						return Result.UnknowInstrumentType;
				}
			}
			output.Instruments.Add(PSGInstrumentData.DEFAULT_PSG_INSTRUMENT);

			// WAVE TABLES
			amount = ReadNumber8();
			output.WaveTables = new List<WaveTableData>();
			for (int i = 0; i < amount; i++)
			{
				WaveTableData waveTable = new WaveTableData();
				output.WaveTables.Add(waveTable);

				waveTable.Size = ReadNumber32();
				waveTable.Data = new uint[waveTable.Size];
				for (int j = 0; j < waveTable.Size; j++)
				{
					waveTable.Data[j] = ReadNumber32();
				}
			}

			// CHANNELS
			output.Channels = new List<ChannelData>();
			for (int channelIndex = 0; channelIndex < channelCount; channelIndex++)
			{
				ChannelData channel = new ChannelData();
				output.Channels.Add(channel);

				channel.EffectsCount = ReadNumber8();
				channel.Pages = new PatternPage[output.PatternPages];
				for (int pageIndex = 0; pageIndex < output.PatternPages; pageIndex++)
				{
					PatternPage page = new PatternPage();
					channel.Pages[pageIndex] = page;
					
					page.Notes = new NoteData[output.TotalRowsPerPattern];
					for (int noteIndex = 0; noteIndex < output.TotalRowsPerPattern; noteIndex++)
					{
						NoteData note = new NoteData();
						page.Notes[noteIndex] = note;

						note.Note = ReadNumber16();
						note.Octave = ReadNumber16();
						note.Volume = (short)ReadNumber16();

						note.Effects = new NoteEffect[channel.EffectsCount];
						for (int effectIndex = 0; effectIndex < channel.EffectsCount; effectIndex++)
						{
							NoteEffect effect = new NoteEffect();
							note.Effects[effectIndex] = effect;

							effect.Type = (short)ReadNumber16();
							effect.Value = (short)ReadNumber16();
						}

						note.Instrument = (short)ReadNumber16();
					}
				}
			}

			// SAMPLES
			amount = ReadNumber8();
			output.Samples = new List<SampleData>();
			for (int i = 0; i < amount; i++)
			{
				uint size = ReadNumber32();
				string name = (output.FileVersion <= Constants.VER_21) ? string.Empty : ReadString();
				if (string.IsNullOrWhiteSpace(name)) name = DMFGlobalInstruments.GetNextDefaultName("UnnamedSample");
				SampleData sample = new SampleData(name);

				sample.Size = size;
				sample.SampleRate = ReadNumber8();
				sample.Pitch = ReadNumber8();
				sample.Amp = ReadNumber8();
				sample.SampleBits = (output.FileVersion >= Constants.VER_24) ? ReadNumber8() : Constants.SAMPLE_BITS;
				if (sample.SampleBits != Constants.SAMPLE_BITS) return Result.UnsupportedSampleBits;

				sample.Data = new ushort[sample.Size];
				for (int j = 0; j < sample.Size; j++)
				{
					sample.Data[j] = ReadNumber16();
				}

				output.Samples.Add(DMFGlobalInstruments.TrySaveInstrument<SampleData>(sample));
			}

			return Result.OK;
		}

		#region Instruments
		private FMInstrumentData ParseFMInstrument(string name, byte version)
		{
			FMInstrumentData data = new FMInstrumentData(name);

			data.Alg = ReadNumber8();
			if (version <= Constants.VER_18) ReadNumber8(); // ignore data
			data.Fb = ReadNumber8();
			if (version <= Constants.VER_18) ReadNumber8(); // ignore data
			data.LFO = ReadNumber8();
			if (version <= Constants.VER_18) ReadNumber8(); // ignore data

			byte totalOperators = MAX_OPERATORS;
			if (version <= Constants.VER_18) ReadNumber8(); // ignore data
			data.LFO2 = ReadNumber8();

			data.Operators = new List<FMOperatorData>();
			for (int i = 0; i < totalOperators; i++)
			{
				FMOperatorData op = new FMOperatorData();
				op.Am = ReadNumber8();
				op.Ar = ReadNumber8();
				if (version <= Constants.VER_18) ReadNumber8(); // ignore DAM
				op.Dr = ReadNumber8();
				if (version <= Constants.VER_18)
				{
					ReadNumber8(); // ignore DVB
					ReadNumber8(); // ignore EGT
					ReadNumber8(); // ignore KSL
				}

				op.Mult = ReadNumber8();
				op.Rr = ReadNumber8();
				op.Sl = ReadNumber8();
				if (version <= Constants.VER_18) ReadNumber8(); // ignore SUS
				op.Tl = ReadNumber8();
				if (version <= Constants.VER_18)
				{
					ReadNumber8(); // ignore VIB
					ReadNumber8(); // ignore WS
				}
				else
					op.Dt2 = ReadNumber8(); // this propertie isn't use in above versions in Genesis system

				op.Rs = ReadNumber8();
				op.Dt = ReadNumber8();
				op.D2r = ReadNumber8();
				op.SsgMode = ReadNumber8();

				data.Operators.Add(op);
			}

			return data;
		}

		private PSGInstrumentData ParsePSGInstrument(string name)
		{
			PSGInstrumentData data = new PSGInstrumentData(name);

			data.Volume = ParsePSGEnvelope();
			data.Arpeggio = ParsePSGEnvelope();
			data.ArpeggioMode = (PSGInstrumentData.ArpeggioModes)ReadNumber8();
			data.DutyNoise = ParsePSGEnvelope();
			data.WaveTable = ParsePSGEnvelope();

			return data;
		}

		private PSGEnvelopeData ParsePSGEnvelope()
		{
			PSGEnvelopeData data = new PSGEnvelopeData();

			data.Size = ReadNumber8();
			data.Data = new int[data.Size];
			for (int i = 0; i < data.Size; i++)
			{
				data.Data[i] = (int)ReadNumber32();
			}
			data.LoopPosition = data.Size > 0 ? (sbyte)ReadNumber8() : (sbyte)0;

			return data;
		}
		#endregion
	}
}
