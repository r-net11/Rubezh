using System.Collections.Generic;
using FiresecAPI.EmployeeTimeIntervals;
using SKDDriver;
using System;

namespace FiresecService.Service
{
	public partial class FiresecService : FiresecAPI.IFiresecService
	{
		public FiresecAPI.OperationResult<IEnumerable<NamedInterval>> GetNamedIntervals(NamedIntervalFilter filter)
		{
			return SKDDatabaseService.NamedIntervalTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveNamedInterval(NamedInterval item)
		{
			return SKDDatabaseService.NamedIntervalTranslator.Save(item);
		}
		public FiresecAPI.OperationResult MarkDeletedNamedInterval(Guid uid)
		{
			return SKDDatabaseService.NamedIntervalTranslator.MarkDeleted(uid);
		}

		public FiresecAPI.OperationResult<IEnumerable<TimeInterval>> GetTimeIntervals(TimeIntervalFilter filter)
		{
			return SKDDatabaseService.TimeIntervalTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveTimeInterval(TimeInterval item)
		{
			return SKDDatabaseService.TimeIntervalTranslator.Save(item);
		}
		public FiresecAPI.OperationResult MarkDeletedTimeInterval(Guid uid)
		{
			return SKDDatabaseService.TimeIntervalTranslator.MarkDeleted(uid);
		}

		public FiresecAPI.OperationResult<IEnumerable<Holiday>> GetHolidays(HolidayFilter filter)
		{
			return SKDDatabaseService.HolidayTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveHoliday(Holiday item)
		{
			return SKDDatabaseService.HolidayTranslator.Save(item);
		}
		public FiresecAPI.OperationResult MarkDeletedHoliday(Guid uid)
		{
			return SKDDatabaseService.HolidayTranslator.MarkDeleted(uid);
		}

		public FiresecAPI.OperationResult<IEnumerable<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			return SKDDatabaseService.ScheduleSchemeTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveScheduleScheme(ScheduleScheme item)
		{
			return SKDDatabaseService.ScheduleSchemeTranslator.Save(item);
		}
		public FiresecAPI.OperationResult MarkDeletedScheduleScheme(Guid uid)
		{
			return SKDDatabaseService.ScheduleSchemeTranslator.MarkDeleted(uid);
		}

		public FiresecAPI.OperationResult<IEnumerable<DayInterval>> GetDayIntervals(DayIntervalFilter filter)
		{
			return SKDDatabaseService.DayIntervalTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveDayInterval(DayInterval item)
		{
			return SKDDatabaseService.DayIntervalTranslator.Save(item);
		}
		public FiresecAPI.OperationResult MarkDeletedDayInterval(Guid uid)
		{
			return SKDDatabaseService.DayIntervalTranslator.MarkDeleted(uid);
		}

		public FiresecAPI.OperationResult<IEnumerable<Schedule>> GetSchedules(ScheduleFilter filter)
		{
			return SKDDatabaseService.ScheduleTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveSchedule(Schedule item)
		{
			return SKDDatabaseService.ScheduleTranslator.Save(item);
		}
		public FiresecAPI.OperationResult MarkDeletedSchedule(Guid uid)
		{
			return SKDDatabaseService.ScheduleTranslator.MarkDeleted(uid);
		}
		public FiresecAPI.OperationResult<IEnumerable<ShortSchedule>> GetScheduleShortList(ScheduleFilter filter)
		{
			return SKDDatabaseService.ScheduleTranslator.GetList(filter);
		}
		
		public FiresecAPI.OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter)
		{
			return SKDDatabaseService.ScheduleZoneTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveScheduleZone(ScheduleZone item)
		{
			return SKDDatabaseService.ScheduleZoneTranslator.Save(item);
		}
		public FiresecAPI.OperationResult MarkDeletedScheduleZone(Guid uid)
		{
			return SKDDatabaseService.ScheduleZoneTranslator.MarkDeleted(uid);
		}
	}
}