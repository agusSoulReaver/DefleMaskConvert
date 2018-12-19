using DefleMaskConvert.DAO.Exporters.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.Exporters.Echo
{
	public class EchoESM2ASM
	{
		private Moto68KWriter _writer;
		private uint _size;

		private EchoESM2ASM(TextWriter stream, EchoESF data)
		{
			_writer = new Moto68KWriter(stream);
			_size = 0;

			if (data.Header.Count > 0)
			{
				_writer.Comment("Header");
				foreach(var eventData in data.Header)
				{
					WriteEvent(eventData);
				}
			}

			foreach (var page in data.Pages)
			{
				foreach (var row in page.Rows)
				{
					_writer.Comment(string.Format("Page {0} Row {1}", page.Index, row.Index));
					foreach (var eventData in row.Events)
					{
						WriteEvent(eventData);
					}
				}
				_writer.NewLine();
			}

			if (data.Footer.Count > 0)
			{
				_writer.Comment("Footer");
				foreach (var eventData in data.Footer)
				{
					WriteEvent(eventData);
				}
			}

			_writer.NewLine();
		}

		private void WriteEvent(IEchoEvent eventData)
		{
			byte[] data = eventData.GetBinaryData();
			_size += (uint)data.Length;
			_writer.DefineConstantHeader(Moto68KWriter.Sizes.Byte);

			for (int i = 0; i < data.Length; i++)
			{
				if (i > 0) _writer.Text(",");
				_writer.Number(data[i], Moto68KWriter.Formats.Hexa);
			}

			int tabAmount;
			string comment = eventData.GetComment(out tabAmount);
			if (!_writer.IgnoreComments && comment != null)
			{
				for (int i = 0; i < tabAmount; i++) _writer.Tab();
				_writer.Comment(comment);
			}
			else
				_writer.NewLine();
		}

		static public void SaveFile(string path, EchoESF data)
		{
			File.WriteAllLines(path, new string[] { string.Empty });

			using (FileStream file = File.OpenWrite(path))
			{
				TextWriter stream = new StreamWriter(file, Encoding.ASCII);
				new EchoESM2ASM(stream, data);

				stream.Flush();
				stream.Close();
			}
		}
	}
}
