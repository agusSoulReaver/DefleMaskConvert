using DefleMaskConvert.DAO.Exporters.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.Exporters
{
	public class ProjectExporter
	{
		private Moto68KWriter _writer;

		private ProjectExporter(TextWriter stream, List<string> includes, List<KeyValuePair<string, string>> includesWLabels)
		{
			_writer = new Moto68KWriter(stream);

			foreach(var item in includes)
			{
				_writer.Include(item);
			}
			_writer.NewLine();

			foreach (var item in includesWLabels)
			{
				_writer.Label(item.Key);
				_writer.Include(item.Value);
			}
			_writer.NewLine();

			_writer.Even();
		}

		static public void Save(string path, List<string> includes, List<KeyValuePair<string, string>> includesWLabels)
		{
			File.WriteAllLines(path, new string[] { string.Empty });

			using (FileStream file = File.OpenWrite(path))
			{
				TextWriter stream = new StreamWriter(file, Encoding.ASCII);
				new ProjectExporter(stream, includes, includesWLabels);

				stream.Flush();
				stream.Close();
			}
		}
	}
}
