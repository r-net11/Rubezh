using System.Collections.Generic;
using System.ServiceModel;
using TimeIntervals = FiresecAPI.EmployeeTimeIntervals;
using System;

namespace FiresecAPI
{
	public partial interface IFiresecServiceSKD
	{
		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.NamedInterval>> GetNamedIntervals(TimeIntervals.NamedIntervalFilter filter);
		[OperationContract]
		OperationResult SaveNamedInterval(TimeIntervals.NamedInterval item);
		[OperationContract]
		OperationResult MarkDeletedNamedInterval(Guid uid);

		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.TimeInterval>> GetTimeIntervals(TimeIntervals.TimeIntervalFilter filter);
		[OperationContract]
		OperationResult SaveTimeInterval(TimeIntervals.TimeInterval item);
		[OperationContract]
		OperationResult MarkDeletedTimeInterval(Guid uid);

		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.Holiday>> GetHolidays(TimeIntervals.HolidayFilter filter);
		[OperationContract]
		OperationResult SaveHoliday(TimeIntervals.Holiday item);
		[OperationContract]
		OperationResult MarkDeletedHoliday(Guid uid);

		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.ScheduleScheme>> GetScheduleSchemes(TimeIntervals.ScheduleSchemeFilter filter);
		[OperationContract]
		OperationResult SaveScheduleScheme(TimeIntervals.ScheduleScheme item);
		[OperationContract]
		OperationResult MarkDeletedScheduleScheme(Guid uid);

		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.DayInterval>> GetDayIntervals(TimeIntervals.DayIntervalFilter filter);
		[OperationContract]
		OperationResult SaveDayInterval(TimeIntervals.DayInterval item);
		[OperationContract]
		OperationResult MarkDeletedDayInterval(Guid uid);

		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.Schedule>> GetSchedules(TimeIntervals.ScheduleFilter filter);
		[OperationContract]
		OperationResult SaveSchedule(TimeIntervals.Schedule item);
		[OperationContract]
		OperationResult MarkDeletedSchedule(Guid uid);
		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.ShortSchedule>> GetScheduleShortList(TimeIntervals.ScheduleFilter filter);

		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.ScheduleZone>> GetScheduleZones(TimeIntervals.ScheduleZoneFilter filter);
		[OperationContract]
		OperationResult SaveScheduleZone(TimeIntervals.ScheduleZone item);
		[OperationContract]
		OperationResult MarkDeletedScheduleZone(Guid uid);
	}
}