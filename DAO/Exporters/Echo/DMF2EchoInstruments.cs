using DefleMaskConvert.DAO.DefleMask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.Exporters.Echo
{
	public class DMF2EchoInstruments
	{

		public static List<Instrument> Convert(List<InstrumentData> instruments)
		{
			List<Instrument> list = new List<Instrument>();

			foreach(var instrumentData in instruments)
			{
				Instrument instrument = null;
				if (instrumentData is FMInstrumentData)
					instrument = ConvertInstrument(instrumentData as FMInstrumentData);

				if (instrumentData is PSGInstrumentData)
					instrument = ConvertInstrument(instrumentData as PSGInstrumentData);

				if (instrumentData is SampleData)
					instrument = ConvertSample(instrumentData as SampleData);

				if (instrument != null)
					list.Add(instrument);
			}

			return list;
		}

		private static FMInstrument ConvertInstrument(FMInstrumentData data)
		{
			FMInstrument fm = new FMInstrument(data.Name);
			fm.Algorith = data.Alg;
			fm.Feedback = data.Fb;

			fm.Operators = new List<FMOperator>();
			for (int i = 0; i < FMInstrument.TOTAL_OPERATORS; i++)
			{
				var opData = (i < data.Operators.Count) ? data.Operators[i] : data.Operators[data.Operators.Count - 1];
				var op = new FMOperator();
				fm.Operators.Add(op);

				op.Multiplier = opData.Mult;
				op.Detune = opData.Dt;
				op.TotalLevel = opData.Tl;
				op.AttackRate = opData.Ar;
				op.ReleaseScale = opData.Rs;
				op.DecayRate = opData.Dr;
				op.AmplitudeModulation = opData.Am;
				op.SustainRate = opData.D2r;
				op.ReleaseRate = opData.Rr;
				op.SusteinLevel = opData.Sl;
				op.SsgEg = opData.SsgMode;
			}

			return fm;
		}

		private static PSGInstrument ConvertInstrument(PSGInstrumentData data)
		{
			PSGInstrument psg = new PSGInstrument(data.Name);
			//Create envelope data (no loop = end stream looping last value (FE 00 FF))
			int loopDataSize = (data.Volume.LoopPosition == -1) ? 3 : 1;
			int dataSize = data.Volume.Size + loopDataSize;
			int streamEnd = dataSize - loopDataSize;

			psg.Data = new byte[dataSize];
			int offset = 0;
			int volumeIdx = 0;
			while (offset < dataSize)
			{
				if (offset == streamEnd)
				{
					//End of data
					if (loopDataSize == 1)
					{
						//End loop point
						psg.Data[offset++] = 0xFF;
					}
					else
					{
						//Loop last value
						psg.Data[offset++] = 0xFE;
						psg.Data[offset++] = (byte)(0xF - data.Volume.Data[volumeIdx - 1]);
						psg.Data[offset++] = 0xFF;
					}
				}
				else if (offset == data.Volume.LoopPosition)
				{
					//Loop start
					psg.Data[offset++] = 0xFE;
				}
				else// if (volumeIdx < data.Volume.Size)
				{
					psg.Data[offset++] = (byte)(0xF - data.Volume.Data[volumeIdx]);
					volumeIdx++;
				}
				//else
				//	psg.Data[offset++] = 0xFF;
			}

			return psg;
		}

		private static SampleInstrument ConvertSample(SampleData data)
		{
			SampleInstrument sample = new SampleInstrument(data.Name);
			sample.Data = new byte[data.Size + 1];

			for (int i = 0; i < data.Size; i++)
			{
				sample.Data[i] = (byte)(data.Data[i] & 0xFF);
				//Nudge 0xFF bytes to 0xFE
				if (sample.Data[i] == 0xFF)	sample.Data[i] = 0xFE;
			}

			//End of data
			sample.Data[data.Size] = 0xFF;

			return sample;
		}
	}
}
