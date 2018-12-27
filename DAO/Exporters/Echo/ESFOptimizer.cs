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
