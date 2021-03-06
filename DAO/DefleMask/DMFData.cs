﻿using System;
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
