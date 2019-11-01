using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DefleMaskConvert.DAO.Exporters.Utils
{
	public class Moto68KWriter
	{
		public enum Sizes
		{
			Byte = 1,
			Word = 2,
			LongWord = 4
		}

		public enum Separations
		{
			Space, Tab
		}

		public enum Formats
		{
			Decimal, Hexa, Binary
		}

		private static int _totalBytes = 0;
		public static int TotalBytes { get { return _totalBytes; } }

		private static bool _calculateBytes = false;
		public static bool CalcualteBytes
		{
			get { return _calculateBytes; }
			set
			{
				if (value == _calculateBytes)
					return;

				_calculateBytes = value;
				_totalBytes = 0;
			}
		}

		public void IncreaseTotalBytes(Sizes size)
		{
			if(_calculateBytes)
				_totalBytes += (int)size;
		}

		private TextWriter _writer;
		public bool IgnoreComments = false;

		public Moto68KWriter(TextWriter writer)
		{
			_writer = writer;
		}

		private char GetSizeChar(Sizes size)
		{
			switch(size)
			{
				case Sizes.Byte:
					return 'b';
				case Sizes.Word:
					return 'w';
				case Sizes.LongWord:
					return 'l';
			}

			throw new NotImplementedException("The size \"" + size + "\" is not implemented!");
		}

		private int GetSizeBytes(Sizes size)
		{
			return (int)size;
		}

		private char GetSeparationChar(Separations type)
		{
			switch (type)
			{
				case Separations.Space:
					return ' ';
				case Separations.Tab:
					return '\t';
			}

			throw new NotImplementedException("The separation \"" + type + "\" is not implemented!");
		}

		public void Text(string text)
		{
			_writer.Write(text);
		}

		public void NewLine()
		{
			_writer.WriteLine();
		}

		public void Tab()
		{
			_writer.Write(GetSeparationChar(Separations.Tab));
		}

		public void Label(string name, bool useColon=true)
		{
			_writer.Write(name);

			if(useColon)
				_writer.Write(':');

			NewLine();
		}

		public void EchoListEntry(string instrumentLabel)
		{
			Tab();
			_writer.Write("Echo_ListEntry ");
			_writer.Write(instrumentLabel);
			NewLine();
		}

		public void EchoListEnd()
		{
			Tab();
			_writer.Write("Echo_ListEnd");
			NewLine();
		}

		public void Even()
		{
			Even(true);
		}

		private void Even(bool full)
		{
			if(full) Tab();
			_writer.Write("even");

			if (full)
			{
				Comment("Command to not break word-size boundaries");
				NewLine();
			}
		}

		public void Include(string path, Separations separation = Separations.Space)
		{
			Tab();
			_writer.Write("include");
			_writer.Write(GetSeparationChar(separation));
			_writer.Write("\"");
			_writer.Write(path);
			_writer.Write("\"");
			NewLine();
		}

		public void DefineText(string text, string comment = null)
		{
			DefineConstantHeader(Sizes.Byte);
			_writer.Write("\"");
			Text(text);
			_writer.Write("\", 0");
			DefineConstantTail(comment);
			Even();
		}

		public void Comment(string text, Separations separation = Separations.Space)
		{
			if (!IgnoreComments)
			{
				string aux = ";" + GetSeparationChar(separation);
				_writer.Write(aux);

				aux = aux + Environment.NewLine;
				text.Replace("\n", aux);
				text.Replace("\r", aux);

				_writer.Write(text);

				NewLine();
			}
		}

		#region ConstantReference
		public void ConstantReference(string name, object value, string comment=null)
		{
			ConstantReferenceHeader(name);
			_writer.Write(value);
			ConstantReferenceTail(comment);
		}

		private void ConstantReferenceHeader(string name)
		{
			_writer.Write(name);
			_writer.Write(": equ ");
		}

		private void ConstantReferenceTail(string comment)
		{
			if (comment != null && !IgnoreComments)
				Comment(comment);
			else
				NewLine();
		}

		public void ConstantReference(string name, byte value, string comment = null)
		{
			ConstantReference(name, value, Formats.Decimal, comment);
		}

		public void ConstantReference(string name, byte value, Formats format, string comment = null)
		{
			ConstantReferenceHeader(name);
			Number(value, format, false);
			ConstantReferenceTail(comment);
		}

		public void ConstantReference(string name, sbyte value, string comment = null)
		{
			ConstantReference(name, value, Formats.Decimal, comment);
		}

		public void ConstantReference(string name, sbyte value, Formats format, string comment = null)
		{
			ConstantReferenceHeader(name);
			Number(value, format, false);
			ConstantReferenceTail(comment);
		}

		public void ConstantReference(string name, short value, string comment = null)
		{
			ConstantReference(name, value, Formats.Decimal, comment);
		}

		public void ConstantReference(string name, short value, Formats format, string comment = null)
		{
			ConstantReferenceHeader(name);
			Number(value, format, false);
			ConstantReferenceTail(comment);
		}

		public void ConstantReference(string name, ushort value, string comment = null)
		{
			ConstantReference(name, value, Formats.Decimal, comment);
		}

		public void ConstantReference(string name, ushort value, Formats format, string comment = null)
		{
			ConstantReferenceHeader(name);
			Number(value, format, false);
			ConstantReferenceTail(comment);
		}

		public void ConstantReference(string name, int value, string comment = null)
		{
			ConstantReference(name, value, Formats.Decimal, comment);
		}

		public void ConstantReference(string name, int value, Formats format, string comment = null)
		{
			ConstantReferenceHeader(name);
			Number(value, format, false);
			ConstantReferenceTail(comment);
		}

		public void ConstantReference(string name, uint value, string comment = null)
		{
			ConstantReference(name, value, Formats.Decimal, comment);
		}

		public void ConstantReference(string name, uint value, Formats format, string comment = null)
		{
			ConstantReferenceHeader(name);
			Number(value, format, false);
			ConstantReferenceTail(comment);
		}
		#endregion

		#region Define Constant
		public void DefineConstant(byte value, string comment = null)
		{
			DefineConstant(value, Formats.Decimal, comment);
		}

		public void DefineConstant(byte value, Formats format, string comment=null)
		{
			DefineConstantHeader(Sizes.Byte);
			Number(value, format);
			DefineConstantTail(comment);
		}

		public void DefineConstant(sbyte value, string comment = null)
		{
			DefineConstant(value, Formats.Decimal, comment);
		}

		public void DefineConstant(sbyte value, Formats format, string comment = null)
		{
			DefineConstantHeader(Sizes.Byte);
			Number(value, format);
			DefineConstantTail(comment);
		}

		public void DefineConstant(short value, string comment = null)
		{
			DefineConstant(value, Formats.Decimal, comment);
		}

		public void DefineConstant(short value, Formats format, string comment = null)
		{
			DefineConstantHeader(Sizes.Word);
			Number(value, format);
			DefineConstantTail(comment);
		}

		public void DefineConstant(ushort value, string comment = null)
		{
			DefineConstant(value, Formats.Decimal, comment);
		}

		public void DefineConstant(ushort value, Formats format, string comment = null)
		{
			DefineConstantHeader(Sizes.Word);
			Number(value, format);
			DefineConstantTail(comment);
		}

		public void DefineConstant(int value, string comment = null)
		{
			DefineConstant(value, Formats.Decimal, comment);
		}

		public void DefineConstant(int value, Formats format, string comment = null)
		{
			DefineConstantHeader(Sizes.LongWord);
			Number(value, format);
			DefineConstantTail(comment);
		}

		public void DefineConstant(uint value, string comment = null)
		{
			DefineConstant(value, Formats.Decimal, comment);
		}

		public void DefineConstant(uint value, Formats format, string comment = null)
		{
			DefineConstantHeader(Sizes.LongWord);
			Number(value, format);
			DefineConstantTail(comment);
		}

		public void DefineConstant(string label, Sizes size, string comment = null)
		{
			DefineConstantHeader(size);
			Text(label);
			DefineConstantTail(comment);
		}

		public void DefineConstantHeader(Sizes size, Separations separation = Separations.Space)
		{
			DefineConstantHeader(size, false, separation);
		}

		public void DefineConstantHeader(Sizes size, bool increaseTotalBytes, Separations separation = Separations.Space)
		{
			Tab();

			_writer.Write("dc.");
			_writer.Write(GetSizeChar(size));
			_writer.Write(GetSeparationChar(separation));

			if (_calculateBytes && increaseTotalBytes)
				_totalBytes += GetSizeBytes(size);
		}

		private void DefineConstantTail(string comment = null)
		{
			if (comment != null && !IgnoreComments)
			{
				_writer.Write(' ');
				Comment(comment);
			}
			else
				NewLine();
		}
		#endregion

		#region Write Number
		public void Number(byte value, Formats format=Formats.Decimal)
		{
			Number(value, format, _calculateBytes);
		}

		private void Number(byte value, Formats format, bool calculateBytes)
		{
			string result;
			switch (format)
			{
				case Formats.Hexa:
					result = string.Format("0x{0}", value.ToString("X").PadLeft(2,'0'));
					break;

				case Formats.Binary:
					string binaryString = Convert.ToString(value, 2);
					int zeroCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(binaryString.Length) / 8)) * 8;
					result = string.Format("%{0}", binaryString.PadLeft(zeroCount, '0'));
					break;

				case Formats.Decimal:
					result = value.ToString();
					break;

				default:
					throw new NotImplementedException("The format \"" + format + "\" is not implemented!");
			}

			_writer.Write(result);
			if (calculateBytes)
				_totalBytes += GetSizeBytes(Sizes.Byte);
		}

		public void Number(sbyte value, Formats format = Formats.Decimal)
		{
			Number(value, format, _calculateBytes);
		}

		private void Number(sbyte value, Formats format, bool calculateBytes)
		{
			string result;
			switch (format)
			{
				case Formats.Hexa:
					result = string.Format("0x{0}", value.ToString("x").PadLeft(2, '0'));
					break;

				case Formats.Binary:
					string binaryString = Convert.ToString(value, 2);
					int zeroCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(binaryString.Length) / 8)) * 8;
					result = string.Format("%{0}", binaryString.PadLeft(zeroCount, '0'));
					break;

				case Formats.Decimal:
					result = value.ToString();
					break;

				default:
					throw new NotImplementedException("The format \"" + format + "\" is not implemented!");
			}

			_writer.Write(result);
			if (calculateBytes)
				_totalBytes += GetSizeBytes(Sizes.Byte);
		}

		public void Number(short value, Formats format = Formats.Decimal)
		{
			Number(value, format, _calculateBytes);
		}

		private void Number(short value, Formats format, bool calculateBytes)
		{
			string result;
			switch (format)
			{
				case Formats.Hexa:
					result = string.Format("0x{0}", value.ToString("x").PadLeft(4, '0'));
					break;

				case Formats.Binary:
					string binaryString = Convert.ToString(value, 2);
					int zeroCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(binaryString.Length) / 8)) * 8;
					result = string.Format("%{0}", binaryString.PadLeft(zeroCount, '0'));
					break;

				case Formats.Decimal:
					result = value.ToString();
					break;

				default:
					throw new NotImplementedException("The format \"" + format + "\" is not implemented!");
			}

			_writer.Write(result);
			if (calculateBytes)
				_totalBytes += GetSizeBytes(Sizes.Word);
		}

		public void Number(ushort value, Formats format = Formats.Decimal)
		{
			Number(value, format, _calculateBytes);
		}

		private void Number(ushort value, Formats format, bool calculateBytes)
		{
			string result;
			switch (format)
			{
				case Formats.Hexa:
					result = string.Format("0x{0}", value.ToString("x").PadLeft(4, '0'));
					break;

				case Formats.Binary:
					string binaryString = Convert.ToString(value, 2);
					int zeroCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(binaryString.Length) / 8)) * 8;
					result = string.Format("%{0}", binaryString.PadLeft(zeroCount, '0'));
					break;

				case Formats.Decimal:
					result = value.ToString();
					break;

				default:
					throw new NotImplementedException("The format \"" + format + "\" is not implemented!");
			}

			_writer.Write(result);
			if (calculateBytes)
				_totalBytes += GetSizeBytes(Sizes.Word);
		}

		public void Number(int value, Formats format = Formats.Decimal)
		{
			Number(value, format, _calculateBytes);
		}

		private void Number(int value, Formats format, bool calculateBytes)
		{
			string result;
			switch (format)
			{
				case Formats.Hexa:
					result = string.Format("0x{0}", value.ToString("x").PadLeft(8, '0'));
					break;

				case Formats.Binary:
					string binaryString = Convert.ToString(value, 2);
					int zeroCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(binaryString.Length) / 8)) * 8;
					result = string.Format("%{0}", binaryString.PadLeft(zeroCount, '0'));
					break;

				case Formats.Decimal:
					result = value.ToString();
					break;

				default:
					throw new NotImplementedException("The format \"" + format + "\" is not implemented!");
			}

			_writer.Write(result);
			if (calculateBytes)
				_totalBytes += GetSizeBytes(Sizes.LongWord);
		}

		public void Number(uint value, Formats format = Formats.Decimal)
		{
			Number(value, format, _calculateBytes);
		}

		private void Number(uint value, Formats format, bool calculateBytes)
		{
			string result;
			switch (format)
			{
				case Formats.Hexa:
					result = string.Format("0x{0}", value.ToString("x").PadLeft(8, '0'));
					break;

				case Formats.Binary:
					string binaryString = Convert.ToString(value, 2);
					int zeroCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(binaryString.Length) / 8)) * 8;
					result = string.Format("%{0}", binaryString.PadLeft(zeroCount, '0'));
					break;

				case Formats.Decimal:
					result = value.ToString();
					break;

				default:
					throw new NotImplementedException("The format \"" + format + "\" is not implemented!");
			}

			_writer.Write(result);
			if (calculateBytes)
				_totalBytes += GetSizeBytes(Sizes.LongWord);
		}
		#endregion

	}
}
