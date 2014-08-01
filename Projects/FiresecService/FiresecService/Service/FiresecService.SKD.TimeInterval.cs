using System.Collections.Generic;
using FiresecAPI.EmployeeTimeIntervals;
using SKDDriver;

namespace FiresecService.Service
{
	public partial class FiresecService : FiresecAPI.IFiresecService
	{
		public FiresecAPI.OperationResult<IEnumerable<NamedInterval>> GetNamedIntervals(NamedIntervalFilter filter)
		{
			return SKDDatabaseService.NamedIntervalTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveNamedIntervals(IEnumerable<NamedInterval> items)
		{
			return SKDDatabaseService.NamedIntervalTranslator.Save(items);
		}
		public FiresecAPI.OperationResult MarkDeletedNamedInterval(NamedInterval item)
		{
			return SKDDatabaseService.NamedIntervalTranslator.MarkDeleted(item.UID);
		}

		public FiresecAPI.OperationResult<IEnumerable<TimeInterval>> GetTimeIntervals(TimeIntervalFilter filter)
		{
			return SKDDatabaseService.TimeIntervalTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveTimeIntervals(IEnumerable<TimeInterval> items)
		{
			return SKDDatabaseService.TimeIntervalTranslator.Save(items);
		}
		public FiresecAPI.OperationResult MarkDeletedTimeInterval(TimeInterval item)
		{
			return SKDDatabaseService.TimeIntervalTranslator.MarkDeleted(item.UID);
		}

		public FiresecAPI.OperationResult<IEnumerable<Holiday>> GetHolidays(HolidayFilter filter)
		{
			return SKDDatabaseService.HolidayTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveHolidays(IEnumerable<Holiday> items)
		{
			return SKDDatabaseService.HolidayTranslator.Save(items);
		}
		public FiresecAPI.OperationResult MarkDeletedHoliday(Holiday item)
		{
			return SKDDatabaseService.HolidayTranslator.MarkDeleted(item.UID);
		}

		public FiresecAPI.OperationResult<IEnumerable<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			return SKDDatabaseService.ScheduleSchemeTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveScheduleSchemes(IEnumerable<ScheduleScheme> items)
		{
			return SKDDatabaseService.ScheduleSchemeTranslator.Save(items);
		}
		public FiresecAPI.OperationResult MarkDeletedScheduleScheme(ScheduleScheme item)
		{
			return SKDDatabaseService.ScheduleSchemeTranslator.MarkDeleted(item.UID);
		}

		public FiresecAPI.OperationResult<IEnumerable<DayInterval>> GetDayIntervals(DayIntervalFilter filter)
		{
			return SKDDatabaseService.DayIntervalTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveDayIntervals(IEnumerable<DayInterval> items)
		{
			return SKDDatabaseService.DayIntervalTranslator.Save(items);
		}
		public FiresecAPI.OperationResult MarkDeletedDayInterval(DayInterval item)
		{
			return SKDDatabaseService.DayIntervalTranslator.MarkDeleted(item.UID);
		}

		public FiresecAPI.OperationResult<IEnumerable<Schedule>> GetSchedules(ScheduleFilter filter)
		{
			return SKDDatabaseService.ScheduleTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveSchedules(IEnumerable<Schedule> items)
		{
			return SKDDatabaseService.ScheduleTranslator.Save(items);
		}
		public FiresecAPI.OperationResult MarkDeletedSchedule(Schedule item)
		{
			return SKDDatabaseService.ScheduleTranslator.MarkDeleted(item.UID);
		}
		public FiresecAPI.OperationResult<IEnumerable<ShortSchedule>> GetScheduleShortList(ScheduleFilter filter)
		{
			return SKDDatabaseService.ScheduleTranslator.GetList(filter);
		}
		
		public FiresecAPI.OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter)
		{
			return SKDDatabaseService.ScheduleZoneTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveScheduleZones(IEnumerable<ScheduleZone> items)
		{
			return SKDDatabaseService.ScheduleZoneTranslator.Save(items);
		}
		public FiresecAPI.OperationResult MarkDeletedScheduleZone(ScheduleZone item)
		{
			return SKDDatabaseService.ScheduleZoneTranslator.MarkDeleted(item.UID);
		}
	}
}