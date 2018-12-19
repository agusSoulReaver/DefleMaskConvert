using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.DefleMask
{
	public class FMInstrumentData : InstrumentData
	{
		public FMInstrumentData(string name)
			: base(name)
		{
		}

		public byte Alg { get; set; }
		public byte Fb { get; set; }
		public byte LFO { get; set; }
		public byte LFO2 { get; set; }

		public List<FMOperatorData> Operators { get; set; }

		#region Equal
		public override bool Equals(Object obj)
		{
			if (obj is FMInstrumentData)
			{
				FMInstrumentData other = (FMInstrumentData)obj;
				if (this.Alg == other.Alg &&
					this.Fb == other.Fb &&
					this.LFO == other.LFO &&
					this.LFO2 == other.LFO2 &&
					this.Operators.Count == other.Operators.Count)
				{
					for (int i = 0; i < this.Operators.Count; i++)
					{
						if (this.Operators[i] != other.Operators[i])
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

		public static bool operator ==(FMInstrumentData x, FMInstrumentData y)
		{
			if ((object)x == null && (object)y == null) return true;
			return (object)x != null && x.Equals(y);
		}

		public static bool operator !=(FMInstrumentData x, FMInstrumentData y)
		{
			return (object)x != null && !(x == y);
		}
		#endregion
	}

	public class FMOperatorData
	{
		public byte Am { get; set; }
		public byte Ar { get; set; }
		public byte Dr { get; set; }
		public byte Mult { get; set; }
		public byte Rr { get; set; }
		public byte Sl { get; set; }
		public byte Tl { get; set; }
		public byte Dt2 { get; set; }
		public byte Rs { get; set; }
		public byte Dt { get; set; }
		public byte D2r { get; set; }
		public byte SsgMode { get; set; }

		#region Equal
		public override bool Equals(Object obj)
		{
			if (!(obj is FMOperatorData)) return false;

			FMOperatorData other = (FMOperatorData)obj;
			return	this.Am == other.Am &&
					this.Ar == other.Ar &&
					this.Dr == other.Dr &&
					this.Mult == other.Mult &&
					this.Rr == other.Rr &&
					this.Sl == other.Sl &&
					this.Tl == other.Tl &&
					this.Dt2 == other.Dt2&&
					this.Rs == other.Rs &&
					this.Dt == other.Dt &&
					this.D2r == other.D2r &&
					this.SsgMode == other.SsgMode;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public static bool operator ==(FMOperatorData x, FMOperatorData y)
		{
			if ((object)x == null && (object)y == null) return true;
			return (object)x != null && x.Equals(y);
		}

		public static bool operator !=(FMOperatorData x, FMOperatorData y)
		{
			return (object)x != null && !(x == y);
		}
		#endregion
	}
}
