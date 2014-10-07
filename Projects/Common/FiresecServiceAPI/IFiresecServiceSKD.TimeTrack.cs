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
		OperationResult RestoreDayInterval(Guid uid);

		[OperationContract]
		OperationResult<IEnumerable<DayIntervalPart>> GetDayIntervalParts(DayIntervalPartFilter filter);
		[OperationContract]
		OperationResult SaveDayIntervalPart(DayIntervalPart item);
		[OperationContract]
		OperationResult RemoveDayIntervalPart(Guid uid);
		
		[OperationContract]
		OperationResult<IEnumerable<Holiday>> GetHolidays(HolidayFilter filter);
		[OperationContract]
		OperationResult SaveHoliday(Holiday item);
		[OperationContract]
		OperationResult MarkDeletedHoliday(Guid uid);
		[OperationContract]
		OperationResult RestoreHoliday(Guid uid);

		[OperationContract]
		OperationResult<IEnumerable<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter);
		[OperationContract]
		OperationResult SaveScheduleScheme(ScheduleScheme item);
		[OperationContract]
		OperationResult MarkDeletedScheduleScheme(Guid uid);
		[OperationContract]
		OperationResult RestoreScheduleScheme(Guid uid);

		[OperationContract]
		OperationResult<IEnumerable<ScheduleDayInterval>> GetSheduleDayIntervals(ScheduleDayIntervalFilter filter);
		[OperationContract]
		OperationResult SaveSheduleDayInterval(ScheduleDayInterval item);
		[OperationContract]
		OperationResult RemoveSheduleDayInterval(Guid uid);

		[OperationContract]
		OperationResult<IEnumerable<Schedule>> GetSchedules(ScheduleFilter filter);
		[OperationContract]
		OperationResult SaveSchedule(Schedule item);
		[OperationContract]
		OperationResult MarkDeletedSchedule(Guid uid);
		[OperationContract]
		OperationResult<IEnumerable<ShortSchedule>> GetScheduleShortList(ScheduleFilter filter);
		[OperationContract]
		OperationResult RestoreSchedule(Guid uid);

		[OperationContract]
		OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter);
		[OperationContract]
		OperationResult SaveScheduleZone(ScheduleZone item);
		[OperationContract]
		OperationResult MarkDeletedScheduleZone(Guid uid);

		[OperationContract]
		OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid employeeUID, DateTime startDateTime, DateTime endDateTime);
		[OperationContract]
		OperationResult AddTimeTrackDocument(TimeTrackDocument timeTrackDocument);
		[OperationContract]
		OperationResult EditTimeTrackDocument(TimeTrackDocument timeTrackDocument);
		[OperationContract]
		OperationResult RemoveTimeTrackDocument(Guid timeTrackDocumentUID);

		[OperationContract]
		OperationResult<List<TimeTrackDocumentType>> GetTimeTrackDocumentTypes(Guid organisationUID);
		[OperationContract]
		OperationResult AddTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType);
		[OperationContract]
		OperationResult EditTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType);
		[OperationContract]
		OperationResult RemoveTimeTrackDocumentType(Guid timeTrackDocumentTypeUID);
	}
}