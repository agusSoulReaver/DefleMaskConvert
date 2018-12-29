﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefleMaskConvert.DAO.Exporters.Echo
{
	public class ESFOptimizer
	{
		static public void Optimize(EchoESF data)
		{
			TryMergeTimers(data.Pages);
			TryDistributeEvents(data);
		}

		static private void TryDistributeEvents(EchoESF data)
		{
			List<IEchoChannelEvent> loopEvents = null;	
			var firstEvents = data.Pages[0].Rows[0].Events;

			RefreshRelevantEventIndexes(firstEvents);
			if (_indexes.Count > 0)
			{
				data.Header.Insert(0, new DelayEvent(1, true));
				loopEvents = new List<IEchoChannelEvent>();

				for (int i = _indexes.Count; --i >= 0; )
				{
					var channelEvent = (IEchoChannelEvent)firstEvents[_indexes[i]];
					loopEvents.Insert(0, channelEvent);
					firstEvents.RemoveAt(_indexes[i]);
					data.Header.Insert(0, channelEvent);
				}
			}

			EchoPatternPage startLoopPage = ContaineStartLoop(data.Header) ? data.Pages[0] : null;
			if (startLoopPage == null) loopEvents = null;

			for (int pageIndex = 0; pageIndex < data.Pages.Count; pageIndex++)
			{
				var page = data.Pages[pageIndex];

				for (int rowIndex = 0; rowIndex < page.Rows.Count; rowIndex++)
				{
					if (page == startLoopPage && rowIndex == 0) continue;

					var row = page.Rows[rowIndex];
					if (startLoopPage == null && ContaineStartLoop(row.Events))
					{
						startLoopPage = page;
						RefreshRelevantEventIndexes(row.Events);
						if (_indexes.Count > 0)
						{
							loopEvents = new List<IEchoChannelEvent>();
							for (int i = _indexes.Count; --i >= 0; )
							{
								var channelEvent = (IEchoChannelEvent)row.Events[_indexes[i]];
								loopEvents.Insert(0, channelEvent);
							}
						}
					}
					else
					{
						if (row.Events.Count <= 0) continue;
						RefreshRelevantEventIndexes(row.Events);
					}

					if (_indexes.Count <= 0) continue;
					TryDistributeEvents(data, row.Events, pageIndex, rowIndex);
				}
			}

			if (loopEvents != null)
				TryDistributeLoopEvents(data, loopEvents);
		}

		static private void TryDistributeEvents(EchoESF data, List<IEchoEvent> events, int startPage, int startRow)
		{
			EchoPatternPage page;
			EchoPatternRow row;
			List<IEchoEvent> saveEvents;
			int saveIndex;

			while (_indexes.Count > 0)
			{
				bool completed = false;
				for (int i = _indexes.Count; --i >= 0 && !completed; )
				{
					int index = _indexes[i];
					if (index >= events.Count || !(events[index] is SetInstrumentEvent)) continue;

					SetInstrumentEvent setInstrument = (SetInstrumentEvent)events[index];
					_indexes.RemoveAt(i);

					for (int pageIndex = startPage + 1; --pageIndex >= 0 && !completed; )
					{
						page = data.Pages[pageIndex];

						for (int rowIndex = (pageIndex == startPage ? startRow : page.Rows.Count); --rowIndex >= 0 && !completed; )
						{
							row = page.Rows[rowIndex];
							for (int eventIndex = row.Events.Count; --eventIndex >= 0 && !completed; )
							{
								if (setInstrument.IsSameKind(row.Events[eventIndex]))
								{
									saveEvents = FindFreeSpace(data.Pages, setInstrument, pageIndex, rowIndex, startPage, startRow, out saveIndex, true);
									if (saveEvents != null)
									{
										events.RemoveAt(index);
										saveEvents.Insert(saveIndex, setInstrument);
										SetVolumeEvent setVolume;
										if (TryExtractSetVolume<IEchoEvent>(setInstrument.Channel, ref events, out setVolume))
											saveEvents.Insert(saveIndex + 1, setVolume);
									}
									completed = true;
								}
							}
						}
					}

					if (!completed)
					{
						for (int eventIndex = data.Header.Count; --eventIndex >= 0 && !completed; )
						{
							completed = setInstrument.IsSameKind(data.Header[eventIndex]);
						}

						if (!completed)
						{
							saveEvents = GetFreeSpace(data.Pages, startPage, startRow, 0, 0, out saveIndex);
							if (saveEvents != null)
							{
								events.RemoveAt(index);
								saveEvents.Insert(saveIndex, setInstrument);
								SetVolumeEvent setVolume;
								if (TryExtractSetVolume<IEchoEvent>(setInstrument.Channel, ref events, out setVolume))
									saveEvents.Insert(saveIndex + 1, setVolume);
							}
						}
					}

					completed = true;
				}

				if (!completed) break;
			}

			RefreshRelevantEventIndexes(events);
			for (int i = _indexes.Count; --i >= 0; )
			{
				int index = _indexes[i];
				_indexes.RemoveAt(i);

				var action = (IEchoChannelEvent)events[index];
				bool completed = false;

				for (int pageIndex = startPage + 1; --pageIndex >= 0 && !completed; )
				{
					page = data.Pages[pageIndex];

					for (int rowIndex = (pageIndex == startPage? startRow : page.Rows.Count); --rowIndex >= 0 && !completed; )
					{
						row = page.Rows[rowIndex];
						for (int eventIndex = row.Events.Count; --eventIndex >= 0 && !completed; )
						{
							if (action.IsSameKind(row.Events[eventIndex]))
							{
								saveEvents = FindFreeSpace(data.Pages, action, pageIndex, rowIndex, startPage, startRow, out saveIndex, true);
								if (saveEvents != null)
								{
									events.RemoveAt(index);
									saveEvents.Insert(saveIndex, action);
								}

								completed = true;
							}
						}
					}
				}
			}
		}

		static private void TryDistributeLoopEvents(EchoESF data, List<IEchoChannelEvent> loopEvents)
		{
			int loopIndex = GetLoopIndex(data.Footer);
			if (loopIndex < 0) return;

			for (int i = loopIndex; --i >= 0; )
			{
				if(data.Footer[i] is DelayEvent)
				{
					DelayEvent delay = (DelayEvent)data.Footer[i];
					int ticks = delay.GetRealTicks();
					if (ticks > 1)
					{
						ticks--;
						data.Footer[i] = new DelayEvent((byte)ticks, ticks <= DelayEvent.SHORT_DELAY_LIMIT);
						loopIndex = i + 1;
						data.Footer.Insert(loopIndex, new DelayEvent(1, true));
						break;
					}
				}
			}

			EchoPatternPage page;
			EchoPatternRow row;

			List<IEchoEvent> saveEvents;
			int saveIndex;

			while (loopEvents.Count > 0)
			{
				bool completed = false;
				for (int i = loopEvents.Count; --i >= 0 && !completed; )
				{
					if (!(loopEvents[i] is SetInstrumentEvent)) continue;

					SetInstrumentEvent setInstrument = (SetInstrumentEvent)loopEvents[i];
					loopEvents.RemoveAt(i);

					for (int pageIndex = data.Pages.Count; --pageIndex >= 0 && !completed; )
					{
						page = data.Pages[pageIndex];

						for (int rowIndex = page.Rows.Count; --rowIndex >= 0 && !completed; )
						{
							row = page.Rows[rowIndex];
							for (int eventIndex = row.Events.Count; --eventIndex >= 0 && !completed; )
							{
								if (setInstrument.IsSameKind(row.Events[eventIndex]))
								{
									var prevInstrument = (SetInstrumentEvent)row.Events[eventIndex];
									if (!setInstrument.HadSameParameters(prevInstrument))
									{
										saveEvents = FindFreeSpace(data.Pages, setInstrument,
											pageIndex, rowIndex,
											data.Pages.Count - 1,
											data.Pages[data.Pages.Count - 1].Rows.Count,
											out saveIndex, false);

										if (saveEvents == null)
										{
											saveEvents = data.Footer;
											saveIndex = loopIndex;
										}

										saveEvents.Insert(saveIndex, setInstrument);
										SetVolumeEvent setVolume;
										if (TryExtractSetVolume<IEchoChannelEvent>(setInstrument.Channel, ref loopEvents, out setVolume))
											saveEvents.Insert(saveIndex + 1, setVolume);
									}
									completed = true;
								}
							}
						}
					}

					completed = true;
				}

				if (!completed) break;
			}

			for (int i = loopEvents.Count; --i >= 0; )
			{
				var action = loopEvents[i];
				loopEvents.RemoveAt(i);
				bool completed = false;

				for (int pageIndex = data.Pages.Count; --pageIndex >= 0 && !completed; )
				{
					page = data.Pages[pageIndex];

					for (int rowIndex = page.Rows.Count; --rowIndex >= 0 && !completed; )
					{
						row = page.Rows[rowIndex];
						for (int eventIndex = row.Events.Count; --eventIndex >= 0 && !completed; )
						{
							if (action.IsSameKind(row.Events[eventIndex]))
							{
								if (!action.HadSameParameters((IEchoChannelEvent)row.Events[eventIndex]))
								{
									saveEvents = FindFreeSpace(data.Pages, action,
										pageIndex, rowIndex,
										data.Pages.Count - 1,
										data.Pages[data.Pages.Count-1].Rows.Count,
										out saveIndex, false);

									if (saveEvents == null)
									{
										saveEvents = data.Footer;
										saveIndex = loopIndex;
									}
									saveEvents.Insert(saveIndex, action);
								}

								completed = true;
							}
						}
					}
				}
			}
		}

		static private readonly List<EchoPatternRow> _freeRows = new List<EchoPatternRow>();
		static private List<IEchoEvent> GetFreeSpace(List<EchoPatternPage> pages, int pageLimit, int rowLimit, int startPage, int startRow, out int index)
		{
			index = -1;
			_freeRows.Clear();

			for (int i = startPage; i <= pageLimit; i++)
			{
				var page = pages[i];
				for (int j = (i == startPage ? startRow : 0); j < (i == pageLimit ? rowLimit : page.Rows.Count); j++)
				{
					foreach (var item in page.Rows[j].Events)
					{
						if (item is DelayEvent)
						{
							_freeRows.Add(page.Rows[j]);
							break;
						}
					}
				}
			}

			if (_freeRows.Count > 0)
			{
				_freeRows.Sort(SortFreeRows);
				index = FindFreeSpaceIndex(_freeRows[0]);
				return _freeRows[0].Events;
			}

			return null;
		}

		static private List<IEchoEvent> FindFreeSpace(List<EchoPatternPage> pages, IEchoChannelEvent action, int pageLimit, int rowLimit, int startPage, int startRow, out int index, bool cutWhenNoteOn)
		{
			index = -1;
			
			for (int i = startPage+1; --i >= pageLimit; )
			{
				var page = pages[i];

				for (int j = (i == startPage? startRow: page.Rows.Count); --j >= (i == pageLimit? rowLimit:0); )
				{
					var row = page.Rows[j];
					for (int k = row.Events.Count; --k >= 0; )
					{
						if (cutWhenNoteOn && row.Events[k] is NoteOnEvent && ((NoteOnEvent)row.Events[k]).Channel == action.Channel) return null;

						if (row.Events[k] is NoteOffEvent && ((NoteOffEvent)row.Events[k]).Channel == action.Channel)
							return GetFreeSpace(pages, startPage, startRow, i, j + 1, out index);
					}
				}
			}

			return null;
		}

		static private int FindFreeSpaceIndex(EchoPatternRow row)
		{
			for (int i = 0; i < row.Events.Count; i++)
			{
				if (row.Events[i] is DelayEvent)
					return i + 1;
			}

			return -1;
		}

		static private int SortFreeRows(EchoPatternRow a, EchoPatternRow b)
		{
			if (a.Events.Count < b.Events.Count) return -1;
			if (a.Events.Count > b.Events.Count) return 1;

			return 0;
		}

		static private bool TryExtractSetVolume<T>(ESFChannel channel, ref List<T> events, out SetVolumeEvent setVolume)
			where T:IEchoEvent
		{
			setVolume = default(SetVolumeEvent);

			for (int i = events.Count; --i >= 0; )
			{
				if (events[i] is SetVolumeEvent)
				{
					var other = (SetVolumeEvent)((object)events[i]);
					if (other.Channel == channel)
					{
						events.RemoveAt(i);
						setVolume = other;
						return true;
					}
				}
			}

			return false;
		}

		static private List<int> _indexes = new List<int>();
		static private void RefreshRelevantEventIndexes(List<IEchoEvent> events)
		{
			_indexes.Clear();
			for (int i = 0; i < events.Count; i++)
			{
				if (events[i] is IEchoChannelEvent)
					_indexes.Add(i);
			}
		}

		static private bool ContaineStartLoop(List<IEchoEvent> events)
		{
			foreach(var item in events)
			{
				if (item is PlaybackEvent)
				{
					PlaybackEvent playback = (PlaybackEvent)item;
					if (playback.Action == PlaybackEvent.Actions.SetLoop)
						return true;
				}
			}

			return false;
		}

		static private int GetLoopIndex(List<IEchoEvent> events)
		{
			for (int i = 0; i < events.Count; i++)
			{
				if (events[i] is PlaybackEvent)
				{
					PlaybackEvent playback = (PlaybackEvent)events[i];
					if (playback.Action == PlaybackEvent.Actions.GoToLoop)
						return i;
				}
			}

			return -1;
		}

		static private void TryMergeTimers(List<EchoPatternPage> pages)
		{
			for (int i = 0; i < pages.Count; i++)
			{
				int totalDelay;
				var page = pages[i];
				for (int j = 0; j < page.Rows.Count; j++)
				{
					var row = page.Rows[j];
					for (int k = 0; k < row.Events.Count; k++)
					{
						if (row.Events[k] is DelayEvent)
						{
							DelayEvent delay = (DelayEvent)row.Events[k];
							if (delay.IsFullDelay)
								continue;

							while (k + 1 < row.Events.Count && row.Events[k + 1] is DelayEvent)
							{
								DelayEvent next = (DelayEvent)row.Events[k + 1];
								row.Events.RemoveAt(k + 1);

								totalDelay = delay.GetRealTicks() + next.GetRealTicks();
								if (totalDelay >= DelayEvent.DELAY_LIMIT)
								{
									delay = new DelayEvent(0, false);
									totalDelay -= DelayEvent.DELAY_LIMIT;

									if (totalDelay <= 0)
									{
										row.Events[k] = delay;
										break;
									}
									else
										row.Events.Insert(k++, delay);
								}
								
								delay = new DelayEvent((byte)totalDelay, totalDelay <= DelayEvent.SHORT_DELAY_LIMIT);
								row.Events[k] = delay;
							}
						}
					}
				}

				if (i <= 0 || page.Rows.Count <= 0 || page.Rows[0].Events.Count <= 0 || !(page.Rows[0].Events[0] is DelayEvent)) continue;
				var prevPage = pages[i-1];
				if (prevPage.Rows.Count <= 0 ||
					prevPage.Rows[prevPage.Rows.Count - 1].Events.Count <= 0 ||
					!(prevPage.Rows[prevPage.Rows.Count - 1].Events[prevPage.Rows[prevPage.Rows.Count - 1].Events.Count - 1] is DelayEvent)) continue;

				DelayEvent currentDelay = (DelayEvent)page.Rows[0].Events[0];
				var prevLastRow = prevPage.Rows[prevPage.Rows.Count - 1];
				DelayEvent prevDelay = (DelayEvent)prevLastRow.Events[prevLastRow.Events.Count - 1];

				if (prevDelay.IsFullDelay || currentDelay.IsFullDelay) continue;

				totalDelay = currentDelay.GetRealTicks() + prevDelay.GetRealTicks();
				if (totalDelay >= DelayEvent.DELAY_LIMIT)
				{
					prevLastRow.Events[prevLastRow.Events.Count - 1] = new DelayEvent(0, false);
					totalDelay -= DelayEvent.DELAY_LIMIT;
					if (totalDelay <= 0)
					{
						page.Rows[0].Events.RemoveAt(0);
						continue;
					}
				}
				else
					prevLastRow.Events.RemoveAt(prevLastRow.Events.Count - 1);

				page.Rows[0].Events[0] = new DelayEvent((byte)totalDelay, totalDelay <= DelayEvent.SHORT_DELAY_LIMIT);
			}
		}
	}
}