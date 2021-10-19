using DefleMaskConvert.DAO.Exporters.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.Exporters.Echo
{
	public class EchoInstruments2ASM
	{
		private const int MAX_COLUMNS = 16;

		private Moto68KWriter _writer;

		private EchoInstruments2ASM(TextWriter stream, List<Instrument> instruments)
		{
			_writer = new Moto68KWriter(stream);

			List<string> labels = new List<string>();
			foreach (var instrument in instruments)
			{
				string label = null;
				if(instrument is FMInstrument)
					label = WriteInstrument(instrument as FMInstrument);

				if (instrument is PSGInstrument)
					label = WriteInstrument(instrument as PSGInstrument);

				if (instrument is SampleInstrument)
					label = WriteInstrument(instrument as SampleInstrument);

				if (label != null)
				{
					labels.Add(label);
					_writer.Even();
					_writer.NewLine();
				}
			}

			if (labels.Count > 0)
			{
				_writer.Label("GlobalInstruments");
				foreach (var instrument in labels)
				{
					_writer.EchoListEntry(instrument);
				}
				_writer.EchoListEnd();
				_writer.Even();
			}
		}

		private string WriteInstrument(FMInstrument instrument)
		{
			string label = "FM_"+ FilterLabel(instrument.Name);
			byte[] data = instrument.GetBinaryData();

			_writer.Label(label);
			_writer.DefineConstant(data[0], Moto68KWriter.Formats.Hexa);

			for (int i = 0; i < FMOperator.BINARY_SIZE; i++)
			{
				_writer.DefineConstantHeader(Moto68KWriter.Sizes.Byte);
				for (int opIndex = 0; opIndex < FMInstrument.TOTAL_OPERATORS; opIndex++)
				{
					if (opIndex > 0) _writer.Text(", ");
					_writer.Number(data[(opIndex + (i * FMInstrument.TOTAL_OPERATORS)) + 1], Moto68KWriter.Formats.Hexa);
				}

				_writer.NewLine();
			}

			return label;
		}

		private string WriteInstrument(PSGInstrument instrument)
		{
			string label = "PSG_" + FilterLabel(instrument.Name);

			_writer.Label(label);
			WriteByteArray(instrument.Data);

			return label;
		}

		private string WriteInstrument(SampleInstrument instrument)
		{
			string label = "PCM_" + FilterLabel(instrument.Name);

			_writer.Label(label);
			WriteByteArray(instrument.Data);

			return label;
		}

		private void WriteByteArray(byte[] data)
		{
			for (int i = 0; i < data.Length; )
			{
				_writer.DefineConstantHeader(Moto68KWriter.Sizes.Byte);
				for (int j = 0; j < MAX_COLUMNS && i < data.Length; j++, i++)
				{
					if (j > 0)
					{
						_writer.Text(",");
						if(j % 4 == 0) _writer.Text(" ");
					}
					_writer.Number(data[i], Moto68KWriter.Formats.Hexa);
				}

				_writer.NewLine();
			}
		}

		private string FilterLabel(string label)
		{
			return Regex.Replace(label, "[ .$@+-]", "_");
		}

		static public void SaveFile(string path, List<Instrument> instruments)
		{
			File.WriteAllLines(path, new string[] { string.Empty });

			using (FileStream file = File.OpenWrite(path))
			{
				TextWriter stream = new StreamWriter(file, Encoding.ASCII);
				new EchoInstruments2ASM(stream, instruments);

				stream.Flush();
				stream.Close();
			}
		}
	}
}
