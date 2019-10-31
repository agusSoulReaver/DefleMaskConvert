using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.Exporters.Utils
{
	public class SFXPrioritiesExporter
	{
		private Moto68KWriter _writer;

		private SFXPrioritiesExporter(TextWriter stream, Dictionary<string, byte> data)
		{
			_writer = new Moto68KWriter(stream);

			foreach(var pair in data)
			{
				_writer.ConstantReference(pair.Key, pair.Value);
			}
		}

		static public void SaveFile(string path, Dictionary<string, byte> data)
		{
			File.WriteAllLines(path, new string[] { string.Empty });

			using (FileStream file = File.OpenWrite(path))
			{
				TextWriter stream = new StreamWriter(file, Encoding.ASCII);
				new SFXPrioritiesExporter(stream, data);

				stream.Flush();
				stream.Close();
			}
		}
	}
}
