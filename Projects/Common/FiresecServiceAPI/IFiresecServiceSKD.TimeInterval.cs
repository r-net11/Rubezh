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
		OperationResult MarkDeletedNamedIntervals(IEnumerable<TimeIntervals.NamedInterval> items);

		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.TimeInterval>> GetTimeIntervals(TimeIntervals.TimeIntervalFilter filter);
		[OperationContract]
		OperationResult SaveTimeIntervals(IEnumerable<TimeIntervals.TimeInterval> items);
		[OperationContract]
		OperationResult MarkDeletedTimeIntervals(IEnumerable<TimeIntervals.TimeInterval> items);

		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.Holiday>> GetHolidays(TimeIntervals.HolidayFilter filter);
		[OperationContract]
		OperationResult SaveHolidays(IEnumerable<TimeIntervals.Holiday> items);
		[OperationContract]
		OperationResult MarkDeletedHolidays(IEnumerable<TimeIntervals.Holiday> items);

		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.ScheduleScheme>> GetScheduleSchemes(TimeIntervals.ScheduleSchemeFilter filter);
		[OperationContract]
		OperationResult SaveScheduleSchemes(IEnumerable<TimeIntervals.ScheduleScheme> items);
		[OperationContract]
		OperationResult MarkDeletedScheduleSchemes(IEnumerable<TimeIntervals.ScheduleScheme> items);

		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.DayInterval>> GetDayIntervals(TimeIntervals.DayIntervalFilter filter);
		[OperationContract]
		OperationResult SaveDayIntervals(IEnumerable<TimeIntervals.DayInterval> items);
		[OperationContract]
		OperationResult MarkDeletedDayIntervals(IEnumerable<TimeIntervals.DayInterval> items);

		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.Schedule>> GetSchedules(TimeIntervals.ScheduleFilter filter);
		[OperationContract]
		OperationResult SaveSchedules(IEnumerable<TimeIntervals.Schedule> items);
		[OperationContract]
		OperationResult MarkDeletedSchedules(IEnumerable<TimeIntervals.Schedule> items);
		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.ShortSchedule>> GetScheduleShortList(TimeIntervals.ScheduleFilter filter);

		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.ScheduleZone>> GetScheduleZones(TimeIntervals.ScheduleZoneFilter filter);
		[OperationContract]
		OperationResult SaveScheduleZones(IEnumerable<TimeIntervals.ScheduleZone> items);
		[OperationContract]
		OperationResult MarkDeletedScheduleZones(IEnumerable<TimeIntervals.ScheduleZone> items);
	}
}