using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace RubezhAPI
{
	public partial interface IRubezhServiceSKD
	{
		[OperationContract]
		OperationResult<List<DayInterval>> GetDayIntervals(Guid clientUID, DayIntervalFilter filter);
		[OperationContract]
		OperationResult<bool> SaveDayInterval(Guid clientUID, DayInterval item, bool isNew);
		[OperationContract]
		OperationResult<bool> MarkDeletedDayInterval(Guid clientUID, Guid uid, string name);
		[OperationContract]
		OperationResult<bool> RestoreDayInterval(Guid clientUID, Guid uid, string name);

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
		OperationResult<bool> MarkDeletedHoliday(Guid clientUID, Guid uid, string name);
		[OperationContract]
		OperationResult<bool> RestoreHoliday(Guid clientUID, Guid uid, string name);

		[OperationContract]
		OperationResult<List<ScheduleScheme>> GetScheduleSchemes(Guid clientUID, ScheduleSchemeFilter filter);
		[OperationContract]
		OperationResult<bool> SaveScheduleScheme(Guid clientUID, ScheduleScheme item, bool isNew);
		[OperationContract]
		OperationResult<bool> MarkDeletedScheduleScheme(Guid clientUID, Guid uid, string name);
		[OperationContract]
		OperationResult<bool> RestoreScheduleScheme(Guid clientUID, Guid uid, string name);

		[OperationContract]
		OperationResult<List<Schedule>> GetSchedules(Guid clientUID, ScheduleFilter filter);
		[OperationContract]
		OperationResult<bool> SaveSchedule(Guid clientUID, Schedule item, bool isNew);
		[OperationContract]
		OperationResult<bool> MarkDeletedSchedule(Guid clientUID, Guid uid, string name);
		[OperationContract]
		OperationResult<bool> RestoreSchedule(Guid clientUID, Guid uid, string name);

		[OperationContract]
		OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid clientUID, Guid employeeUID, DateTime startDateTime, DateTime endDateTime);
		[OperationContract]
		OperationResult<bool> AddTimeTrackDocument(Guid clientUID, TimeTrackDocument timeTrackDocument);
		[OperationContract]
		OperationResult<bool> EditTimeTrackDocument(Guid clientUID, TimeTrackDocument timeTrackDocument);
		[OperationContract]
		OperationResult<bool> RemoveTimeTrackDocument(Guid clientUID, Guid timeTrackDocumentUID);

		[OperationContract]
		OperationResult<List<TimeTrackDocumentType>> GetTimeTrackDocumentTypes(Guid clientUID, Guid organisationUID);
		[OperationContract]
		OperationResult<bool> AddTimeTrackDocumentType(Guid clientUID, TimeTrackDocumentType timeTrackDocumentType);
		[OperationContract]
		OperationResult<bool> EditTimeTrackDocumentType(Guid clientUID, TimeTrackDocumentType timeTrackDocumentType);
		[OperationContract]
		OperationResult<bool> RemoveTimeTrackDocumentType(Guid clientUID, Guid timeTrackDocumentTypeUID);

		[OperationContract]
		OperationResult<bool> AddCustomPassJournal(Guid clientUID, Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime);
		[OperationContract]
		OperationResult<bool> EditPassJournal(Guid clientUID, Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime);
		[OperationContract]
		OperationResult<bool> DeletePassJournal(Guid clientUID, Guid uid);
		[OperationContract]
		OperationResult<bool> DeleteAllPassJournalItems(Guid clientUID, Guid uid, DateTime enterTime, DateTime exitTime);

		[OperationContract]
		OperationResult<DateTime> GetPassJournalMinDate(Guid clientUID);
		[OperationContract]
		OperationResult<DateTime> GetCardsMinDate(Guid clientUID);

	}
}