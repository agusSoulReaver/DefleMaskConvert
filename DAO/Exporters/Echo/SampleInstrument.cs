﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.Exporters.Echo
{
	public class SampleInstrument : Instrument
	{
		public SampleInstrument(string name)
			: base(name)
		{
		}

		public byte[] Data { get; set; }

		public override byte[] GetBinaryData()
		{
			return Data;
		}
	}
}
