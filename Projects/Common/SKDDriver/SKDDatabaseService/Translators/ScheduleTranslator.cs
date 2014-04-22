using System;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using LinqKit;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class ScheduleTranslator : OrganisationElementTranslator<DataAccess.Schedule, Schedule, ScheduleFilter>
	{
		private ScheduleZoneTranslator _scheduleZoneTranslator;
		public ScheduleTranslator(DataAccess.SKDDataContext context, ScheduleZoneTranslator scheduleZoneTranslator)
			: base(context)
		{
			_scheduleZoneTranslator = scheduleZoneTranslator;
		}

		protected override IQueryable<DataAccess.Schedule> GetQuery(ScheduleFilter filter)
		{
			return base.GetQuery(filter).OrderBy(item => item.Name);
		}

		protected override OperationResult CanSave(Schedule item)
		{
			bool sameName = Table.Any(x => x.OrganisationUID == item.OrganisationUID && x.UID != item.UID && !x.IsDeleted && x.Name == item.Name);
			if (sameName)
				return new OperationResult("График с таким же названием уже содержится в базе данных");
			return base.CanSave(item);
		}

		protected override Schedule Translate(DataAccess.Schedule tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.ScheduleSchemeUID = tableItem.ScheduleSchemeUID.HasValue ? tableItem.ScheduleSchemeUID.Value : Guid.Empty;
			result.IsIgnoreHoliday = tableItem.IsIgnoreHoliday;
			result.IsOnlyFirstEnter = tableItem.IsOnlyFirstEnter;
			result.Zones = _scheduleZoneTranslator.TranslateAll(tableItem.ScheduleZoneLinks.Where(item => !item.IsDeleted));
			return result;
		}

		protected override void TranslateBack(DataAccess.Schedule tableItem, Schedule apiItem)
		{
			var scheduleScheme = apiItem.ScheduleSchemeUID == Guid.Empty ? null : Context.ScheduleSchemes.FirstOrDefault(item => item.UID == apiItem.ScheduleSchemeUID);
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.IsIgnoreHoliday = apiItem.IsIgnoreHoliday;
			tableItem.IsOnlyFirstEnter = apiItem.IsOnlyFirstEnter;
			if (scheduleScheme == null && apiItem.ScheduleSchemeUID != Guid.Empty)
				tableItem.ScheduleSchemeUID = apiItem.ScheduleSchemeUID;
			else
				tableItem.ScheduleScheme = scheduleScheme;
			_scheduleZoneTranslator.Save(apiItem.Zones, false);
		}
	}
}
