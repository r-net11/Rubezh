using System.Collections.Generic;
using System.ServiceModel;
using TimeIntervals = FiresecAPI.EmployeeTimeIntervals;
using System;

namespace FiresecAPI
{
	public partial interface IFiresecServiceSKD
	{
		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.DayInterval>> GetDayIntervals(TimeIntervals.DayIntervalFilter filter);
		[OperationContract]
		OperationResult SaveDayInterval(TimeIntervals.DayInterval item);
		[OperationContract]
		OperationResult MarkDeletedDayInterval(Guid uid);

		[OperationContract]
		OperationResult<IEnumerable<TimeIntervals.DayIntervalPart>> GetDayIntervalParts(TimeIntervals.DayIntervalPartFilter filter);
		[OperationContract]
		OperationResult SaveDayIntervalPart(TimeIntervals.DayIntervalPart item);
		[OperationContract]
		OperationResult MarkDeletedDayIntervalPart(Guid uid);

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
		OperationResult<IEnumerable<TimeIntervals.ScheduleDayInterval>> GetSheduleDayIntervals(TimeIntervals.ScheduleDayIntervalFilter filter);
		[OperationContract]
		OperationResult SaveSheduleDayInterval(TimeIntervals.ScheduleDayInterval item);
		[OperationContract]
		OperationResult MarkDeletedSheduleDayInterval(Guid uid);

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

		[OperationContract]
		OperationResult<FiresecAPI.SKD.TimeTrackDocument> GetTimeTrackDocument(DateTime dateTime, Guid employeeUID);
		[OperationContract]
		OperationResult SaveTimeTrackDocument(FiresecAPI.SKD.TimeTrackDocument item);
	}
}