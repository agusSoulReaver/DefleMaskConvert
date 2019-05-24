using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.DefleMask
{
	public class DMFData
	{
		public enum FrameModes
		{
			PAL = 0,
			NTSC,
			Custom
		}

		public string FilePath { get; set; }
		public string ExportName { get; set; }
		public bool Export { get; set; }
		public bool LockChannels { get; set; }
		public bool LoopWholeTrack { get; set; }
		public ESF_PCMRate PCMRate { get; set; }

		public string SongName { get; set; }
		public string SongAuthor { get; set; }
		public byte FileVersion { get; set; }
		public DefleMaskConvert.Constants.Systems System { get; set; }

		public byte HighlightA { get; set; }
		public byte HighlightB { get; set; }

		public byte TimeBase { get; set; }
		public byte TickTimeEven { get; set; }
		public byte TickTimeOdd { get; set; }

		public FrameModes FrameMode { get; set; }
		public byte CustomHz1 { get; set; }
		public byte CustomHz2 { get; set; }
		public byte CustomHz3 { get; set; }

		public uint TotalRowsPerPattern { get; set; }
		public byte PatternPages { get; set; }

		public byte[][] PatternMatrix { get; set; }

		public List<InstrumentData> Instruments { get; set; }
		public List<WaveTableData> WaveTables { get; set; }
		public List<ChannelData> Channels { get; set; }
		public List<SampleData> Samples { get; set; }

		public DMFData()
		{
			Export = true;
			PCMRate = ESF_PCMRate.NotChange;
		}

		public DMFData(DMFData other, int pageIndex)
			: this()
		{
			this.FilePath = "";
			this.LockChannels = true;

			this.HighlightA = other.HighlightA;
			this.HighlightB = other.HighlightB;

			this.TimeBase = other.TimeBase;
			this.TickTimeEven = other.TickTimeEven;
			this.TickTimeOdd = other.TickTimeOdd;

			this.FrameMode = other.FrameMode;
			this.CustomHz1 = other.CustomHz1;
			this.CustomHz2 = other.CustomHz2;
			this.CustomHz3 = other.CustomHz3;

			this.PatternPages = 1;

			this.Instruments = other.Instruments;
			this.WaveTables = other.WaveTables;
			this.Samples = other.Samples;

			bool findName = true;
			this.Channels = new List<ChannelData>();
			for (int i = 0; i < other.Channels.Count; i++)
			{
				var otherChannel = other.Channels[i];
				var channel = new ChannelData();
				channel.EffectsCount = otherChannel.EffectsCount;
				this.Channels.Add(channel);

				channel.Pages = new PatternPage[1];
				channel.Pages[0] = otherChannel.Pages[pageIndex];

				if(findName)
				{
					foreach(var note in channel.Pages[0].Notes)
					{
						if (note.Instrument >= 0)
						{
							var instrument = this.Instruments[note.Instrument];
							this.ExportName = instrument.Name;
							findName = false;
							break;
						}
					}
				}
			}

			uint totalRows = 0;
			for (int i = 0; i < this.Channels.Count; i++)
			{
				var channel = this.Channels[i];
				for (int noteIndex = channel.Pages[0].Notes.Length; --noteIndex >= 0; )
				{
					var note = channel.Pages[0].Notes[noteIndex];
					if (note.Note == (ushort)Notes.Off)
					{
						totalRows = (uint)noteIndex+1;
						break;
					}
				}
			}

			this.TotalRowsPerPattern = totalRows > 0? totalRows : other.TotalRowsPerPattern;
		}

		public bool IsUsingPSGNoiseFrequency()
		{
			for (int patternIndex = 0; patternIndex < PatternPages; patternIndex++)
			{
				for (int row = 0; row < TotalRowsPerPattern; row++)
				{
					for (int channelIndex = 0; channelIndex < Channels.Count; channelIndex++)
					{
						NoteData noteData = Channels[channelIndex].Pages[patternIndex].Notes[row];
						for (int effectCounter = 0; effectCounter < noteData.Effects.Length; effectCounter++)
						{
							EffectType effectType = (EffectType)noteData.Effects[effectCounter].Type;
							byte effectParam = (byte)noteData.Effects[effectCounter].Value;
							if (effectType == EffectType.PSGNoise && (effectParam & 0xF0) != 0)
								return true;
						}
					}
				}
			}

			return false;
		}

		public bool IsLoopJumpSet()
		{
			for (int patternIndex = 0; patternIndex < PatternPages; patternIndex++)
			{
				for (int row = 0; row < TotalRowsPerPattern; row++)
				{
					for (int channelIndex = 0; channelIndex < Channels.Count; channelIndex++)
					{
						NoteData noteData = Channels[channelIndex].Pages[patternIndex].Notes[row];
						for (int effectCounter = 0; effectCounter < noteData.Effects.Length; effectCounter++)
						{
							EffectType effectType = (EffectType)noteData.Effects[effectCounter].Type;
							if (effectType == EffectType.Jump)
								return true;
						}
					}
				}
			}

			return false;
		}

		public List<UnsupportedEffect> GetUnsupportedEffects()
		{
			List<UnsupportedEffect> result = new List<UnsupportedEffect>();
			var unsupported = Enum.GetValues(typeof(UnsupportedEffect)).Cast<UnsupportedEffect>();

			for (int patternIndex = 0; patternIndex < PatternPages; patternIndex++)
			{
				for (int row = 0; row < TotalRowsPerPattern; row++)
				{
					for (int channelIndex = 0; channelIndex < Channels.Count; channelIndex++)
					{
						NoteData noteData = Channels[channelIndex].Pages[patternIndex].Notes[row];
						for (int effectCounter = 0; effectCounter < noteData.Effects.Length; effectCounter++)
						{
							UnsupportedEffect effectType = (UnsupportedEffect)noteData.Effects[effectCounter].Type;
							if (effectType != UnsupportedEffect.None && !result.Contains(effectType) && unsupported.Contains<UnsupportedEffect>(effectType))
								result.Add(effectType);
						}
					}
				}
			}

			foreach(var sample in Samples)
			{
				if (sample.Pitch != 5 && !result.Contains(UnsupportedEffect.SamplePitch))
					result.Add(UnsupportedEffect.SamplePitch);

				if (sample.Amp != 50 && !result.Contains(UnsupportedEffect.SampleAmp))
					result.Add(UnsupportedEffect.SampleAmp);
			}

			return result;
		}
	}
}
