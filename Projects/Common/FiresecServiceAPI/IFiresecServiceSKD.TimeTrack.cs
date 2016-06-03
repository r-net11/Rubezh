using StrazhAPI.Models;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace StrazhAPI
{
	public partial interface IFiresecServiceSKD
	{
		[OperationContract]
		OperationResult<IEnumerable<DayInterval>> GetDayIntervals(DayIntervalFilter filter);

		[OperationContract]
		OperationResult SaveDayInterval(DayInterval item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedDayInterval(Guid uid, string name);

		[OperationContract]
		OperationResult RestoreDayInterval(Guid uid, string name);

		[OperationContract]
		OperationResult<IEnumerable<DayIntervalPart>> GetDayIntervalParts(DayIntervalPartFilter filter);

		[OperationContract]
		OperationResult SaveDayIntervalPart(DayIntervalPart item, bool isNew, string name);

		[OperationContract]
		OperationResult RemoveDayIntervalPart(Guid uid, string name);

		[OperationContract]
		OperationResult<IEnumerable<Holiday>> GetHolidays(HolidayFilter filter);

		[OperationContract]
		OperationResult SaveHoliday(Holiday item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedHoliday(Guid uid, string name);

		[OperationContract]
		OperationResult RestoreHoliday(Guid uid, string name);

		[OperationContract]
		OperationResult<IEnumerable<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter);

		[OperationContract]
		OperationResult SaveScheduleScheme(ScheduleScheme item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedScheduleScheme(Guid uid, string name);

		[OperationContract]
		OperationResult RestoreScheduleScheme(Guid uid, string name);

		[OperationContract]
		OperationResult<IEnumerable<ScheduleDayInterval>> GetSheduleDayIntervals(ScheduleDayIntervalFilter filter);

		[OperationContract]
		OperationResult SaveSheduleDayInterval(ScheduleDayInterval item, string name);

		[OperationContract]
		OperationResult RemoveSheduleDayInterval(Guid uid, string name);

		[OperationContract]
		OperationResult<IEnumerable<Schedule>> GetSchedules(ScheduleFilter filter);

		[OperationContract]
		OperationResult SaveSchedule(Schedule item, bool isNew);

		[OperationContract]
		OperationResult MarkDeletedSchedule(Guid uid, string name);

		[OperationContract]
		OperationResult<IEnumerable<ShortSchedule>> GetScheduleShortList(ScheduleFilter filter);

		[OperationContract]
		OperationResult RestoreSchedule(Guid uid, string name);

		[OperationContract]
		OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter);

		[OperationContract]
		OperationResult SaveScheduleZone(ScheduleZone item, string name);

		[OperationContract]
		OperationResult MarkDeletedScheduleZone(Guid uid, string name);

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
		OperationResult CheckDocumentType(TimeTrackDocumentType documentType, Guid organisationUID);

		[OperationContract]
		OperationResult EditTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType);

		[OperationContract]
		OperationResult RemoveTimeTrackDocumentType(Guid timeTrackDocumentTypeUID);

		[OperationContract]
		OperationResult<Dictionary<DayTimeTrackPart, List<DayTimeTrackPart>>> FindConflictIntervals(List<DayTimeTrackPart> dayTimeTrackParts, Guid employeeGuid, DateTime currentDate);

		[OperationContract]
		OperationResult<bool> CheckForCanForseCloseInterval(Guid openedIntervalGuid);

		[OperationContract]
		OperationResult<List<DayTimeTrackPart>> GetMissedIntervals(DateTime currentDate, ShortEmployee currentEmployee);

		[OperationContract]
		OperationResult<IEnumerable<DayTimeTrackPart>> GetIntersectionIntervals(DayTimeTrackPart currentDayTimeTrackPart,
			ShortEmployee currentEmployee);

		[OperationContract]
		OperationResult SaveAllTimeTracks(IEnumerable<DayTimeTrackPart> collectionToSave, ShortEmployee employee, User currentUser, IEnumerable<DayTimeTrackPart> removeDayTimeTrackParts, DateTime? resetAdjustmentsDate);

		[OperationContract]
		OperationResult DeletePassJournal(Guid uid);

		[OperationContract]
		OperationResult DeleteAllPassJournalItems(DayTimeTrackPart dayTimeTrackPart);

		[OperationContract]
		OperationResult<DateTime> GetPassJournalMinDate();

		[OperationContract]
		OperationResult<DateTime> GetCardsMinDate();
	}
}