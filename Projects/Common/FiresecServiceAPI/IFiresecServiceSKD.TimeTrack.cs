using System;
using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI.SKD;

namespace FiresecAPI
{
	public partial interface IFiresecServiceSKD
	{
		[OperationContract]
		OperationResult<List<DayInterval>> GetDayIntervals(DayIntervalFilter filter);
		[OperationContract]
		OperationResult<bool> SaveDayInterval(DayInterval item, bool isNew);
		[OperationContract]
		OperationResult MarkDeletedDayInterval(Guid uid, string name);
		[OperationContract]
		OperationResult RestoreDayInterval(Guid uid, string name);

		//[OperationContract]
		//OperationResult<IEnumerable<DayIntervalPart>> GetDayIntervalParts(DayIntervalPartFilter filter);
		//[OperationContract]
		//OperationResult SaveDayIntervalPart(DayIntervalPart item, string name);
		//[OperationContract]
		//OperationResult RemoveDayIntervalPart(Guid uid, string name);
		
		[OperationContract]
		OperationResult<List<Holiday>> GetHolidays(HolidayFilter filter);
		[OperationContract]
		OperationResult<bool> SaveHoliday(Holiday item, bool isNew);
		[OperationContract]
		OperationResult MarkDeletedHoliday(Guid uid, string name);
		[OperationContract]
		OperationResult RestoreHoliday(Guid uid, string name);

		[OperationContract]
		OperationResult<List<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter);
		[OperationContract]
		OperationResult<bool> SaveScheduleScheme(ScheduleScheme item, bool isNew);
		[OperationContract]
		OperationResult MarkDeletedScheduleScheme(Guid uid, string name);
		[OperationContract]
		OperationResult RestoreScheduleScheme(Guid uid, string name);

		//[OperationContract]
		//OperationResult<IEnumerable<ScheduleDayInterval>> GetSheduleDayIntervals(ScheduleDayIntervalFilter filter);
		//[OperationContract]
		//OperationResult SaveSheduleDayInterval(ScheduleDayInterval item, string name);
		//[OperationContract]
		//OperationResult RemoveSheduleDayInterval(Guid uid, string name);

		[OperationContract]
		OperationResult<List<Schedule>> GetSchedules(ScheduleFilter filter);
		[OperationContract]
		OperationResult<bool> SaveSchedule(Schedule item, bool isNew);
		[OperationContract]
		OperationResult MarkDeletedSchedule(Guid uid, string name);
		[OperationContract]
		OperationResult RestoreSchedule(Guid uid, string name);

		//[OperationContract]
		//OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter);
		//[OperationContract]
		//OperationResult SaveScheduleZone(ScheduleZone item, string name);
		//[OperationContract]
		//OperationResult MarkDeletedScheduleZone(Guid uid, string name);

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

		[OperationContract]
		OperationResult AddCustomPassJournal(Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime);
		[OperationContract]
		OperationResult EditPassJournal(Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime);
		[OperationContract]
		OperationResult DeletePassJournal(Guid uid);
		[OperationContract]
		OperationResult DeleteAllPassJournalItems(Guid uid, DateTime enterTime, DateTime exitTime);

		[OperationContract]
		OperationResult<DateTime> GetPassJournalMinDate();
		[OperationContract]
		OperationResult<DateTime> GetCardsMinDate();

	}
}