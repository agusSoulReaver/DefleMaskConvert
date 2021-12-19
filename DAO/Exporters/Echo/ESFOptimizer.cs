using System;
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
			var firstEvents = data.Pages[0].Rows[0].Events;

			RefreshRelevantEventIndexes(firstEvents);
			if (_indexes.Count > 0)
			{
				data.Header.Insert(0, new DelayEvent(1, true));

				for (int i = _indexes.Count; --i >= 0; )
				{
					var channelEvent = (IEchoChannelEvent)firstEvents[_indexes[i]];
					firstEvents.RemoveAt(_indexes[i]);
					data.Header.Insert(0, channelEvent);
				}
			}

			for (int i = 0; i < data.Header.Count; i++)
			{
				var channelEvent = data.Header[i];
				if (channelEvent is LockChannelEvent)
				{
					data.Header.RemoveAt(i);
					data.Header.Insert(0, channelEvent);
				}
			}

			EchoPatternPage startLoopPage = ContaineStartLoop(data.Header) ? data.Pages[0] : GetLoopPage(data);

			for (int pageIndex = 0; pageIndex < data.Pages.Count; pageIndex++)
			{
				var page = data.Pages[pageIndex];

				for (int rowIndex = 0; rowIndex < page.Rows.Count; rowIndex++)
				{
					var row = page.Rows[rowIndex];
					if (startLoopPage == null && ContaineStartLoop(row.Events))
						startLoopPage = page;
					
					if (row.Events.Count <= 0) continue;
					RefreshRelevantEventIndexes(row.Events);

					if (_indexes.Count <= 0) continue;

					TryDistributeEvents(data, row.Events, pageIndex, rowIndex, 0);
				}
			}

			if (startLoopPage != null)
			{
				var loopEvents = FindLoopEvents(data, startLoopPage);
				if(loopEvents.Count > 0) TryDistributeLoopEvents(data, loopEvents);
			}
		}

		static private void TryDistributeEvents(EchoESF data, List<IEchoEvent> events, int startPage, int startRow, int loopPageIndex)
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

					for (int pageIndex = startPage + 1; --pageIndex >= loopPageIndex && !completed; )
					{
						page = data.Pages[pageIndex];

						for (int rowIndex = (pageIndex == startPage ? startRow : page.Rows.Count); --rowIndex >= 0 && !completed; )
						{
							row = page.Rows[rowIndex];
							for (int eventIndex = row.Events.Count; --eventIndex >= 0 && !completed; )
							{
								var eventData = row.Events[eventIndex];
								bool canMove = setInstrument.IsSameKind(eventData);
								if (!canMove && eventData is NoteOnEvent)
								{
									var noteOn = (NoteOnEvent)eventData;
									canMove = noteOn.Channel != ESFChannel.DAC && noteOn.Channel == setInstrument.Channel && noteOn.InstrumentIndex != setInstrument.InstrumentIndex;
								}

								if (canMove)
								{
									saveEvents = FindFreeSpace(data.Pages, setInstrument, pageIndex, rowIndex, startPage, startRow, out saveIndex, true);
									if (saveEvents != null)
									{
										events.RemoveAt(index);
										saveEvents.Insert(saveIndex, setInstrument);
										TryExtractAttributes<IEchoEvent>(saveEvents, saveIndex, setInstrument.Channel, ref events);
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
							saveEvents = GetFreeSpace(data.Pages, startPage, startRow, loopPageIndex, 0, out saveIndex);
							if (saveEvents != null)
							{
								events.RemoveAt(index);
								saveEvents.Insert(saveIndex, setInstrument);
								TryExtractAttributes<IEchoEvent>(saveEvents, saveIndex, setInstrument.Channel, ref events);
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
			List<IEchoEvent> endEvents;
			int loopIndex = GetLoopIndex(data, out endEvents);
			if (loopIndex < 0) return;

			for (int i = loopIndex; --i >= 0; )
			{
				if(endEvents[i] is DelayEvent)
				{
					DelayEvent delay = (DelayEvent)endEvents[i];
					int ticks = delay.GetRealTicks();
					if (ticks > 1)
					{
						ticks--;
						endEvents[i] = new DelayEvent((byte)ticks, ticks <= DelayEvent.SHORT_DELAY_LIMIT);
						loopIndex = i + 1;
						endEvents.Insert(loopIndex, new DelayEvent(1, true));
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
											saveEvents = endEvents;
											saveIndex = loopIndex;
										}

										if(setInstrument.Channel == ESFChannel.PSG4)
											saveEvents.Insert(saveIndex++, new NoteOffEvent(ESFChannel.PSG4));

										saveEvents.Insert(saveIndex, setInstrument);
										TryExtractAttributes<IEchoChannelEvent>(saveEvents, saveIndex, setInstrument.Channel, ref loopEvents);
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
										saveEvents = endEvents;
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

		static private void TryExtractAttributes<T>(List<IEchoEvent> saveEvents, int saveIndex, ESFChannel channel, ref List<T> events)
			where T:IEchoEvent
		{
			//SetFMParametersEvent setFMParams;
			//if (TryExtractSetFMParameters<T>(channel, ref events, out setFMParams))
			//	saveEvents.Insert(saveIndex + 1, setFMParams);

			SetVolumeEvent setVolume;
			if (TryExtractSetVolume<T>(channel, ref events, out setVolume))
				saveEvents.Insert(saveIndex + 1, setVolume);
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

		static private bool TryExtractSetFMParameters<T>(ESFChannel channel, ref List<T> events, out SetFMParametersEvent setVolume)
			where T : IEchoEvent
		{
			setVolume = default(SetFMParametersEvent);

			for (int i = events.Count; --i >= 0; )
			{
				if (events[i] is SetFMParametersEvent)
				{
					var other = (SetFMParametersEvent)((object)events[i]);
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
				var e = events[i];
				if (e is IEchoChannelEvent)
					_indexes.Add(i);
			}
		}

		static private EchoPatternPage GetLoopPage(EchoESF data)
		{
			foreach(var page in data.Pages)
			{
				foreach(var row in page.Rows)
				{
					if (ContaineStartLoop(row.Events))
						return page;
				}
			}

			return null;
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

		static private int GetLoopIndex(EchoESF data, out List<IEchoEvent> events)
		{
			int index = FindGoToLoopIndex(data.Footer);
			if (index >= 0)
			{
				events = data.Footer;
				return index;
			}

			for (int page = data.Pages.Count; --page >= 0; )
			{
				for (int row = data.Pages[page].Rows.Count; --row >= 0; )
				{
					events = data.Pages[page].Rows[row].Events;
					index = FindGoToLoopIndex(events);
					if (index >= 0)
						return index;
				}
			}

			events = null;
			return -1;
		}

		static private int FindGoToLoopIndex(List<IEchoEvent> events)
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

		static private List<IEchoChannelEvent> _loopEvents = new List<IEchoChannelEvent>();
		static private HashSet<ESFChannel> _loopChannels = new HashSet<ESFChannel>();
		static private List<IEchoChannelEvent> FindLoopEvents(EchoESF data, EchoPatternPage startPage)
		{
			_loopEvents.Clear();
			_loopChannels.Clear();

			int startIndex = data.Pages.IndexOf(startPage);
			bool goBackward = startIndex > 0;
			int dir = goBackward ? -1 : 1;
			FilterLoopEvents(data, startIndex, goBackward, dir);
			if(goBackward)
				FilterLoopEvents(data, startIndex, false, 1);

			return _loopEvents;
		}

		static private void FilterLoopEvents(EchoESF data, int startIndex, bool goBackward, int dir)
		{
			if (!goBackward) FilterLoopEvents(goBackward, data.Header, dir);

			for (int pageIndex = goBackward ? startIndex - 1 : startIndex; pageIndex >= 0 && pageIndex <= startIndex; pageIndex += dir)
			{
				var page = data.Pages[pageIndex];

				for (int rowIndex = goBackward ? page.Rows.Count - 1 : 0; rowIndex >= 0 && rowIndex < page.Rows.Count; rowIndex += dir)
				{
					var row = page.Rows[rowIndex];
					FilterLoopEvents(goBackward, row.Events, dir);
				}
			}

			if (goBackward) FilterLoopEvents(goBackward, data.Header, dir);
		}

		static private void FilterLoopEvents(bool goBackward, List<IEchoEvent> events, int dir)
		{
			for (int eventIndex = goBackward ? events.Count - 1 : 0; eventIndex >= 0 && eventIndex < events.Count; eventIndex += dir)
			{
				var e = events[eventIndex] as IEchoChannelEvent;
				if (e != null)
				{
					bool add = true;
					foreach (var item in _loopEvents)
					{
						if (item.IsSameKind(e))
						{
							add = false;
							break;
						}
					}

					if (add)
					{
						if(!(e is SetInstrumentEvent))
							_loopEvents.Add(e);
						else if (!_loopChannels.Contains(e.Channel))
						{
							_loopEvents.Add(e);
							_loopChannels.Add(e.Channel);
						}
					}
				}
			}
		}
	}
}
