using FiresecAPI.SKD;
using LinqKit;
using System;
using System.Linq;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class ScheduleTranslator : EmployeeTranslatorBase<DataAccess.Schedule, Schedule, ScheduleFilter, ShortSchedule>
	{
		public ScheduleTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected override IQueryable<DataAccess.Schedule> GetQuery(ScheduleFilter filter)
		{
			return base.GetQuery(filter).OrderBy(item => item.Name);
		}

		protected override System.Linq.Expressions.Expression<Func<DataAccess.Schedule, bool>> IsInFilter(ScheduleFilter filter)
		{
			var result = base.IsInFilter(filter);
			if (filter.ScheduleSchemeUIDs.Count > 0)
				result = result.And(e => filter.ScheduleSchemeUIDs.Contains(e.ScheduleSchemeUID.Value));
			return result;
		}

		protected override OperationResult CanSave(Schedule item)
		{
			var result = base.CanSave(item);
			if (result.HasError)
				return result;
			bool hasSameName = Table.Any(x => x.OrganisationUID == item.OrganisationUID && x.UID != item.UID && !x.IsDeleted && x.Name == item.Name);
			if (hasSameName)
				return new OperationResult(Resources.Language.ScheduleTranslator.CanSave_Error);
			return new OperationResult();
		}

		protected override Schedule Translate(DataAccess.Schedule tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.ScheduleSchemeUID = tableItem.ScheduleSchemeUID.HasValue ? tableItem.ScheduleSchemeUID.Value : Guid.Empty;
			result.IsIgnoreHoliday = tableItem.IsIgnoreHoliday;
			result.IsOnlyFirstEnter = tableItem.IsOnlyFirstEnter;
			result.AllowedLate = tableItem.AllowedLate;
			result.IsEnabledAllowLate = tableItem.IsEnabledAllowLate;
			result.AllowedEarlyLeave = tableItem.AllowedEarlyLeave;
			result.IsEnabledAllowEarlyLeave = tableItem.IsEnabledAllowEarlyLeave;
			result.AllowedAbsentLowThan = tableItem.AllowedAbsentLowThan;
			result.IsAllowAbsent = tableItem.IsAllowAbsent;
			result.NotAllowOvertimeLowerThan = tableItem.NotAllowOvertimeLowerThan;
			result.IsEnabledOvertime = tableItem.IsEnabledOvertime;
			result.Zones = DatabaseService.ScheduleZoneTranslator.TranslateAll(tableItem.ScheduleZones);
			return result;
		}

		protected override ShortSchedule TranslateToShort(DataAccess.Schedule tableItem)
		{
			var result = base.TranslateToShort(tableItem);
			result.Name = tableItem.Name;
			return result;
		}

		protected override void TranslateBack(DataAccess.Schedule tableItem, Schedule apiItem)
		{
			var scheduleScheme = apiItem.ScheduleSchemeUID == Guid.Empty ? null : Context.ScheduleSchemes.FirstOrDefault(item => item.UID == apiItem.ScheduleSchemeUID);
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.IsIgnoreHoliday = apiItem.IsIgnoreHoliday;
			tableItem.IsOnlyFirstEnter = apiItem.IsOnlyFirstEnter;
			tableItem.AllowedLate = apiItem.AllowedLate;
			tableItem.IsEnabledAllowLate = apiItem.IsEnabledAllowLate;
			tableItem.AllowedEarlyLeave = apiItem.AllowedEarlyLeave;
			tableItem.IsEnabledAllowEarlyLeave = apiItem.IsEnabledAllowEarlyLeave;
			tableItem.AllowedAbsentLowThan = apiItem.AllowedAbsentLowThan;
			tableItem.IsAllowAbsent = apiItem.IsAllowAbsent;
			tableItem.NotAllowOvertimeLowerThan = apiItem.NotAllowOvertimeLowerThan;
			tableItem.IsEnabledOvertime = apiItem.IsEnabledOvertime;
			if (scheduleScheme == null && apiItem.ScheduleSchemeUID != Guid.Empty)
				tableItem.ScheduleSchemeUID = apiItem.ScheduleSchemeUID;
			else
				tableItem.ScheduleScheme = scheduleScheme;
			//DatabaseService.ScheduleZoneTranslator.Save(apiItem.Zones);
		}

		public override OperationResult Save(Schedule apiItem)
		{
			var zonesSaveResult = DatabaseService.ScheduleZoneTranslator.Save(apiItem.Zones);
			if (zonesSaveResult.HasError)
				return zonesSaveResult;
			return base.Save(apiItem);
		}

		public string GetName(Guid? uid)
		{
			if (uid == null)
				return "";
			var tableItem = Table.FirstOrDefault(x => x.UID == uid.Value);
			if (tableItem == null)
				return "";
			return tableItem.Name;
		}

		protected override Guid? GetLinkUID(DataAccess.Employee employee)
		{
			return employee.ScheduleUID;
		}
	}
}