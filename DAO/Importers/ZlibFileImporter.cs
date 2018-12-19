using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.Importers
{
	abstract public class ZlibFileImporter
	{
		private byte[] _uncompress;
		private int _index;
		private StringBuilder _textBuilder = new StringBuilder(byte.MaxValue);

		public ZlibFileImporter(string path)
		{
			var file = File.ReadAllBytes(path);
			_uncompress = ZlibStream.UncompressBuffer(file);
			Reset();
		}

		protected void Reset()
		{
			_index = 0;
		}

		protected string ReadString()
		{
			return ReadString(ReadNumber8());
		}

		protected string ReadString(byte length)
		{
			_textBuilder.Clear();

			for (byte i = 0; i < length; i++)
			{
				_textBuilder.Append((char)ReadNumber8());
			}

			return _textBuilder.ToString();
		}

		protected byte ReadNumber8()
		{
			return _uncompress[_index++];
		}

		protected ushort ReadNumber16()
		{
			byte lower = ReadNumber8();
			byte upper = ReadNumber8();
			return (ushort)(upper << 8 | lower);
		}

		protected uint ReadNumber32()
		{
			ushort lower = ReadNumber16();
			ushort upper = ReadNumber16();
			return (uint)(upper << 16 | lower);
		}

		protected void ApplyOffset(int bytesCount)
		{
			_index += bytesCount;
		}
	}
}
