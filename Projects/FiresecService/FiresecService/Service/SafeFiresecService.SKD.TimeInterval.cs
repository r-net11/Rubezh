using System.Collections.Generic;
using Common;
using FiresecAPI.EmployeeTimeIntervals;
using System;

namespace FiresecService.Service
{
	public partial class SafeFiresecService
	{
		public FiresecAPI.OperationResult<IEnumerable<NamedInterval>> GetNamedIntervals(NamedIntervalFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<NamedInterval>>>(() => FiresecService.GetNamedIntervals(filter));
		}
		public FiresecAPI.OperationResult SaveNamedInterval(NamedInterval item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveNamedInterval(item));
		}
		public FiresecAPI.OperationResult MarkDeletedNamedInterval(Guid uid)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.MarkDeletedNamedInterval(uid));
		}

		public FiresecAPI.OperationResult<IEnumerable<TimeInterval>> GetTimeIntervals(TimeIntervalFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<TimeInterval>>>(() => FiresecService.GetTimeIntervals(filter));
		}
		public FiresecAPI.OperationResult SaveTimeInterval(TimeInterval item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveTimeInterval(item));
		}
		public FiresecAPI.OperationResult MarkDeletedTimeInterval(Guid uid)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.MarkDeletedTimeInterval(uid));
		}

		public FiresecAPI.OperationResult<IEnumerable<Holiday>> GetHolidays(HolidayFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<Holiday>>>(() => FiresecService.GetHolidays(filter));
		}
		public FiresecAPI.OperationResult SaveHoliday(Holiday item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveHoliday(item));
		}
		public FiresecAPI.OperationResult MarkDeletedHoliday(Guid uid)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.MarkDeletedHoliday(uid));
		}

		public FiresecAPI.OperationResult<IEnumerable<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<ScheduleScheme>>>(() => FiresecService.GetScheduleSchemes(filter));
		}
		public FiresecAPI.OperationResult SaveScheduleScheme(ScheduleScheme item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveScheduleScheme(item));
		}
		public FiresecAPI.OperationResult MarkDeletedScheduleScheme(Guid uid)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.MarkDeletedScheduleScheme(uid));
		}

		public FiresecAPI.OperationResult<IEnumerable<DayInterval>> GetDayIntervals(DayIntervalFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<DayInterval>>>(() => FiresecService.GetDayIntervals(filter));
		}
		public FiresecAPI.OperationResult SaveDayInterval(DayInterval item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveDayInterval(item));
		}
		public FiresecAPI.OperationResult MarkDeletedDayInterval(Guid uid)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.MarkDeletedDayInterval(uid));
		}

		public FiresecAPI.OperationResult<IEnumerable<Schedule>> GetSchedules(ScheduleFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<Schedule>>>(() => FiresecService.GetSchedules(filter));
		}
		public FiresecAPI.OperationResult SaveSchedule(Schedule item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveSchedule(item));
		}
		public FiresecAPI.OperationResult MarkDeletedSchedule(Guid uid)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.MarkDeletedSchedule(uid));
		}
		public FiresecAPI.OperationResult<IEnumerable<ShortSchedule>> GetScheduleShortList(ScheduleFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<ShortSchedule>>>(() => FiresecService.GetScheduleShortList(filter));
		}

		public FiresecAPI.OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<ScheduleZone>>>(() => FiresecService.GetScheduleZones(filter));
		}
		public FiresecAPI.OperationResult SaveScheduleZone(ScheduleZone item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveScheduleZone(item));
		}
		public FiresecAPI.OperationResult MarkDeletedScheduleZone(Guid uid)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.MarkDeletedScheduleZone(uid));
		}
	}
}