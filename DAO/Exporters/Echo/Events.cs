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
		bool IsSameKind(IEchoEvent other);
	}

	public interface IEchoChannelEvent : IEchoEvent
	{
		ESFChannel Channel { get; }
		bool HadSameParameters(IEchoChannelEvent other);
	}

	public struct NoteOnEvent : IEchoEvent
	{
		public ESFChannel Channel { get; private set; }
		public readonly byte Note, Octave;
		private readonly byte _data;
		public readonly byte InstrumentIndex;

		public NoteOnEvent(ESFChannel channel, ChannelType channelType, byte note, byte octave, byte instrumentIndex)
			: this()
		{
			Channel = channel;
			Note = note;
			Octave = octave;
			InstrumentIndex = instrumentIndex;

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

		public bool IsSameKind(IEchoEvent other)
		{
			return other is NoteOnEvent && ((NoteOnEvent)other).Channel == this.Channel;
		}
	}

	public struct NoteOffEvent : IEchoEvent
	{
		public ESFChannel Channel { get; private set; }

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

		public bool IsSameKind(IEchoEvent other)
		{
			return other is NoteOffEvent && ((NoteOffEvent)other).Channel == this.Channel;
		}
	}

	public struct SetVolumeEvent : IEchoChannelEvent
	{
		public ESFChannel Channel { get; private set; }
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

		public bool IsSameKind(IEchoEvent other)
		{
			return other is SetVolumeEvent && ((SetVolumeEvent)other).Channel == this.Channel;
		}

		public bool HadSameParameters(IEchoChannelEvent other)
		{
			if (this.IsSameKind(other))
			{
				return ((SetVolumeEvent)other).Volume == this.Volume;
			}

			return false;
		}
	}

	public struct SetFrequencyEvent : IEchoEvent
	{
		public ESFChannel Channel { get; private set; }
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

		public bool IsSameKind(IEchoEvent other)
		{
			return other is SetFrequencyEvent && ((SetFrequencyEvent)other).Channel == this.Channel;
		}

	}

	public struct SetPSGNoiseFrequency : IEchoChannelEvent
	{
		private SetFrequencyEvent _wrapper;

		public ESFChannel Channel { get { return _wrapper.Channel; } }

		public SetPSGNoiseFrequency(ESFChannel channel, ushort frequency)
			:this()
		{
			_wrapper = new SetFrequencyEvent(channel, frequency);
		}

		public byte[] GetBinaryData()
		{
			return _wrapper.GetBinaryData();
		}

		public string GetComment(out int tabAmount)
		{
			return _wrapper.GetComment(out tabAmount);
		}

		public bool IsSameKind(IEchoEvent other)
		{
			return other is SetPSGNoiseFrequency && ((SetPSGNoiseFrequency)other).Channel == this.Channel;
		}

		public bool HadSameParameters(IEchoChannelEvent other)
		{
			if (this.IsSameKind(other))
			{
				return ((SetPSGNoiseFrequency)other)._wrapper.Frequency == this._wrapper.Frequency;
			}

			return false;
		}
	}

	public struct SetInstrumentEvent : IEchoChannelEvent
	{
		public ESFChannel Channel { get; private set; }
		public readonly byte InstrumentIndex;
		public readonly string InstrumentName;

		public SetInstrumentEvent(ESFChannel channel, byte instrumentIndex, string name)
			: this()
		{
			if (channel == ESFChannel.DAC)
				throw new ArgumentException("You can't set DAC's instrument!");

			Channel = channel;
			InstrumentIndex = instrumentIndex;
			InstrumentName = name;
		}

		public byte[] GetBinaryData()
		{
			return new byte[] { (byte)((byte)Channel | 0x40), InstrumentIndex };
		}

		public string GetComment(out int tabAmount)
		{
			tabAmount = 2;
			return string.Format(@"Set instrument ""{1}"" for channel {0}", Constants.GetChannelName(Channel), InstrumentName);
		}

		public bool IsSameKind(IEchoEvent other)
		{
			return other is SetInstrumentEvent && ((SetInstrumentEvent)other).Channel == this.Channel;
		}

		public bool HadSameParameters(IEchoChannelEvent other)
		{
			if (this.IsSameKind(other))
			{
				return ((SetInstrumentEvent)other).InstrumentIndex == this.InstrumentIndex;
			}

			return false;
		}
	}

	public struct DelayEvent : IEchoEvent
	{
		public const int SHORT_DELAY_LIMIT = 0x10;
		public const int DELAY_LIMIT = byte.MaxValue + 1;

		public readonly byte Ticks;
		public readonly bool IsShort;

		public bool IsFullDelay { get { return !IsShort && Ticks == 0; } }

		public DelayEvent(byte ticks, bool isShort)
			: this()
		{
			Ticks = isShort ? (byte)((ticks-1) & 0x0F) : ticks;
			IsShort = isShort;
		}

		public int GetRealTicks()
		{
			return IsShort ? Ticks + 1 : Ticks > 0 ? Ticks : DELAY_LIMIT;
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

		public bool IsSameKind(IEchoEvent other)
		{
			return other is DelayEvent;
		}
	}

	public struct LockChannelEvent : IEchoEvent
	{
		public ESFChannel Channel { get; private set; }

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

		public bool IsSameKind(IEchoEvent other)
		{
			return other is LockChannelEvent && ((LockChannelEvent)other).Channel == this.Channel;
		}
	}

	public struct SetFMParametersEvent : IEchoChannelEvent
	{
		public ESFChannel Channel { get; private set; }
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
			if (pan == 0) pan = 0x3;

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

		public bool IsSameKind(IEchoEvent other)
		{
			return other is SetFMParametersEvent && ((SetFMParametersEvent)other).Channel == this.Channel;
		}


		public bool HadSameParameters(IEchoChannelEvent other)
		{
			if (this.IsSameKind(other))
				return ((SetFMParametersEvent)other)._data == this._data;

			return false;
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

		public bool IsSameKind(IEchoEvent other)
		{
			return other is SetBankRegisterEvent;
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

		public bool IsSameKind(IEchoEvent other)
		{
			return other is PlaybackEvent;
		}
	}

}
