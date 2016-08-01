using System;
using System.Collections.Generic;
using Common;
using StrazhAPI;
using StrazhAPI.Models;
using StrazhAPI.SKD;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		public OperationResult<IEnumerable<DayInterval>> GetDayIntervals(DayIntervalFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.GetDayIntervals(filter));
		}

		public OperationResult SaveDayInterval(DayInterval item, bool isNew)
		{
			return SafeContext.Execute(() => FiresecService.SaveDayInterval(item, isNew));
		}

		public OperationResult MarkDeletedDayInterval(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedDayInterval(uid, name));
		}

		public OperationResult RestoreDayInterval(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreDayInterval(uid, name));
		}

		public OperationResult<IEnumerable<DayIntervalPart>> GetDayIntervalParts(DayIntervalPartFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.GetDayIntervalParts(filter));
		}

		public OperationResult SaveDayIntervalPart(DayIntervalPart item, bool isNew, string name)
		{
			return SafeContext.Execute(() => FiresecService.SaveDayIntervalPart(item, isNew, name));
		}

		public OperationResult RemoveDayIntervalPart(DayIntervalPart item, string name)
		{
			return SafeContext.Execute(() => FiresecService.RemoveDayIntervalPart(item.UID, name));
		}

		public OperationResult<IEnumerable<Holiday>> GetHolidays(HolidayFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.GetHolidays(filter));
		}

		public OperationResult SaveHoliday(Holiday item, bool isNew)
		{
			return SafeContext.Execute(() => FiresecService.SaveHoliday(item, isNew));
		}

		public OperationResult MarkDeletedHoliday(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedHoliday(uid, name));
		}

		public OperationResult RestoreHoliday(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreHoliday(uid, name));
		}

		public OperationResult<IEnumerable<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.GetScheduleSchemes(filter));
		}

		public OperationResult SaveScheduleScheme(ScheduleScheme item, bool isNew)
		{
			return SafeContext.Execute(() => FiresecService.SaveScheduleScheme(item, isNew));
		}

		public OperationResult MarkDeletedScheduleScheme(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedScheduleScheme(uid, name));
		}

		public OperationResult RestoreScheduleScheme(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreScheduleScheme(uid, name));
		}

		public OperationResult<IEnumerable<ScheduleDayInterval>> GetSheduleDayIntervals(ScheduleDayIntervalFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.GetSheduleDayIntervals(filter));
		}

		public OperationResult SaveSheduleDayInterval(ScheduleDayInterval item, string name)
		{
			return SafeContext.Execute(() => FiresecService.SaveSheduleDayInterval(item, name));
		}

		public OperationResult RemoveSheduleDayInterval(ScheduleDayInterval item, string name)
		{
			return SafeContext.Execute(() => FiresecService.RemoveSheduleDayInterval(item.UID, name));
		}

		public OperationResult<IEnumerable<Schedule>> GetSchedules(ScheduleFilter filter)
		{
			return _isDisconnecting ? new OperationResult<IEnumerable<Schedule>>() : SafeContext.Execute(() => FiresecService.GetSchedules(filter));
		}

		public OperationResult SaveSchedule(Schedule item, bool isNew)
		{
			return SafeContext.Execute(() => FiresecService.SaveSchedule(item, isNew));
		}

		public OperationResult MarkDeletedSchedule(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedSchedule(uid, name));
		}

		public OperationResult<IEnumerable<ShortSchedule>> GetScheduleShortList(ScheduleFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.GetScheduleShortList(filter));
		}

		public OperationResult RestoreSchedule(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreSchedule(uid, name));
		}

		public OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter)
		{
			return SafeContext.Execute(() => FiresecService.GetScheduleZones(filter));
		}

		public OperationResult SaveScheduleZone(ScheduleZone item, string name)
		{
			return SafeContext.Execute(() => FiresecService.SaveScheduleZone(item, name));
		}

		public OperationResult MarkDeletedScheduleZone(ScheduleZone item, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedScheduleZone(item.UID, name));
		}

		public OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid employeeUID, DateTime startDateTime, DateTime endDateTime)
		{
			return SafeContext.Execute(() => FiresecService.GetTimeTrackDocument(employeeUID, startDateTime, endDateTime));
		}

		public OperationResult AddTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return SafeContext.Execute(() => FiresecService.AddTimeTrackDocument(timeTrackDocument));
		}

		public OperationResult EditTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return SafeContext.Execute(() => FiresecService.EditTimeTrackDocument(timeTrackDocument));
		}

		public OperationResult RemoveTimeTrackDocument(Guid timeTrackDocumentUID)
		{
			return SafeContext.Execute(() => FiresecService.RemoveTimeTrackDocument(timeTrackDocumentUID));
		}

		public OperationResult<List<TimeTrackDocumentType>> GetTimeTrackDocumentTypes(Guid organisationUID)
		{
			return SafeContext.Execute(() => FiresecService.GetTimeTrackDocumentTypes(organisationUID));
		}

		public OperationResult AddTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			return SafeContext.Execute(() => FiresecService.AddTimeTrackDocumentType(timeTrackDocumentType));
		}

		public OperationResult CheckDocumentType(TimeTrackDocumentType documentType, Guid organisationUID)
		{
			return SafeContext.Execute(() => FiresecService.CheckDocumentType(documentType, organisationUID));
		}

		public OperationResult EditTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			return SafeContext.Execute(() => FiresecService.EditTimeTrackDocumentType(timeTrackDocumentType));
		}

		public OperationResult RemoveTimeTrackDocumentType(Guid timeTrackDocumentTypeUID)
		{
			return SafeContext.Execute(() => FiresecService.RemoveTimeTrackDocumentType(timeTrackDocumentTypeUID));
		}

		public OperationResult<bool> CheckForCanForseCloseInterval(Guid openedIntervalGuid)
		{
			return SafeContext.Execute(() => FiresecService.CheckForCanForseCloseInterval(openedIntervalGuid));
		}

		public OperationResult<IEnumerable<DayTimeTrackPart>> GetIntersectionIntervals(DayTimeTrackPart currentDayTimeTrackPart, ShortEmployee currentEmployee)
		{
			return SafeContext.Execute(() => FiresecService.GetIntersectionIntervals(currentDayTimeTrackPart, currentEmployee));
		}

		public OperationResult SaveAllTimeTracks(IEnumerable<DayTimeTrackPart> collectionToSave, ShortEmployee employee, User currentUser, IEnumerable<DayTimeTrackPart> removeDayTimeTrackParts)
		{
			return SafeContext.Execute(() => FiresecService.SaveAllTimeTracks(collectionToSave, employee, currentUser, removeDayTimeTrackParts));
		}

		public OperationResult DeletePassJournal(Guid uid)
		{
			return SafeContext.Execute(() => FiresecService.DeletePassJournal(uid));
		}

		public OperationResult DeleteAllPassJournalItems(DayTimeTrackPart dayTimeTrackPart)
		{
			return SafeContext.Execute(() => FiresecService.DeleteAllPassJournalItems(dayTimeTrackPart));
		}

		public OperationResult<DateTime> GetPassJournalMinDate()
		{
			return SafeContext.Execute(() => FiresecService.GetPassJournalMinDate());
		}

		public OperationResult<DateTime> GetCardsMinDate()
		{
			return SafeContext.Execute(() => FiresecService.GetCardsMinDate());
		}
	}
}