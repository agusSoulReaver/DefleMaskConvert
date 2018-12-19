using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.Exporters.Echo
{

	public class EchoESF
	{
		public readonly List<IEchoEvent> Header = new List<IEchoEvent>();
		public readonly List<EchoPatternPage> Pages = new List<EchoPatternPage>();
		public readonly List<IEchoEvent> Footer = new List<IEchoEvent>();

		static private List<byte> _binary = new List<byte>();
		public byte[] GetBinaryData()
		{
			_binary.Clear();

			foreach(var data in Header)
			{
				_binary.AddRange(data.GetBinaryData());
			}

			foreach (var page in Pages)
			{
				foreach (var row in page.Rows)
				{
					foreach (var data in row.Events)
					{
						_binary.AddRange(data.GetBinaryData());
					}
				}
			}

			foreach (var data in Footer)
			{
				_binary.AddRange(data.GetBinaryData());
			}

			return _binary.ToArray();
		}
	}

	public class EchoPatternPage
	{
		public readonly int Index;
		public readonly List<EchoPatternRow> Rows;

		public EchoPatternPage(int index)
		{
			Index = index;
			Rows = new List<EchoPatternRow>();
		}
	}

	public class EchoPatternRow
	{
		public readonly int Index;
		public readonly List<IEchoEvent> Events;

		public EchoPatternRow(int index)
		{
			Index = index;
			Events = new List<IEchoEvent>();
		}
	}
}
