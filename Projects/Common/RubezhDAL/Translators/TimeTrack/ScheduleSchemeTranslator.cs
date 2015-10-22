using RubezhAPI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using API = RubezhAPI.SKD;

namespace RubezhDAL.DataClasses
{
	public class ScheduleSchemeTranslator : OrganisationItemTranslatorBase<ScheduleScheme, API.ScheduleScheme, API.ScheduleSchemeFilter>
	{
		public ScheduleSchemeTranslator(DbService context)
			: base(context) { }
		public override DbSet<ScheduleScheme> Table
		{
			get { return Context.ScheduleSchemes; }
		}

		public override IQueryable<ScheduleScheme> GetTableItems()
		{
			return base.GetTableItems().Include(x => x.ScheduleDays.Select(scheduleDay => scheduleDay.DayInterval));
		}

		protected override void ClearDependentData(ScheduleScheme tableItem)
		{
			Context.ScheduleDays.RemoveRange(tableItem.ScheduleDays);
		}

		protected override IEnumerable<API.ScheduleScheme> GetAPIItems(IQueryable<ScheduleScheme> tableItems)
		{
			return tableItems.Select(tableItem => new API.ScheduleScheme
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				IsDeleted = tableItem.IsDeleted,
				RemovalDate = tableItem.RemovalDate != null ? tableItem.RemovalDate.Value : new DateTime(),
				OrganisationUID = tableItem.OrganisationUID != null ? tableItem.OrganisationUID.Value : Guid.Empty,
				DayIntervals = tableItem.ScheduleDays.OrderBy(item => item.Number).Select(x => new API.ScheduleDayInterval
				{
					Number = x.Number,
					ScheduleSchemeUID = x.ScheduleSchemeUID != null ? x.ScheduleSchemeUID.Value : Guid.Empty,
					UID = x.UID,
					DayIntervalName = x.DayInterval != null ? x.DayInterval.Name : null,
					DayIntervalUID = x.DayInterval != null ? x.DayInterval.UID : Guid.Empty
				}).ToList(),
				Type = (API.ScheduleSchemeType)tableItem.Type,
				DaysCount = tableItem.DaysCount
			});
		}

		public override void TranslateBack(API.ScheduleScheme apiItem, ScheduleScheme tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			tableItem.Type = (int)apiItem.Type;
			tableItem.DaysCount = apiItem.DaysCount;
			tableItem.ScheduleDays = apiItem.DayIntervals.Select(x => new ScheduleDay
			{
				DayIntervalUID = x.DayIntervalUID != Guid.Empty ? (Guid?)x.DayIntervalUID : null,
				Number = x.Number,
				UID = x.UID
			}).ToList();
		}
	}
}