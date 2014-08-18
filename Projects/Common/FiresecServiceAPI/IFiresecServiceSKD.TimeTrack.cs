using System;
using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI.SKD;

namespace FiresecAPI
{
	public partial interface IFiresecServiceSKD
	{
		[OperationContract]
		OperationResult<IEnumerable<DayInterval>> GetDayIntervals(DayIntervalFilter filter);
		[OperationContract]
		OperationResult SaveDayInterval(DayInterval item);
		[OperationContract]
		OperationResult MarkDeletedDayInterval(Guid uid);

		[OperationContract]
		OperationResult<IEnumerable<DayIntervalPart>> GetDayIntervalParts(DayIntervalPartFilter filter);
		[OperationContract]
		OperationResult SaveDayIntervalPart(DayIntervalPart item);
		[OperationContract]
		OperationResult MarkDeletedDayIntervalPart(Guid uid);

		[OperationContract]
		OperationResult<IEnumerable<Holiday>> GetHolidays(HolidayFilter filter);
		[OperationContract]
		OperationResult SaveHoliday(Holiday item);
		[OperationContract]
		OperationResult MarkDeletedHoliday(Guid uid);

		[OperationContract]
		OperationResult<IEnumerable<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter);
		[OperationContract]
		OperationResult SaveScheduleScheme(ScheduleScheme item);
		[OperationContract]
		OperationResult MarkDeletedScheduleScheme(Guid uid);

		[OperationContract]
		OperationResult<IEnumerable<ScheduleDayInterval>> GetSheduleDayIntervals(ScheduleDayIntervalFilter filter);
		[OperationContract]
		OperationResult SaveSheduleDayInterval(ScheduleDayInterval item);
		[OperationContract]
		OperationResult MarkDeletedSheduleDayInterval(Guid uid);

		[OperationContract]
		OperationResult<IEnumerable<Schedule>> GetSchedules(ScheduleFilter filter);
		[OperationContract]
		OperationResult SaveSchedule(Schedule item);
		[OperationContract]
		OperationResult MarkDeletedSchedule(Guid uid);
		[OperationContract]
		OperationResult<IEnumerable<ShortSchedule>> GetScheduleShortList(ScheduleFilter filter);

		[OperationContract]
		OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter);
		[OperationContract]
		OperationResult SaveScheduleZone(ScheduleZone item);
		[OperationContract]
		OperationResult MarkDeletedScheduleZone(Guid uid);

		[OperationContract]
		OperationResult<FiresecAPI.SKD.TimeTrackDocument> GetTimeTrackDocument(DateTime dateTime, Guid employeeUID);
		[OperationContract]
		OperationResult SaveTimeTrackDocument(FiresecAPI.SKD.TimeTrackDocument item);
	}
}