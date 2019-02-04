/*
 * This class it's based in the project dmf2esf (https://github.com/BigEvilCorporation/dmf2esf).
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.Exporters.Echo
{
	internal class ProcessingChannel
	{
		public readonly DMF2EchoESF.ChannelId Id;
		public readonly ChannelType Type;
		public readonly ESFChannel ESFId;
		public readonly bool Export;

		public ProcessingChannel(DMF2EchoESF.ChannelId id, ChannelType type, ESFChannel esfId, bool export)
		{
			Id = id;
			Type = type;
			ESFId = esfId;
			Export = export;
		}

		//public byte EffectCount = 0;

		public bool NoteOn = false;

		public byte Note = 0;
		public byte Octave = 0;
		public ushort NoteFreq = 0; // freq of last note on
		public ushort ToneFreq = 0;
		public byte NewNote = 0;
		public byte NewOctave = 0;
		public byte EffectNote = 0;
		public byte EffectOctave = 0;
		public int EffectSemitone = 0;
		public ushort LastFreq = 0; // difference
		public ushort NewFreq = 0;  // difference
		public byte Instrument = 0xFF;
		public byte NewInstrument = 0;
		public byte Volume = 0x7F;
		public byte NewVolume = 0;
		public byte LastVolume = 0x7F;
		public byte SubtickFX = 0;       // 0=none, >0=yes
		public byte lastPanning = 0x11;
		public byte lastFMS = 0;
		public byte lastAMS = 0;

		public bool MustChangeInstrument
		{
			get { return NewInstrument != Instrument && NewInstrument != 0xff; }
		}

		public EffectArpeggio m_effectArpeggio = new EffectArpeggio();
		public EffectPortmento m_effectPortmento = new EffectPortmento();
		public EffectPortaNote m_effectPortaNote = new EffectPortaNote();
		public EffectVibrato m_effectVibrato = new EffectVibrato();
		public EffectTremolo m_effectTremolo = new EffectTremolo();
		public EffectVolSlide m_effectVolSlide = new EffectVolSlide();
		public EffectRetrigger m_effectRetrigger = new EffectRetrigger();
		public EffectNoteSlide m_effectNoteSlide = new EffectNoteSlide();
		public EffectNoteCut m_effectNoteCut = new EffectNoteCut();
		public EffectNoteDelay m_effectNoteDelay = new EffectNoteDelay();
		public EffectPSGNoise m_effectPSGNoise = new EffectPSGNoise();
		public EffectFineTune FineTune = new EffectFineTune();
	}

	internal class EffectArpeggio
	{
		public EffectArpeggio() { Arp = EffectMode.Off; }

		//0xx (arpeggio)
		public EffectMode Arp;
		public ushort Arp1;
		public ushort Arp2;
		public byte ArpCounter;
	};

	internal class EffectPortmento
	{
		public EffectPortmento() { Porta = EffectMode.Off; }

		//1xx, 2xx (portamento)
		public EffectMode Porta;
		public EffectStage Stage;
		public byte PortaSpeed;
		public bool NoteOnthisTick;
	};

	internal class EffectPortaNote
	{
		public EffectPortaNote() { PortaNote = EffectMode.Off; }

		//3xx (porta to note), cancels out 1xx,2xx
		public EffectMode PortaNote;
		public EffectStage Stage;
		public byte PortaNoteSpeed;
		public byte PortaNoteCurrentNote;
		public byte PortaNoteTargetNote;
		public byte PortaNoteCurrentOctave;
		public byte PortaNoteTargetOctave;
	};

	internal class EffectVibrato
	{
		public EffectVibrato()
		{
			mode = EffectMode.Off;
			sineTime = 0;
		}

		//4xx (vibrato)
		public EffectMode mode;
		public EffectStage stage;
		public byte sineSpeed;
		public byte sineAmplitude;
		public uint sineTime;
	};

	// 5xx, 6xx ignored

	internal class EffectTremolo
	{
		public EffectTremolo() { Tremolo = EffectMode.Off; }

		// 7xx (tremolo)
		public EffectMode Tremolo;
		public byte TremoloActive;
		public byte TremoloDepth;
		public byte TremoloSpeed;
		public byte TremoloOffset;
	};

	// 8xx (panning) doesn't need variables

	internal class EffectVolSlide
	{
		public EffectVolSlide() { VolSlide = EffectMode.Off; }

		// Axx (volume slide)
		public EffectMode VolSlide;
		public sbyte VolSlideValue;
		public ushort CurrVol;
	};

	// Bxx (position jump, global effect)

	internal class EffectRetrigger
	{
		public EffectRetrigger() { Retrig = EffectMode.Off; }

		// Cxx (note retrig)
		public EffectMode Retrig;
		public byte RetrigSpeed;
	};

	// Dxx (pattern break, global effect)

	internal class EffectNoteSlide
	{
		public EffectNoteSlide() { NoteSlide = EffectMode.Off; }

		// E1xy, E2xy (note slide)
		public EffectMode NoteSlide;
		public byte NoteSlideSpeed;
		public byte NoteSlideFinal;
	};

	// E3xx (set vibrato mode)
	// E4xx (set fine vibrato depth)

	internal class EffectFineTune
	{
		public EffectFineTune() { Mode = EffectMode.Off; }

		// E5xx
		public EffectMode Mode;
		public float NoteOffset;
		public ushort NoteFrequency;
		public byte NoteOctave;
	};

	// EBxx (set sample bank, no support for now)

	internal class EffectNoteCut
	{
		public EffectNoteCut() { NoteCut = EffectMode.Off; }

		// ECxx (note cut)
		public EffectMode NoteCut;
		public byte NoteCutActive;
		public byte NoteCutOffset;
	};

	internal class EffectNoteDelay
	{
		public EffectNoteDelay() { NoteDelay = EffectMode.Off; }

		// EDxx (note delay)
		public EffectMode NoteDelay;
		public byte NoteDelayOffset;
	};

	internal class EffectPSGNoise
	{
		public EffectPSGNoise() { Mode = EffectMode.Off; EnvelopeSize = 0; }

		public EffectMode Mode;
		public byte EnvelopeIdx;
		public byte EnvelopeSize;
		public int[] EnvelopeData;
	};

	// EBxx (global fine tune, no support for now)
	// 17xx (DAC enable)

	// 11xy, 12xx, 13xx ... (operator modifying effects not supported by Echo)
}
