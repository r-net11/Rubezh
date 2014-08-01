using System.Collections.Generic;
using Common;
using FiresecAPI.EmployeeTimeIntervals;

namespace FiresecService.Service
{
	public partial class SafeFiresecService
	{
		public FiresecAPI.OperationResult<IEnumerable<NamedInterval>> GetNamedIntervals(NamedIntervalFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<NamedInterval>>>(() => FiresecService.GetNamedIntervals(filter));
		}
		public FiresecAPI.OperationResult SaveNamedIntervals(IEnumerable<NamedInterval> items)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveNamedIntervals(items));
		}
		public FiresecAPI.OperationResult MarkDeletedNamedInterval(NamedInterval item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.MarkDeletedNamedInterval(item));
		}

		public FiresecAPI.OperationResult<IEnumerable<TimeInterval>> GetTimeIntervals(TimeIntervalFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<TimeInterval>>>(() => FiresecService.GetTimeIntervals(filter));
		}
		public FiresecAPI.OperationResult SaveTimeIntervals(IEnumerable<TimeInterval> items)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveTimeIntervals(items));
		}
		public FiresecAPI.OperationResult MarkDeletedTimeInterval(TimeInterval item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.MarkDeletedTimeInterval(item));
		}

		public FiresecAPI.OperationResult<IEnumerable<Holiday>> GetHolidays(HolidayFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<Holiday>>>(() => FiresecService.GetHolidays(filter));
		}
		public FiresecAPI.OperationResult SaveHolidays(IEnumerable<Holiday> items)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveHolidays(items));
		}
		public FiresecAPI.OperationResult MarkDeletedHoliday(Holiday item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.MarkDeletedHoliday(item));
		}

		public FiresecAPI.OperationResult<IEnumerable<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<ScheduleScheme>>>(() => FiresecService.GetScheduleSchemes(filter));
		}
		public FiresecAPI.OperationResult SaveScheduleSchemes(IEnumerable<ScheduleScheme> items)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveScheduleSchemes(items));
		}
		public FiresecAPI.OperationResult MarkDeletedScheduleScheme(ScheduleScheme item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.MarkDeletedScheduleScheme(item));
		}

		public FiresecAPI.OperationResult<IEnumerable<DayInterval>> GetDayIntervals(DayIntervalFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<DayInterval>>>(() => FiresecService.GetDayIntervals(filter));
		}
		public FiresecAPI.OperationResult SaveDayIntervals(IEnumerable<DayInterval> items)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveDayIntervals(items));
		}
		public FiresecAPI.OperationResult MarkDeletedDayInterval(DayInterval item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.MarkDeletedDayInterval(item));
		}

		public FiresecAPI.OperationResult<IEnumerable<Schedule>> GetSchedules(ScheduleFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<Schedule>>>(() => FiresecService.GetSchedules(filter));
		}
		public FiresecAPI.OperationResult SaveSchedules(IEnumerable<Schedule> items)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveSchedules(items));
		}
		public FiresecAPI.OperationResult MarkDeletedSchedule(Schedule item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.MarkDeletedSchedule(item));
		}
		public FiresecAPI.OperationResult<IEnumerable<ShortSchedule>> GetScheduleShortList(ScheduleFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<ShortSchedule>>>(() => FiresecService.GetScheduleShortList(filter));
		}

		public FiresecAPI.OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<ScheduleZone>>>(() => FiresecService.GetScheduleZones(filter));
		}
		public FiresecAPI.OperationResult SaveScheduleZones(IEnumerable<ScheduleZone> items)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveScheduleZones(items));
		}
		public FiresecAPI.OperationResult MarkDeletedScheduleZone(ScheduleZone item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.MarkDeletedScheduleZone(item));
		}
	}
}