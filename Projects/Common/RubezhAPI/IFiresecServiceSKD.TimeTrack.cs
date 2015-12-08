using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace RubezhAPI
{
	public partial interface IFiresecServiceSKD
	{
		[OperationContract]
		OperationResult<List<DayInterval>> GetDayIntervals(Guid clientUID, DayIntervalFilter filter);
		[OperationContract]
		OperationResult<bool> SaveDayInterval(Guid clientUID, DayInterval item, bool isNew);
		[OperationContract]
		OperationResult MarkDeletedDayInterval(Guid clientUID, Guid uid, string name);
		[OperationContract]
		OperationResult RestoreDayInterval(Guid clientUID, Guid uid, string name);

		//[OperationContract]
		//OperationResult<IEnumerable<DayIntervalPart>> GetDayIntervalParts(Guid clientUID,DayIntervalPartFilter filter);
		//[OperationContract]
		//OperationResult SaveDayIntervalPart(Guid clientUID,DayIntervalPart item, string name);
		//[OperationContract]
		//OperationResult RemoveDayIntervalPart(Guid clientUID,Guid uid, string name);

		[OperationContract]
		OperationResult<List<Holiday>> GetHolidays(Guid clientUID, HolidayFilter filter);
		[OperationContract]
		OperationResult<bool> SaveHoliday(Guid clientUID, Holiday item, bool isNew);
		[OperationContract]
		OperationResult MarkDeletedHoliday(Guid clientUID, Guid uid, string name);
		[OperationContract]
		OperationResult RestoreHoliday(Guid clientUID, Guid uid, string name);

		[OperationContract]
		OperationResult<List<ScheduleScheme>> GetScheduleSchemes(Guid clientUID, ScheduleSchemeFilter filter);
		[OperationContract]
		OperationResult<bool> SaveScheduleScheme(Guid clientUID, ScheduleScheme item, bool isNew);
		[OperationContract]
		OperationResult MarkDeletedScheduleScheme(Guid clientUID, Guid uid, string name);
		[OperationContract]
		OperationResult RestoreScheduleScheme(Guid clientUID, Guid uid, string name);

		//[OperationContract]
		//OperationResult<IEnumerable<ScheduleDayInterval>> GetSheduleDayIntervals(Guid clientUID,ScheduleDayIntervalFilter filter);
		//[OperationContract]
		//OperationResult SaveSheduleDayInterval(Guid clientUID,ScheduleDayInterval item, string name);
		//[OperationContract]
		//OperationResult RemoveSheduleDayInterval(Guid clientUID,Guid uid, string name);

		[OperationContract]
		OperationResult<List<Schedule>> GetSchedules(Guid clientUID, ScheduleFilter filter);
		[OperationContract]
		OperationResult<bool> SaveSchedule(Guid clientUID, Schedule item, bool isNew);
		[OperationContract]
		OperationResult MarkDeletedSchedule(Guid clientUID, Guid uid, string name);
		[OperationContract]
		OperationResult RestoreSchedule(Guid clientUID, Guid uid, string name);

		//[OperationContract]
		//OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(Guid clientUID,ScheduleZoneFilter filter);
		//[OperationContract]
		//OperationResult SaveScheduleZone(Guid clientUID,ScheduleZone item, string name);
		//[OperationContract]
		//OperationResult MarkDeletedScheduleZone(Guid clientUID,Guid uid, string name);

		[OperationContract]
		OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid clientUID, Guid employeeUID, DateTime startDateTime, DateTime endDateTime);
		[OperationContract]
		OperationResult AddTimeTrackDocument(Guid clientUID, TimeTrackDocument timeTrackDocument);
		[OperationContract]
		OperationResult EditTimeTrackDocument(Guid clientUID, TimeTrackDocument timeTrackDocument);
		[OperationContract]
		OperationResult RemoveTimeTrackDocument(Guid clientUID, Guid timeTrackDocumentUID);

		[OperationContract]
		OperationResult<List<TimeTrackDocumentType>> GetTimeTrackDocumentTypes(Guid clientUID, Guid organisationUID);
		[OperationContract]
		OperationResult AddTimeTrackDocumentType(Guid clientUID, TimeTrackDocumentType timeTrackDocumentType);
		[OperationContract]
		OperationResult EditTimeTrackDocumentType(Guid clientUID, TimeTrackDocumentType timeTrackDocumentType);
		[OperationContract]
		OperationResult RemoveTimeTrackDocumentType(Guid clientUID, Guid timeTrackDocumentTypeUID);

		[OperationContract]
		OperationResult AddCustomPassJournal(Guid clientUID, Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime);
		[OperationContract]
		OperationResult EditPassJournal(Guid clientUID, Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime);
		[OperationContract]
		OperationResult DeletePassJournal(Guid clientUID, Guid uid);
		[OperationContract]
		OperationResult DeleteAllPassJournalItems(Guid clientUID, Guid uid, DateTime enterTime, DateTime exitTime);

		[OperationContract]
		OperationResult<DateTime> GetPassJournalMinDate(Guid clientUID);
		[OperationContract]
		OperationResult<DateTime> GetCardsMinDate(Guid clientUID);

	}
}