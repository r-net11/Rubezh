using System.Collections.Generic;
using Common;
using FiresecAPI.EmployeeTimeIntervals;
using System;

namespace FiresecClient
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
		public FiresecAPI.OperationResult MarkDeletedNamedInterval(NamedInterval item)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedNamedInterval(item.UID));
		}
	
		public FiresecAPI.OperationResult<IEnumerable<TimeInterval>> GetTimeIntervals(TimeIntervalFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<TimeInterval>>>(() => FiresecService.GetTimeIntervals(filter));
		}
		public FiresecAPI.OperationResult SaveTimeInterval(TimeInterval item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveTimeInterval(item));
		}
		public FiresecAPI.OperationResult MarkDeletedTimeInterval(TimeInterval item)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedTimeInterval(item.UID));
		}

		public FiresecAPI.OperationResult<IEnumerable<Holiday>> GetHolidays(HolidayFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<Holiday>>>(() => FiresecService.GetHolidays(filter));
		}
		public FiresecAPI.OperationResult SaveHoliday(Holiday item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveHoliday(item));
		}
		public FiresecAPI.OperationResult MarkDeletedHoliday(Holiday item)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedHoliday(item.UID));
		}

		public FiresecAPI.OperationResult<IEnumerable<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<ScheduleScheme>>>(() => FiresecService.GetScheduleSchemes(filter));
		}
		public FiresecAPI.OperationResult SaveScheduleScheme(ScheduleScheme item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveScheduleScheme(item));
		}
		public FiresecAPI.OperationResult MarkDeletedScheduleScheme(ScheduleScheme item)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedScheduleScheme(item.UID));
		}

		public FiresecAPI.OperationResult<IEnumerable<DayInterval>> GetDayIntervals(DayIntervalFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<DayInterval>>>(() => FiresecService.GetDayIntervals(filter));
		}
		public FiresecAPI.OperationResult SaveDayInterval(DayInterval item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveDayInterval(item));
		}
		public FiresecAPI.OperationResult MarkDeletedDayInterval(DayInterval item)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedDayInterval(item.UID));
		}

		public FiresecAPI.OperationResult<IEnumerable<Schedule>> GetSchedules(ScheduleFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<Schedule>>>(() => FiresecService.GetSchedules(filter));
		}
		public FiresecAPI.OperationResult SaveSchedule(Schedule item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveSchedule(item));
		}
		public FiresecAPI.OperationResult MarkDeletedSchedule(Schedule item)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedSchedule(item.UID));
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
		public FiresecAPI.OperationResult MarkDeletedScheduleZone(ScheduleZone item)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedScheduleZone(item.UID));
		}

		public FiresecAPI.OperationResult<FiresecAPI.SKD.TimeTrackDocument> GetTimeTrackDocument(DateTime dateTime, Guid employeeUID)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<FiresecAPI.SKD.TimeTrackDocument>>(() => FiresecService.GetTimeTrackDocument(dateTime, employeeUID));
		}
		public FiresecAPI.OperationResult SaveTimeTrackDocument(FiresecAPI.SKD.TimeTrackDocument item)
		{
			return SafeContext.Execute(() => FiresecService.SaveTimeTrackDocument(item));
		}
	}
}