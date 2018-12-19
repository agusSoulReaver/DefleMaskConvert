using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.DefleMask
{
	public class WaveTableData
	{
		public uint Size { get; set; }
		public uint[] Data { get; set; }

		#region Equal
		public override bool Equals(Object obj)
		{
			if (obj is WaveTableData)
			{
				WaveTableData other = (WaveTableData)obj;
				if (this.Size == other.Size && this.Data.Length == other.Data.Length)
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

		public static bool operator ==(WaveTableData x, WaveTableData y)
		{
			if ((object)x == null && (object)y == null) return true;
			return (object)x != null && x.Equals(y);
		}

		public static bool operator !=(WaveTableData x, WaveTableData y)
		{
			return (object)x != null && !(x == y);
		}
		#endregion
	}
}
