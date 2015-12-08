using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace RubezhAPI
{
	public partial interface IFiresecServiceSKD
	{
		[OperationContract]
		OperationResult<List<DayInterval>> GetDayIntervals(DayIntervalFilter filter, Guid clientUID);
		[OperationContract]
		OperationResult<bool> SaveDayInterval(DayInterval item, bool isNew, Guid clientUID);
		[OperationContract]
		OperationResult MarkDeletedDayInterval(Guid uid, string name, Guid clientUID);
		[OperationContract]
		OperationResult RestoreDayInterval(Guid uid, string name, Guid clientUID);

		//[OperationContract]
		//OperationResult<IEnumerable<DayIntervalPart>> GetDayIntervalParts(DayIntervalPartFilter filter, Guid clientUID);
		//[OperationContract]
		//OperationResult SaveDayIntervalPart(DayIntervalPart item, string name, Guid clientUID);
		//[OperationContract]
		//OperationResult RemoveDayIntervalPart(Guid uid, string name, Guid clientUID);

		[OperationContract]
		OperationResult<List<Holiday>> GetHolidays(HolidayFilter filter, Guid clientUID);
		[OperationContract]
		OperationResult<bool> SaveHoliday(Holiday item, bool isNew, Guid clientUID);
		[OperationContract]
		OperationResult MarkDeletedHoliday(Guid uid, string name, Guid clientUID);
		[OperationContract]
		OperationResult RestoreHoliday(Guid uid, string name, Guid clientUID);

		[OperationContract]
		OperationResult<List<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter, Guid clientUID);
		[OperationContract]
		OperationResult<bool> SaveScheduleScheme(ScheduleScheme item, bool isNew, Guid clientUID);
		[OperationContract]
		OperationResult MarkDeletedScheduleScheme(Guid uid, string name, Guid clientUID);
		[OperationContract]
		OperationResult RestoreScheduleScheme(Guid uid, string name, Guid clientUID);

		//[OperationContract]
		//OperationResult<IEnumerable<ScheduleDayInterval>> GetSheduleDayIntervals(ScheduleDayIntervalFilter filter, Guid clientUID);
		//[OperationContract]
		//OperationResult SaveSheduleDayInterval(ScheduleDayInterval item, string name, Guid clientUID);
		//[OperationContract]
		//OperationResult RemoveSheduleDayInterval(Guid uid, string name, Guid clientUID);

		[OperationContract]
		OperationResult<List<Schedule>> GetSchedules(ScheduleFilter filter, Guid clientUID);
		[OperationContract]
		OperationResult<bool> SaveSchedule(Schedule item, bool isNew, Guid clientUID);
		[OperationContract]
		OperationResult MarkDeletedSchedule(Guid uid, string name, Guid clientUID);
		[OperationContract]
		OperationResult RestoreSchedule(Guid uid, string name, Guid clientUID);

		//[OperationContract]
		//OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter, Guid clientUID);
		//[OperationContract]
		//OperationResult SaveScheduleZone(ScheduleZone item, string name, Guid clientUID);
		//[OperationContract]
		//OperationResult MarkDeletedScheduleZone(Guid uid, string name, Guid clientUID);

		[OperationContract]
		OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid employeeUID, DateTime startDateTime, DateTime endDateTime, Guid clientUID);
		[OperationContract]
		OperationResult AddTimeTrackDocument(TimeTrackDocument timeTrackDocument, Guid clientUID);
		[OperationContract]
		OperationResult EditTimeTrackDocument(TimeTrackDocument timeTrackDocument, Guid clientUID);
		[OperationContract]
		OperationResult RemoveTimeTrackDocument(Guid timeTrackDocumentUID, Guid clientUID);

		[OperationContract]
		OperationResult<List<TimeTrackDocumentType>> GetTimeTrackDocumentTypes(Guid organisationUID, Guid clientUID);
		[OperationContract]
		OperationResult AddTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType, Guid clientUID);
		[OperationContract]
		OperationResult EditTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType, Guid clientUID);
		[OperationContract]
		OperationResult RemoveTimeTrackDocumentType(Guid timeTrackDocumentTypeUID, Guid clientUID);

		[OperationContract]
		OperationResult AddCustomPassJournal(Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime, Guid clientUID);
		[OperationContract]
		OperationResult EditPassJournal(Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime, Guid clientUID);
		[OperationContract]
		OperationResult DeletePassJournal(Guid uid, Guid clientUID);
		[OperationContract]
		OperationResult DeleteAllPassJournalItems(Guid uid, DateTime enterTime, DateTime exitTime, Guid clientUID);

		[OperationContract]
		OperationResult<DateTime> GetPassJournalMinDate(Guid clientUID);
		[OperationContract]
		OperationResult<DateTime> GetCardsMinDate(Guid clientUID);

	}
}