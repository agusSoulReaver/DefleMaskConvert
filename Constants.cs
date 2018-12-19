using DefleMaskConvert.DAO.Exporters.Echo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert
{
	public enum ChannelType
	{
		Invalid,
		FM,    // Normal FM channel
		FM6,   // FM channel with DAC
		PSG,   // Normal PSG tone generator
		PSG4,
	};

	public enum PSGNoiseMode
	{
		WhiteHigh = 0,
		WhiteMedium,
		WhiteLow,
		WhitePSG3,
		PeriodicHigh,
		PeriodicMedium,
		PeriodicLow,
		PeriodicPSG3,
	};

	public enum Notes
	{
		Off = 100
	};

	public enum EffectMode
	{
		Off,     // all effects
		Normal,  // all effects except portamento, note slide
		Up,      // vibrato, portamento, note slide
		Down,    // vibrato, portamento, note slide
		Schedule,// porta note
	};

	public enum EffectStage
	{
		Off,
		Initialise,
		Continue,
		End
	};

	public enum EffectType
	{
		None						= 0xff, // No effect
		SetLFO						= 0x10, // LFO
		DACOn						= 0x17, // DAC enable (already done)
		PSGNoise					= 0x20, // PSG noise mode (already done)
		Arpeggio					= 0x00, // Arpeggio
		PortmentoUp					= 0x01, // Portamento up
		PortmentoDown				= 0x02, // Portamento down
		PortmentoToNote = 0x03, // Tone portamento
		Vibrato						= 0x04, // Vibrato
		//PortmentoToNoteAndVolSlide	= 0x05, // Tone portamento + volume slide
		VibratoAndVolSlide			= 0x06, // Vibrato + volume slide
		//Tremolo						= 0x07, // Tremolo
		Pan							= 0x08, // Set panning
		SetSpeed1					= 0x09, // Set speed 1
		SetSpeed2					= 0x0f, // Set speed 2
		VolumeSlide					= 0x0a, // Volume slide
		//NoteRetrigger				= 0x0c, // Note retrig

		//Extended effects
		//NoteSlideUp					= 0xe1, // Note slide up
		//NoteSlideDown				= 0xe2, // Note slide down
		//SetVibratoMode				= 0xe3, // Vibrato mode
		//SetFineVibrato				= 0xe4, // Fine vibrato depth
		//NoteCut						= 0xec, // Note cut
		NoteDelay					= 0xed,	// Note delay
		Jump						= 0x0b, // Position jump
		Break						= 0x0d, // Pattern break
	};

	public enum UnsupportedEffect
	{
		None						= 0xff, // No effect

		//PortmentoToNote				= 0x03, // Tone portamento
		PortmentoToNoteAndVolSlide	= 0x05, // Tone portamento + volume slide
		Tremolo						= 0x07, // Tremolo
		NoteRetrigger				= 0x0c, // Note retrig

		Feedback					= 0x11,
		TLOperator1					= 0x12,
		TLOperator2					= 0x13,
		TLOperator3					= 0x14,
		TLOperator4					= 0x15,
		MULT						= 0x16,
		ExtChn3Mode					= 0x18,
		GlobalAR					= 0x19,
		AROperator1					= 0x1A,
		AROperator2					= 0x1B,
		AROperator3					= 0x1C,
		AROperator4					= 0x1D,

		NoteSlideUp					= 0xe1, // Note slide up
		NoteSlideDown				= 0xe2, // Note slide down
		SetVibratoMode				= 0xe3, // Vibrato mode
		SetFineVibrato				= 0xe4, // Fine vibrato depth
		NoteCut						= 0xec, // Note cut

		SamplePitch					= 0xfe,
		SampleAmp					= 0xfd,
	}

	public enum FMRegister
	{
		LFO = 0x22,
		TimerA_MSBs = 0x24,
		TimerA_LSBs = 0x25,
		TimerB = 0x26
	};

	public enum ESFChannel
	{
		FM1 = 0x00,
		FM2 = 0x01,
		FM3 = 0x02,
		FM4 = 0x04,
		FM5 = 0x05,
		FM6 = 0x06,
		PSG1 = 0x08,
		PSG2 = 0x09,
		PSG3 = 0x0a,
		PSG4 = 0x0b,
		DAC = 0x0c,
	};

	public enum ESF_PCMRate
	{
		NotChange = 0x0,
		Freq26632Hz = 0x1,
		Freq17755Hz = 0x2,
		Freq13316Hz = 0x3,
		Freq10653Hz = 0x4,
		Freq8877Hz = 0x5,
		Freq7609Hz = 0x6,
		Freq6658Hz = 0x7,
		Freq5918Hz = 0x8,
		Freq5326Hz = 0x9,
		Freq4842Hz = 0xA,
		Freq4439Hz = 0xB,
		Freq4097Hz = 0xC,
	};

	public class Constants
	{
		public const ushort LAST_NOTE = 12;
		public const byte SAMPLE_BITS = 8;

		public const byte VER_24 = 24;
		public const byte VER_21 = 21;
		public const byte VER_19 = 19;
		public const byte VER_18 = 18;
		public const byte VER_17 = 17;
		static readonly private List<byte> SUPPORTED_FILE_VERSIONS = new List<byte>(new byte[]
		{
			VER_24,
			VER_21,
			VER_19,
			VER_18,
			VER_17
		});
		static readonly private List<string> SUPPORTED_DEFLEMASK_VERSIONS = new List<string>(new string[]
		{
			"0.12.0",
			"0.11.1",
			"0.11.0",
			"0.10c (@01/21/2015)",
			"0.9c (@06/09/13)"
		});

		static public int GetTotalSupportedVersions()
		{
			return SUPPORTED_FILE_VERSIONS.Count;
		}

		static public bool CheckIsSupportedVersion(byte version)
		{
			for (int i = 0; i < SUPPORTED_FILE_VERSIONS.Count; i++)
			{
				if (version == SUPPORTED_FILE_VERSIONS[i]) return true;
			}

			return false;
		}

		static public void GetDefleMaskVersion(int index, out byte version, out string name)
		{
			version = (byte)((index < SUPPORTED_FILE_VERSIONS.Count) ? SUPPORTED_FILE_VERSIONS[index] : 0);
			name = (index < SUPPORTED_DEFLEMASK_VERSIONS.Count) ? SUPPORTED_DEFLEMASK_VERSIONS[index] : "??";
		}

		public enum Systems
		{
			Genesis			= 0x02,
			GenesisExtCh3	= 0x12,
			SMS				= 0x03,
			Gameboy			= 0x04,
			PCEngine		= 0x05,
			NES				= 0x06,
			C64_SID_8580	= 0x07,
			C64_SID_6581	= 0x17,
			YM2151			= 0x08,
		}

		static public int GetSystemChannels(Systems system)
		{
			switch(system)
			{
				case Systems.Genesis:		return 10;
				case Systems.GenesisExtCh3:	return 13;
				case Systems.SMS:			return 4;
				case Systems.Gameboy:		return 4;
				case Systems.PCEngine:		return 6;
				case Systems.NES:			return 5;
				case Systems.C64_SID_8580:	return 3;
				case Systems.C64_SID_6581:	return 3;
				case Systems.YM2151:		return 13;
			}

			return -1;
		}

		static readonly private string[] NOTES = new string[]
		{
			"C-",
			"C#",
			"D-",
			"D#",
			"E-",
			"F-",
			"F#",
			"G-",
			"G#",
			"A-",
			"A#",
			"B-"
		};
		static public string GetNoteName(ushort note)
		{
			if(note < NOTES.Length) return NOTES[note];
			return "??";
		}

		static private readonly string[] ESF_CHAN_NAMES = new string[]
		{
			"FM 1", "FM 2", "FM 3", "", "FM 4", "FM 5", "FM 6", "", "PSG 1", "PSG 2", "PSG 3", "PSG 4", "PCM"
		};
		static public string GetChannelName(ESFChannel channel)
		{
			return ESF_CHAN_NAMES[(int)channel];
		}

		static readonly private string[] PSG_NOISE_ACTIONS = new string[]
		{
			"Periodic Noise, High Pitch",
			"Periodic Noise, Medium Pitch",
			"Periodic Noise, Low Pitch",
			"Periodic Noise, PSG3 Frequency",
			"White Noise, High Pitch",
			"White Noise, Medium Pitch",
			"White Noise, Low Pitch",
			"White Noise, PSG3 Frequency"
		};
		static public string GetPSGNoiseNoteOnName(ushort note)
		{
			if (note < PSG_NOISE_ACTIONS.Length) return PSG_NOISE_ACTIONS[note];
			return "??";
		}
	}
}
