using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.DefleMask
{
	public class SampleData : InstrumentData
	{
		public uint Size { get; set; }
		public byte SampleRate { get; set; }
		public byte Pitch { get; set; }
		public byte Amp { get; set; }
		public byte SampleBits { get; set; }

		public ushort[] Data { get; set; }

		public SampleData(string name)
			: base(name)
		{
		}

		#region Equal
		public override bool Equals(Object obj)
		{
			if (obj is SampleData)
			{
				SampleData other = (SampleData)obj;
				if (this.Size == other.Size &&
					this.SampleRate == other.SampleRate &&
					this.Data.Length == other.Data.Length &&
					this.Pitch == other.Pitch &&
					this.Amp == other.Amp &&
					this.SampleBits == other.SampleBits)
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

		public static bool operator ==(SampleData x, SampleData y)
		{
			if ((object)x == null && (object)y == null) return true;
			return (object)x != null && x.Equals(y);
		}

		public static bool operator !=(SampleData x, SampleData y)
		{
			return (object)x != null && !(x == y);
		}
		#endregion
	}
}
