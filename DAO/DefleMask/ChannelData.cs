using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.DefleMask
{
	public class ChannelData
	{
		public byte EffectsCount { get; set; }
		public PatternPage[] Pages { get; set; }

		public bool Export { get; set; }

		public ChannelData()
		{
			Export = true;
		}
	}

	public class PatternPage
	{
		public NoteData[] Notes { get; set; }
	}

	public class NoteData
	{
		public ushort Note { get; set; }
		public ushort Octave { get; set; }
		public short Volume { get; set; }
		public NoteEffect[] Effects { get; set; }
		public short Instrument { get; set; }

		public bool IsEmpty { get { return Note == 0 && Octave == 0; } }
		public bool IsOff { get { return Note == 100; } }
	}

	public class NoteEffect
	{
		public short Type { get; set; }
		public short Value { get; set; }
	}
}
