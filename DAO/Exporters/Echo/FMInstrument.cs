using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.Exporters.Echo
{
	public class FMInstrument : Instrument
	{
		public const int TOTAL_OPERATORS = 4;

		public FMInstrument(string name)
			: base(name)
		{
		}

		public byte Algorith { get; set; }
		public byte Feedback { get; set; }

		public List<FMOperator> Operators { get; set; }

		override public byte[] GetBinaryData()
		{
			List<byte[]> op = new List<byte[]>();
			for (int i = 0; i < Operators.Count; i++)
			{
				op.Add(Operators[i].GetBinaryData());
			}

			byte[] data = new byte[(op.Count * FMOperator.BINARY_SIZE) + 1];
			data[0]	= (byte)((Feedback << 3) | Algorith);

			for (int i = 0; i < FMOperator.BINARY_SIZE; i++)
			{
				for (int opIndex = 0; opIndex < op.Count; opIndex++)
				{
					data[(opIndex + (i * op.Count)) + 1] = op[opIndex][i];
				}
			}

			return data;
		}

	}


	public class FMOperator
	{
		public const int BINARY_SIZE = 7;
		static readonly private sbyte[] DETUNE_TABLE = new sbyte[] { -3, -2, -1, 0, 1, 2, 3 };

		public byte Multiplier { get; set; }
		public byte Detune { get; set; }

		public byte TotalLevel { get; set; }

		public byte AttackRate { get; set; }
		public byte ReleaseScale { get; set; }

		public byte DecayRate { get; set; }
		public byte AmplitudeModulation { get; set; }

		public byte SustainRate { get; set; }
		
		public byte ReleaseRate { get; set; }
		public byte SusteinLevel { get; set; }
		
		public byte SsgEg { get; set; }

		public byte[] GetBinaryData()
		{
			byte[] data = new byte[BINARY_SIZE];

			byte detune;
			if (DETUNE_TABLE[Detune] < 0)
			{
				detune = (byte)(Math.Abs(DETUNE_TABLE[Detune]) & 0x3);
				detune |= 0x4;
			}
			else
				detune = (byte)(DETUNE_TABLE[Detune] & 0x3);

			data[0] = (byte)((detune << 4) | Multiplier);
			data[1] = TotalLevel;
			data[2] = (byte)((ReleaseScale << 6) | AttackRate);
			data[3] = (byte)((AmplitudeModulation << 7) | DecayRate);
			data[4] = SustainRate;
			data[5] = (byte)((SusteinLevel << 4) | ReleaseRate);
			data[6] = SsgEg;

			return data;
		}

	}
}
