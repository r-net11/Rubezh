using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SKDDriver;
using FiresecAPI.EmployeeTimeIntervals;

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
		public FiresecAPI.OperationResult MarkDeletedNamedIntervals(IEnumerable<NamedInterval> items)
		{
			return SKDDatabaseService.NamedIntervalTranslator.MarkDeleted(items);
		}

		public FiresecAPI.OperationResult<IEnumerable<TimeInterval>> GetTimeIntervals(TimeIntervalFilter filter)
		{
			return SKDDatabaseService.TimeIntervalTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveTimeIntervals(IEnumerable<TimeInterval> items)
		{
			return SKDDatabaseService.TimeIntervalTranslator.Save(items);
		}
		public FiresecAPI.OperationResult MarkDeletedTimeIntervals(IEnumerable<TimeInterval> items)
		{
			return SKDDatabaseService.TimeIntervalTranslator.MarkDeleted(items);
		}

		public FiresecAPI.OperationResult<IEnumerable<Holiday>> GetHolidays(HolidayFilter filter)
		{
			return SKDDatabaseService.HolidayTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveHolidays(IEnumerable<Holiday> items)
		{
			return SKDDatabaseService.HolidayTranslator.Save(items);
		}
		public FiresecAPI.OperationResult MarkDeletedHolidays(IEnumerable<Holiday> items)
		{
			return SKDDatabaseService.HolidayTranslator.MarkDeleted(items);
		}

		public FiresecAPI.OperationResult<IEnumerable<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			return SKDDatabaseService.ScheduleSchemeTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveScheduleSchemes(IEnumerable<ScheduleScheme> items)
		{
			return SKDDatabaseService.ScheduleSchemeTranslator.Save(items);
		}
		public FiresecAPI.OperationResult MarkDeletedScheduleSchemes(IEnumerable<ScheduleScheme> items)
		{
			return SKDDatabaseService.ScheduleSchemeTranslator.MarkDeleted(items);
		}

		public FiresecAPI.OperationResult<IEnumerable<DayInterval>> GetDayIntervals(DayIntervalFilter filter)
		{
			return SKDDatabaseService.DayIntervalTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveDayIntervals(IEnumerable<DayInterval> items)
		{
			return SKDDatabaseService.DayIntervalTranslator.Save(items);
		}
		public FiresecAPI.OperationResult MarkDeletedDayIntervals(IEnumerable<DayInterval> items)
		{
			return SKDDatabaseService.DayIntervalTranslator.MarkDeleted(items);
		}

		public FiresecAPI.OperationResult<IEnumerable<Schedule>> GetSchedules(ScheduleFilter filter)
		{
			return SKDDatabaseService.ScheduleTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveSchedules(IEnumerable<Schedule> items)
		{
			return SKDDatabaseService.ScheduleTranslator.Save(items);
		}
		public FiresecAPI.OperationResult MarkDeletedSchedules(IEnumerable<Schedule> items)
		{
			return SKDDatabaseService.ScheduleTranslator.MarkDeleted(items);
		}

		public FiresecAPI.OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter)
		{
			return SKDDatabaseService.ScheduleZoneTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveScheduleZones(IEnumerable<ScheduleZone> items)
		{
			return SKDDatabaseService.ScheduleZoneTranslator.Save(items);
		}
		public FiresecAPI.OperationResult MarkDeletedScheduleZones(IEnumerable<ScheduleZone> items)
		{
			return SKDDatabaseService.ScheduleZoneTranslator.MarkDeleted(items);
		}
	}
}