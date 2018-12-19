using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.DefleMask
{
	public class PSGInstrumentData : InstrumentData
	{
		public const int DEFAULT_PSG_TONE = 0x0C;
		static public readonly PSGInstrumentData DEFAULT_PSG_INSTRUMENT = new PSGInstrumentData();

		public enum ArpeggioModes
		{
			Normal = 0,
			Fixed
		}

		public PSGEnvelopeData Volume { get; set; }
		public PSGEnvelopeData Arpeggio { get; set; }
		public ArpeggioModes ArpeggioMode { get; set; }
		public PSGEnvelopeData DutyNoise { get; set; }
		public PSGEnvelopeData WaveTable { get; set; }

		public PSGInstrumentData(string name)
			: base(name)
		{
		}

		private PSGInstrumentData()
			: this("Default Instrument")
		{
			Volume = new PSGEnvelopeData();
			Volume.Size = 1;
			Volume.Data = new int[] { DEFAULT_PSG_TONE };
			Volume.LoopPosition = -1;
		}

		#region Equal
		public override bool Equals(Object obj)
		{
			if (!(obj is PSGInstrumentData)) return false;

			PSGInstrumentData other = (PSGInstrumentData)obj;
			return	this.Volume == other.Volume &&
					this.Arpeggio == other.Arpeggio &&
					this.ArpeggioMode == other.ArpeggioMode &&
					this.DutyNoise == other.DutyNoise &&
					this.WaveTable == other.WaveTable;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public static bool operator ==(PSGInstrumentData x, PSGInstrumentData y)
		{
			if ((object)x == null && (object)y == null) return true;
			return (object)x != null && x.Equals(y);
		}

		public static bool operator !=(PSGInstrumentData x, PSGInstrumentData y)
		{
			return (object)x != null && !(x == y);
		}
		#endregion
	}

	public class PSGEnvelopeData
	{
		public byte Size { get; set; }
		public int[] Data { get; set; }
		public sbyte LoopPosition { get; set; }

		#region Equal
		public override bool Equals(Object obj)
		{
			if (obj is PSGEnvelopeData)
			{
				PSGEnvelopeData other = (PSGEnvelopeData)obj;
				if (this.Size == other.Size && this.LoopPosition == other.LoopPosition && this.Data.Length == other.Data.Length)
				{
					for (int i = 0; i < this.Data.Length; i++)
					{
						if (this.Data[i] != other.Data[i])
							return false;
					}

					return true;
				}
			}

			return false;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public static bool operator ==(PSGEnvelopeData x, PSGEnvelopeData y)
		{
			if ((object)x == null && (object)y == null) return true;
			return (object)x != null && x.Equals(y);
		}

		public static bool operator !=(PSGEnvelopeData x, PSGEnvelopeData y)
		{
			return (object)x != null && !(x == y);
		}
		#endregion
	}
}
