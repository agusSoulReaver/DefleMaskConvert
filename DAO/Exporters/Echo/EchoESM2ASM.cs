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

		private EchoESM2ASM(TextWriter stream)
		{
			_writer = new Moto68KWriter(stream);
		}

		private EchoESM2ASM(TextWriter stream, EchoESF data, string songName, string author)
			:this(stream)
		{
			_writer.Comment(string.Format("Name: {0}", songName));
			_writer.Comment(string.Format("Author: {0}", author));
			_writer.Comment(string.Format("Size: {0} bytes", CalculateSize(data)));
			_writer.NewLine();

			Write(data);
		}

		private EchoESM2ASM(TextWriter stream, List<EchoESF> list, bool exportChangeBitRate)
			: this(stream)
		{
			_writer.Comment(string.Format("Size: {0} bytes", CalculateSize(list)));
			_writer.Comment("Labels:");
			foreach(var data in list)
			{
				_writer.Comment(data.ExportName, Moto68KWriter.Separations.Tab);
			}

			foreach (var data in list)
			{
				_writer.NewLine();
				_writer.Label(data.ExportName);
				if (exportChangeBitRate && data.BitRate != ESF_PCMRate.NotChange) WriteChangeBitRate(data.BitRate);
				Write(data);
			}
		}

		private void Write(EchoESF data)
		{
			if (data.Header.Count > 0)
			{
				_writer.Tab();
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
					_writer.Tab();
					_writer.Comment(string.Format("Page {0} Row {1}", page.Index.ToString("X").PadLeft(2, '0'), row.Index.ToString().PadLeft(3, '0')));
					foreach (var eventData in row.Events)
					{
						WriteEvent(eventData);
					}
				}
				_writer.NewLine();
			}

			if (data.Footer.Count > 0)
			{
				_writer.Tab();
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

		private void WriteChangeBitRate(ESF_PCMRate data)
		{
			_writer.DefineConstantHeader(Moto68KWriter.Sizes.Byte);
			_writer.Number((byte)0x00, Moto68KWriter.Formats.Hexa);
			_writer.Text(",");
			_writer.Number((byte)data, Moto68KWriter.Formats.Hexa);
			_writer.Tab();
			_writer.Tab();
			_writer.Comment(string.Format("Change BitRate to {0}", data));
		}

		private int CalculateSize(List<EchoESF> list)
		{
			int size = 0;
			foreach (var data in list)
			{
				size += CalculateSize(data);
			}
			return size;
		}

		private int CalculateSize(EchoESF data)
		{
			int size = CalculateSize(data.Header);
			foreach (var page in data.Pages)
			{
				foreach (var row in page.Rows)
				{
					size += CalculateSize(row.Events);
				}
			}
			size += CalculateSize(data.Footer);
			return size;
		}

		private int CalculateSize(List<IEchoEvent> events)
		{
			int size = 0;

			foreach(var item in events)
			{
				byte[] data = item.GetBinaryData();
				size += data.Length;
			}

			return size;
		}

		static public void SaveFile(string path, EchoESF data, string songName, string author)
		{
			File.WriteAllLines(path, new string[] { string.Empty });

			using (FileStream file = File.OpenWrite(path))
			{
				TextWriter stream = new StreamWriter(file, Encoding.ASCII);
				new EchoESM2ASM(stream, data, songName, author);

				stream.Flush();
				stream.Close();
			}
		}

		static public void SaveFile(string path, List<EchoESF> data, bool exportChangeBitRate)
		{
			File.WriteAllLines(path, new string[] { string.Empty });

			using (FileStream file = File.OpenWrite(path))
			{
				TextWriter stream = new StreamWriter(file, Encoding.ASCII);
				new EchoESM2ASM(stream, data, exportChangeBitRate);

				stream.Flush();
				stream.Close();
			}
		}
	}
}
