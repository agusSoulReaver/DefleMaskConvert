using DefleMaskConvert.DAO.Exporters.Echo;
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
		private const string LOCAL_LABEL = "@{0}";
		private const string LABEL_MUSIC = "Jukebox_Songs";
		private const string LABEL_SFX = "Jukebox_SFXs";

		private Moto68KWriter _writer;

		private ProjectExporter(TextWriter stream, Dictionary<string, string> songs, List<EchoESF> sfxs)
		{
			_writer = new Moto68KWriter(stream);

			_writer.DefineConstant((ushort)0);
			_writer.Label(LABEL_MUSIC);
			int index = -1;
			foreach (var pair in songs)
			{
				index++;
				//if(index == songs.Count-1)
				//	_writer.Label(LABEL_MUSIC+"Last");

				_writer.DefineConstantHeader(Moto68KWriter.Sizes.Word);
				_writer.Text("(");
				_writer.Text(string.Format(LOCAL_LABEL, pair.Key));
				_writer.Text("-");
				_writer.Text(LABEL_MUSIC);
				_writer.Text("-");
				_writer.Number(2+(index*2));
				_writer.Text(")");
				_writer.NewLine();
			}
			_writer.DefineConstant((ushort)0);
			_writer.NewLine();

			foreach (var pair in songs)
			{
				_writer.Label(string.Format(LOCAL_LABEL, pair.Key));
				_writer.DefineConstant(pair.Key, Moto68KWriter.Sizes.LongWord);
				_writer.DefineText(pair.Value);
			}
			_writer.NewLine();

			index = -1;
			_writer.DefineConstant((ushort)0);
			_writer.Label(LABEL_SFX);
			foreach (var item in sfxs)
			{
				index++;
				//if (index == sfxs.Count - 1)
				//	_writer.Label(LABEL_SFX + "Last");

				_writer.DefineConstantHeader(Moto68KWriter.Sizes.Word);
				_writer.Text("(");
				_writer.Text(string.Format(LOCAL_LABEL, item.ExportName));
				_writer.Text("-");
				_writer.Text(LABEL_SFX);
				_writer.Text("-");
				_writer.Number(2 + (index * 2));
				_writer.Text(")");
				_writer.NewLine();
			}
			_writer.DefineConstant((ushort)0);
			_writer.NewLine();

			foreach (var item in sfxs)
			{
				_writer.Label(string.Format(LOCAL_LABEL, item.ExportName));
				_writer.DefineConstant(item.ExportName, Moto68KWriter.Sizes.LongWord);
				_writer.DefineText(item.ExportName);
			}
		}

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

		static public void SaveIncludes(string path, List<string> includes, List<KeyValuePair<string, string>> includesWLabels)
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

		static public void SaveJukebox(string path, Dictionary<string, string> songs, List<EchoESF> sfxs)
		{
			File.WriteAllLines(path, new string[] { string.Empty });

			using (FileStream file = File.OpenWrite(path))
			{
				TextWriter stream = new StreamWriter(file, Encoding.ASCII);
				new ProjectExporter(stream, songs, sfxs);

				stream.Flush();
				stream.Close();
			}
		}
	}
}
