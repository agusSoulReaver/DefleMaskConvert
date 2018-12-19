using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.Exporters.Echo
{
	abstract public class Instrument
	{
		public string Name { get; private set; }

		public Instrument(string name)
		{
			this.Name = name;
		}

		abstract public byte[] GetBinaryData();
	}
}
