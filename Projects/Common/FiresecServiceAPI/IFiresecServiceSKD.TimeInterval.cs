using System.Collections.Generic;
using System.ServiceModel;
using TimeIntervals = FiresecAPI.EmployeeTimeIntervals;

namespace FiresecAPI
{
	public partial interface IFiresecServiceSKD
	{
		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.NamedInterval>> GetNamedIntervals(TimeIntervals.NamedIntervalFilter filter);
		[OperationContract]
		OperationResult SaveNamedIntervals(IEnumerable<TimeIntervals.NamedInterval> items);
		[OperationContract]
		OperationResult MarkDeletedNamedInterval(TimeIntervals.NamedInterval item);

		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.TimeInterval>> GetTimeIntervals(TimeIntervals.TimeIntervalFilter filter);
		[OperationContract]
		OperationResult SaveTimeIntervals(IEnumerable<TimeIntervals.TimeInterval> items);
		[OperationContract]
		OperationResult MarkDeletedTimeInterval(TimeIntervals.TimeInterval item);

		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.Holiday>> GetHolidays(TimeIntervals.HolidayFilter filter);
		[OperationContract]
		OperationResult SaveHolidays(IEnumerable<TimeIntervals.Holiday> items);
		[OperationContract]
		OperationResult MarkDeletedHoliday(TimeIntervals.Holiday item);

		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.ScheduleScheme>> GetScheduleSchemes(TimeIntervals.ScheduleSchemeFilter filter);
		[OperationContract]
		OperationResult SaveScheduleSchemes(IEnumerable<TimeIntervals.ScheduleScheme> items);
		[OperationContract]
		OperationResult MarkDeletedScheduleScheme(TimeIntervals.ScheduleScheme item);

		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.DayInterval>> GetDayIntervals(TimeIntervals.DayIntervalFilter filter);
		[OperationContract]
		OperationResult SaveDayIntervals(IEnumerable<TimeIntervals.DayInterval> items);
		[OperationContract]
		OperationResult MarkDeletedDayInterval(TimeIntervals.DayInterval item);

		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.Schedule>> GetSchedules(TimeIntervals.ScheduleFilter filter);
		[OperationContract]
		OperationResult SaveSchedules(IEnumerable<TimeIntervals.Schedule> items);
		[OperationContract]
		OperationResult MarkDeletedSchedule(TimeIntervals.Schedule item);
		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.ShortSchedule>> GetScheduleShortList(TimeIntervals.ScheduleFilter filter);

		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.ScheduleZone>> GetScheduleZones(TimeIntervals.ScheduleZoneFilter filter);
		[OperationContract]
		OperationResult SaveScheduleZones(IEnumerable<TimeIntervals.ScheduleZone> items);
		[OperationContract]
		OperationResult MarkDeletedScheduleZone(TimeIntervals.ScheduleZone item);
	}
}