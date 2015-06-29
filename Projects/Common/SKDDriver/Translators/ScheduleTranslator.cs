using System;
using System.Data.Entity;
using System.Linq;
using API = FiresecAPI.SKD;

namespace SKDDriver.DataClasses
{
	public class ScheduleTranslator : OrganisationItemTranslatorBase<Schedule, API.Schedule, API.ScheduleFilter>
	{
		public ScheduleTranslator(DbService context) : base(context) { }
		
		public override DbSet<Schedule> Table
		{
			get { return Context.Schedules; }
		}

		protected override IQueryable<Schedule> GetTableItems()
		{
			return base.GetTableItems().Include(x => x.ScheduleZones);
		}

		public override API.Schedule Translate(Schedule tableItem)
		{
			var result = base.Translate(tableItem);
            if (result == null)
                return null;
			result.ScheduleSchemeUID = tableItem.ScheduleSchemeUID.HasValue ? tableItem.ScheduleSchemeUID.Value : Guid.Empty;
			result.IsIgnoreHoliday = tableItem.IsIgnoreHoliday;
			result.IsOnlyFirstEnter = tableItem.IsOnlyFirstEnter;
			result.AllowedLate = TimeSpan.FromSeconds(tableItem.AllowedLate);
			result.AllowedEarlyLeave = TimeSpan.FromSeconds(tableItem.AllowedEarlyLeave);
			result.Zones = tableItem.ScheduleZones.Select(x => new API.ScheduleZone
			{
				DoorUID = x.DoorUID,
				ScheduleUID = x.ScheduleUID.Value,
				UID = x.UID,
				ZoneUID = x.ZoneUID
			}).ToList();
			return result;
		}

		public override void TranslateBack(API.Schedule apiItem, Schedule tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			tableItem.IsIgnoreHoliday = apiItem.IsIgnoreHoliday;
			tableItem.IsOnlyFirstEnter = apiItem.IsOnlyFirstEnter;
			tableItem.AllowedLate = (int)apiItem.AllowedLate.TotalSeconds;
			tableItem.AllowedEarlyLeave = (int)apiItem.AllowedEarlyLeave.TotalSeconds;
			tableItem.ScheduleSchemeUID = apiItem.ScheduleSchemeUID != Guid.Empty ? (Guid?)apiItem.ScheduleSchemeUID : null;
			tableItem.ScheduleZones = apiItem.Zones.Select(x => new ScheduleZone
			{
				DoorUID = x.DoorUID,
				ScheduleUID = x.ScheduleUID,
				UID = x.UID,
				ZoneUID = x.ZoneUID
			}).ToList();
		}

		protected override void ClearDependentData(Schedule tableItem)
		{
			Context.ScheduleZones.RemoveRange(tableItem.ScheduleZones);
		}
	}
}