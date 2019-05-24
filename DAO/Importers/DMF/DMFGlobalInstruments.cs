using DefleMaskConvert.DAO.DefleMask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.Importers.DMF
{
	public class DMFGlobalInstruments
	{
		static private List<InstrumentData> _instruments = new List<InstrumentData>();

		static public T TrySaveInstrument<T>(T instrument)
			where T : InstrumentData
		{
			foreach(var saveInstrument in _instruments)
			{
				if (instrument == saveInstrument)
					return (T)saveInstrument;
			}

			foreach (var saveInstrument in _instruments)
			{
				if (instrument.Name == saveInstrument.Name)
				{
					instrument.Name = GetNextDefaultName(instrument.Name);
					break;
				}
			}

			_instruments.Add(instrument);
			return instrument;
		}

		static public string GetNextDefaultName(string prefix)
		{
			return EntryPoint.GetNextDefaultName(from item in _instruments select item.Name, prefix);
		}

		private const int DAC_CHANNEL = 5;
		static private readonly List<InstrumentData> _activeInstruments = new List<InstrumentData>();
		static public List<InstrumentData> GetActiveInstruments(List<DMFData> songs, List<SFXData> sfxs)
		{
			_activeInstruments.Clear();

			ExtractInstruments(songs);
			foreach(var fxs in sfxs)
			{
				if (fxs.Export)
					ExtractInstruments(fxs.FXs);
			}

			_activeInstruments.Add(PSGInstrumentData.DEFAULT_PSG_INSTRUMENT);
			_activeInstruments.Sort(SortInstruments);

			return _activeInstruments;
		}

		static private void ExtractInstruments(List<DMFData> data)
		{
			foreach (var dmf in data)
			{
				if (!dmf.Export) continue;

				bool DACEnabled = false;
				for (int patternIndex = 0; patternIndex < dmf.PatternPages; patternIndex++)
				{
					for (int row = 0; row < dmf.TotalRowsPerPattern; row++)
					{
						for (int channelIndex = 0; channelIndex < dmf.Channels.Count; channelIndex++)
						{
							NoteData noteData = dmf.Channels[channelIndex].Pages[patternIndex].Notes[row];
							for (int effectCounter = 0; effectCounter < noteData.Effects.Length; effectCounter++)
							{
								EffectType effectType = (EffectType)noteData.Effects[effectCounter].Type;
								byte effectParam = (byte)noteData.Effects[effectCounter].Value;
								if (effectType == EffectType.DACOn)
								{
									DACEnabled = effectParam > 0;
									break;
								}
							}

							if (channelIndex == DAC_CHANNEL && DACEnabled)
							{
								byte note = (byte)noteData.Note;
								if (note != (byte)Notes.Off)
								{
									//Notes were 1-based, now 0-based from here
									if (note == Constants.LAST_NOTE) note = 0;

									SampleData sample = dmf.Samples[note];
									if (!_activeInstruments.Contains(sample))
										_activeInstruments.Add(sample);
								}
							}
							else if ((byte)noteData.Instrument != 0xFF)
							{
								InstrumentData instrument = dmf.Instruments[noteData.Instrument];
								if (!_activeInstruments.Contains(instrument) && (!(instrument is PSGInstrumentData) || ((PSGInstrumentData)instrument != PSGInstrumentData.DEFAULT_PSG_INSTRUMENT)))
									_activeInstruments.Add(instrument);
							}
						}
					}
				}
			}
		}

		static private int SortInstruments(InstrumentData a, InstrumentData b)
		{
			if (a is FMInstrumentData && !(b is FMInstrumentData)) return -1;

			if (a is PSGInstrumentData && !(b is PSGInstrumentData)) return b is FMInstrumentData ? 1 : -1;

			if (a is SampleData && !(b is SampleData)) return 1;

			return 0;
		}
	}
}
