﻿using FiresecAPI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using API = FiresecAPI.SKD;

namespace SKDDriver.DataClasses
{
	public class ScheduleSchemeTranslator : OrganisationItemTranslatorBase<ScheduleScheme, API.ScheduleScheme, API.ScheduleSchemeFilter>
	{
		public ScheduleSchemeTranslator(DbService context) : base(context) 
        {
            AsyncTranslator = new ScheduleSchemeAsyncTranslator(this);
        }

        public ScheduleSchemeAsyncTranslator AsyncTranslator { get; private set; }

		public override DbSet<ScheduleScheme> Table
		{
			get { return Context.ScheduleSchemes; }
		}

        public override IQueryable<ScheduleScheme> GetTableItems()
		{
			return base.GetTableItems().Include(x => x.ScheduleDays.Select(scheduleDay => scheduleDay.DayInterval));
		}
		
		public override API.ScheduleScheme Translate(ScheduleScheme tableItem)
		{
			var result = base.Translate(tableItem);
            if (result == null)
                return null;
			result.DayIntervals = tableItem.ScheduleDays.OrderBy(item => item.Number).Select(x => new API.ScheduleDayInterval
			{ 
				DayInterval = x.DayInterval != null ? DbService.DayIntervalTranslator.Translate(x.DayInterval) : null,
				Number = x.Number,
				ScheduleSchemeUID = x.ScheduleSchemeUID.GetValueOrDefault(),
				UID = x.UID
			}).ToList();
			result.Type = (API.ScheduleSchemeType)tableItem.Type;
			result.DaysCount = tableItem.DaysCount;
			return result;
		}

		public override void TranslateBack(API.ScheduleScheme apiItem, ScheduleScheme tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			tableItem.Type = (int)apiItem.Type;
			tableItem.DaysCount = apiItem.DaysCount;
			tableItem.ScheduleDays = apiItem.DayIntervals.Select(x => new ScheduleDay
			{
				DayIntervalUID = x.DayInterval != null && x.DayInterval.UID != Guid.Empty ? (Guid?)x.DayInterval.UID : null,
				Number = x.Number,
				UID = x.UID
			}).ToList();
		}

		protected override void ClearDependentData(ScheduleScheme tableItem)
		{
			Context.ScheduleDays.RemoveRange(tableItem.ScheduleDays);
		}
    }

    public class ScheduleSchemeAsyncTranslator : AsyncTranslator<ScheduleScheme, API.ScheduleScheme, API.ScheduleSchemeFilter>
    {
        public ScheduleSchemeAsyncTranslator(ScheduleSchemeTranslator translator) : base(translator as ITranslatorGet<ScheduleScheme, API.ScheduleScheme, API.ScheduleSchemeFilter>) { }
        public override List<API.ScheduleScheme> GetCollection(DbCallbackResult callbackResult)
        {
            return callbackResult.ScheduleSchemes;
        }
    }
}