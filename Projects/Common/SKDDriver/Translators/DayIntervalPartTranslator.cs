using FiresecAPI.SKD;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OperationResult = FiresecAPI.OperationResult;

namespace StrazhDAL
{
	public class DayIntervalPartTranslator : WithFilterTranslator<DataAccess.DayIntervalPart, DayIntervalPart, DayIntervalPartFilter>
	{
		private int DaySeconds = 86400;

		public DayIntervalPartTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected override IQueryable<DataAccess.DayIntervalPart> GetQuery(DayIntervalPartFilter filter)
		{
			return base.GetQuery(filter).OrderBy(item => item.BeginTime);
		}

		protected override Expression<Func<DataAccess.DayIntervalPart, bool>> IsInFilter(DayIntervalPartFilter filter)
		{
			var result = base.IsInFilter(filter);
			result = result.And(e => filter.DayIntervalUIDs.Contains(e.DayIntervalUID));
			return result;
		}

		protected override DayIntervalPart Translate(DataAccess.DayIntervalPart tableItem)
		{
			var apiItem = base.Translate(tableItem);
			apiItem.DayIntervalUID = tableItem.DayIntervalUID;
			if (tableItem.EndTime > DaySeconds)
			{
				apiItem.TransitionType = DayIntervalPartTransitionType.Night;
				apiItem.BeginTime = TimeSpan.FromSeconds(tableItem.BeginTime);
				apiItem.EndTime = TimeSpan.FromSeconds(tableItem.EndTime - DaySeconds);
			}
			else
			{
				apiItem.TransitionType = DayIntervalPartTransitionType.Day;
				apiItem.BeginTime = TimeSpan.FromSeconds(tableItem.BeginTime);
				apiItem.EndTime = TimeSpan.FromSeconds(tableItem.EndTime);
			}
			return apiItem;
		}

		protected override void TranslateBack(DataAccess.DayIntervalPart tableItem, DayIntervalPart apiItem)
		{
			var dayInterval = Context.DayIntervals.FirstOrDefault(item => item.UID == apiItem.DayIntervalUID);
			tableItem.DayIntervalUID = apiItem.DayIntervalUID;
			if (dayInterval != null)
				tableItem.DayInterval = dayInterval;
			tableItem.BeginTime = (int)apiItem.BeginTime.TotalSeconds;
			tableItem.EndTime = (int)apiItem.EndTime.TotalSeconds;
			if (apiItem.TransitionType == DayIntervalPartTransitionType.Night)
			{
				tableItem.EndTime += DaySeconds;
			}
		}

		public List<DayIntervalPart> TranslateAll(IEnumerable<DataAccess.DayIntervalPart> list)
		{
			return list.Select(item => Translate(item)).ToList();
		}

		/// <summary>
		/// Получить коллекцию прочих временных интервалов для дневного графика
		/// </summary>
		/// <param name="dayIntervalPart">Текущий временной интервал, для дневного графика которого ищутся прочие временные интервалы</param>
		/// <returns>Коллекция временных интервалов дневного графика</returns>
		public IEnumerable<DayIntervalPart> GetOtherDayIntervalParts(DayIntervalPart dayIntervalPart)
		{
			return TranslateAll(Context.DayIntervalParts.Where(
				x => x.DayIntervalUID == dayIntervalPart.DayIntervalUID && x.UID != dayIntervalPart.UID));
		}
	}
}