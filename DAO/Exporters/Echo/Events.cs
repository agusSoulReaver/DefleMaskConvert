using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.Exporters.Echo
{

	public interface IEchoEvent
	{
		byte[] GetBinaryData();
		string GetComment(out int tabAmount);
	}

	public struct NoteOnEvent : IEchoEvent
	{
		public readonly ESFChannel Channel;
		public readonly byte Note, Octave;
		private readonly byte _data;

		public NoteOnEvent(ESFChannel channel, ChannelType channelType, byte note, byte octave)
			: this()
		{
			Channel = channel;
			Note = note;
			Octave = octave;

			byte data;
			switch (channelType)
			{
				case ChannelType.FM:
					data = (byte)(32 * octave + 2 * note + 1);
					break;
				case ChannelType.PSG:
					octave = (byte)((octave > 0) ? octave - 1: 0);
					data = (byte)(24 * octave + 2 * note);
					break;
				case ChannelType.PSG4:
				case ChannelType.FM6:
					data = note;
					break;
				default:
					data = note;
					//fprintf(stderr, "WARNING: Unknown channel type (wtf)\n");
					break;
			}

			_data = data;
		}

		public byte[] GetBinaryData()
		{
			return new byte[] { (byte)Channel, _data };
		}

		public string GetComment(out int tabAmount)
		{
			tabAmount = 2;
			string channelName = Constants.GetChannelName(Channel);
			switch (DMF2EchoESF.ESF_CHANNEL_TYPES[(int)Channel])
			{
				case ChannelType.PSG4:
					return string.Format("{0} on channel {1}", Constants.GetPSGNoiseNoteOnName(Note), channelName);

				case ChannelType.FM6:
					return string.Format("Sample {0} on channel {1}", Note, channelName);

				default:
					return string.Format("Note {0}{1} on channel {2}", Constants.GetNoteName(Note), Octave, channelName);
			}
		}
	}

	public struct NoteOffEvent : IEchoEvent
	{
		public readonly ESFChannel Channel;

		public NoteOffEvent(ESFChannel channel)
			: this()
		{
			Channel = channel;
		}

		public byte[] GetBinaryData()
		{
			return new byte[] { (byte)((byte)Channel | 0x10) };
		}

		public string GetComment(out int tabAmount)
		{
			tabAmount = 3;
			return string.Format("Note off channel {0}", Constants.GetChannelName(Channel));
		}
	}

	public struct SetVolumeEvent : IEchoEvent
	{
		public readonly ESFChannel Channel;
		public readonly byte Volume;

		public SetVolumeEvent(ESFChannel channel, byte volume)
			: this()
		{
			Channel = channel;
			Volume = volume;
		}

		public byte[] GetBinaryData()
		{
			return new byte[] { (byte)((byte)Channel | 0x20), Volume };
		}

		public string GetComment(out int tabAmount)
		{
			tabAmount = 2;
			return string.Format("Set volume for channel {0}", Constants.GetChannelName(Channel));
		}
	}

	public struct SetFrequencyEvent : IEchoEvent
	{
		public readonly ESFChannel Channel;
		public readonly ushort Frequency;
		private readonly byte[] _data;

		public SetFrequencyEvent(ESFChannel channel, ushort frequency)
			: this()
		{
			if (channel == ESFChannel.DAC)
				throw new ArgumentException("You can't set DAC's frequency!");

			byte esfFreq1 = 0;
			byte esfFreq2 = 0;
			int numBytes = 1;

			//If the 2nd byte's MSB is clear, the event is three bytes long. The 2nd
			//and 3rd bytes specify the new frequency, the 2nd byte containing the
			//four least significant bits(LSB aligned), and the 3rd byte containing
			//the six most significant bits(LSB aligned too).

			switch (DMF2EchoESF.ESF_CHANNEL_TYPES[(int)channel])
			{
				case ChannelType.FM:
					esfFreq1 = (byte)(frequency >> 8);
					esfFreq2 = (byte)(frequency & 0xFF);
					numBytes = 2;
					break;
				case ChannelType.PSG:
					// Format: XXXX3210 XX987654
					esfFreq1 = (byte)(frequency & 0x0F);// XXXX3210
					esfFreq2 = (byte)(frequency >> 4);	// XX987654
					numBytes = 2;
					break;
				case ChannelType.PSG4:
					esfFreq1 = (byte)(frequency & 0x07);
					numBytes = 1;
					break;
				default:
					throw new ArgumentException("WARNING: Attempting to set frequency on a channel that doesn't support it!");
			}

			byte[] data = new byte[numBytes];
			data[0] = esfFreq1;
			if (numBytes > 1) data[1] = esfFreq2;

			Channel = channel;
			Frequency = frequency;
			_data = data;
		}

		public byte[] GetBinaryData()
		{
			byte header = (byte)((byte)Channel | 0x30);
			byte[] binary = new byte[_data.Length+1];

			binary[0] = header;
			for (int i = 0; i < _data.Length; i++)
			{
				binary[i+1] = _data[i];
			}

			return binary;
		}

		public string GetComment(out int tabAmount)
		{
			tabAmount = 3 - _data.Length;
			string name = Constants.GetChannelName(Channel);

			switch (DMF2EchoESF.ESF_CHANNEL_TYPES[(int)Channel])
			{
				case ChannelType.FM:
					return string.Format("Set frequency '{0}' (octave {1} semitone {2}) for channel {3}", Frequency, (Frequency >> 11), (Frequency & 0x7FF), name);
				case ChannelType.PSG:
				case ChannelType.PSG4:
					return string.Format("Set frequency '{0}' for channel {1}", Frequency, name);
			}

			return null;
		}
	}

	public struct SetInstrumentEvent : IEchoEvent
	{
		public readonly ESFChannel Channel;
		public readonly byte InstrumentIndex;

		public SetInstrumentEvent(ESFChannel channel, byte instrumentIndex)
			: this()
		{
			if (channel == ESFChannel.DAC)
				throw new ArgumentException("You can't set DAC's instrument!");

			Channel = channel;
			InstrumentIndex = instrumentIndex;
		}

		public byte[] GetBinaryData()
		{
			return new byte[] { (byte)((byte)Channel | 0x40), InstrumentIndex };
		}

		public string GetComment(out int tabAmount)
		{
			tabAmount = 2;
			return string.Format("Set instrument for channel {0}", Constants.GetChannelName(Channel));
		}
	}

	public struct DelayEvent : IEchoEvent
	{
		public readonly byte Ticks;
		public readonly bool IsShort;

		public DelayEvent(byte ticks, bool isShort)
			: this()
		{
			Ticks = isShort ? (byte)((ticks-1) & 0x0F) : ticks;
			IsShort = isShort;
		}

		public byte[] GetBinaryData()
		{
			if (IsShort) return new byte[] { (byte)(Ticks | 0xD0) };

			return new byte[] { 0xFE, Ticks };
		}

		public string GetComment(out int tabAmount)
		{
			tabAmount = 2;
			if (IsShort)
			{
				tabAmount++;
				return string.Format("Short delay. {0} ticks", Ticks + 1);
			}

			return string.Format("Delay {0} ticks", Ticks == 0 ? 0x100 : Ticks);
		}
	}

	public struct LockChannelEvent : IEchoEvent
	{
		public readonly ESFChannel Channel;

		public LockChannelEvent(ESFChannel channel)
			: this()
		{
			Channel = channel == ESFChannel.DAC ? ESFChannel.FM6 : channel;
		}

		public byte[] GetBinaryData()
		{
			return new byte[] { (byte)((byte)Channel | 0xE0) };
		}

		public string GetComment(out int tabAmount)
		{
			tabAmount = 3;
			return string.Format("Lock channel {0}", Constants.GetChannelName(Channel));
		}
	}

	public struct SetFMParametersEvent : IEchoEvent
	{
		public readonly ESFChannel Channel;
		public readonly byte Pan, AMS, FMS;
		private readonly byte _data;

		public SetFMParametersEvent(ESFChannel channel, byte pan, byte AMS, byte FMS)
			: this()
		{
			if(channel > ESFChannel.FM6)
				throw new ArgumentException("You can only set parameters in FM channels!");

			// Panning = %000L000R
			byte panL = (byte)((pan & 0x10) >> 4);
			byte panR = (byte)(pan & 0x01);
			pan = (byte)((panL << 1) | panR);
			byte regB4Pan = (byte)(pan << 6);

			// %LRAA0FFF
			// L = left
			// R = right
			// A = AMS
			// F = FMS
			byte data = (byte)(regB4Pan | (AMS << 4) | FMS);

			Channel = channel;
			_data = data;
			this.AMS = AMS;
			this.FMS = FMS;
			Pan = pan;
		}

		public byte[] GetBinaryData()
		{
			return new byte[] { (byte)((byte)Channel | 0xF0), _data };
		}

		public string GetComment(out int tabAmount)
		{
			tabAmount = 2;
			return string.Format("Set params channel {0}. Pan=({1}|{2}) AMS={3} FMS={4}",
				Constants.GetChannelName(Channel),
				((Pan & 0x2) != 0) ? "L" : "-",
				((Pan & 0x1) != 0) ? "R" : "-",
				AMS, FMS);
		}
	}

	public struct SetBankRegisterEvent : IEchoEvent
	{
		public enum Banks
		{
			Bank0 = 0xF8,
			Bank1 = 0xF9,
		}

		public readonly Banks Bank;
		public readonly FMRegister Register;
		public readonly byte Value;

		private string _comment;

		public SetBankRegisterEvent(Banks bank, FMRegister register, byte value, string comment)
			: this()
		{
			Bank = bank;
			Register = register;
			Value = value;
			_comment = comment;
		}

		public byte[] GetBinaryData()
		{
			return new byte[] { (byte)Bank, (byte)Register, Value };
		}

		public string GetComment(out int tabAmount)
		{
			tabAmount = 1;
			return _comment;
		}
	}

	public struct PlaybackEvent : IEchoEvent
	{
		public enum Actions
		{
			GoToLoop	= 0xFC,
			SetLoop		= 0xFD,
			Stop		= 0xFF,
		}

		public readonly Actions Action;

		public PlaybackEvent(Actions action)
			: this()
		{
			Action = action;
		}

		public byte[] GetBinaryData()
		{
			return new byte[] { (byte)Action };
		}

		public string GetComment(out int tabAmount)
		{
			tabAmount = 3;
			return Action.ToString();
		}
	}

}
