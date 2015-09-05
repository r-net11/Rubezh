using System;
using System.Collections.Generic;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecAPI.SKD;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		public FiresecAPI.OperationResult<IEnumerable<DayInterval>> GetDayIntervals(DayIntervalFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<DayInterval>>>(() => FiresecService.GetDayIntervals(filter));
		}
		public FiresecAPI.OperationResult SaveDayInterval(DayInterval item, bool isNew)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveDayInterval(item, isNew));
		}
		public FiresecAPI.OperationResult MarkDeletedDayInterval(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedDayInterval(uid, name));
		}
		public FiresecAPI.OperationResult RestoreDayInterval(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreDayInterval(uid, name));
		}

		public FiresecAPI.OperationResult<IEnumerable<DayIntervalPart>> GetDayIntervalParts(DayIntervalPartFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<DayIntervalPart>>>(() => FiresecService.GetDayIntervalParts(filter));
		}
		public FiresecAPI.OperationResult SaveDayIntervalPart(DayIntervalPart item, string name)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveDayIntervalPart(item, name));
		}
		public FiresecAPI.OperationResult RemoveDayIntervalPart(DayIntervalPart item, string name)
		{
			return SafeContext.Execute(() => FiresecService.RemoveDayIntervalPart(item.UID, name));
		}

		public FiresecAPI.OperationResult<IEnumerable<Holiday>> GetHolidays(HolidayFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<Holiday>>>(() => FiresecService.GetHolidays(filter));
		}
		public FiresecAPI.OperationResult SaveHoliday(Holiday item, bool isNew)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveHoliday(item, isNew));
		}
		public FiresecAPI.OperationResult MarkDeletedHoliday(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedHoliday(uid, name));
		}
		public FiresecAPI.OperationResult RestoreHoliday(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreHoliday(uid, name));
		}

		public FiresecAPI.OperationResult<IEnumerable<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<ScheduleScheme>>>(() => FiresecService.GetScheduleSchemes(filter));
		}
		public FiresecAPI.OperationResult SaveScheduleScheme(ScheduleScheme item, bool isNew)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveScheduleScheme(item, isNew));
		}
		public FiresecAPI.OperationResult MarkDeletedScheduleScheme(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedScheduleScheme(uid, name));
		}
		public FiresecAPI.OperationResult RestoreScheduleScheme(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreScheduleScheme(uid, name));
		}

		public FiresecAPI.OperationResult<IEnumerable<ScheduleDayInterval>> GetSheduleDayIntervals(ScheduleDayIntervalFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<ScheduleDayInterval>>>(() => FiresecService.GetSheduleDayIntervals(filter));
		}
		public FiresecAPI.OperationResult SaveSheduleDayInterval(ScheduleDayInterval item, string name)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveSheduleDayInterval(item, name));
		}
		public FiresecAPI.OperationResult RemoveSheduleDayInterval(ScheduleDayInterval item, string name)
		{
			return SafeContext.Execute(() => FiresecService.RemoveSheduleDayInterval(item.UID, name));
		}

		public FiresecAPI.OperationResult<IEnumerable<Schedule>> GetSchedules(ScheduleFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<Schedule>>>(() => FiresecService.GetSchedules(filter));
		}
		public FiresecAPI.OperationResult SaveSchedule(Schedule item, bool isNew)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveSchedule(item, isNew));
		}
		public FiresecAPI.OperationResult MarkDeletedSchedule(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedSchedule(uid, name));
		}
		public FiresecAPI.OperationResult<IEnumerable<ShortSchedule>> GetScheduleShortList(ScheduleFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<ShortSchedule>>>(() => FiresecService.GetScheduleShortList(filter));
		}
		public FiresecAPI.OperationResult RestoreSchedule(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreSchedule(uid, name));
		}

		public FiresecAPI.OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<ScheduleZone>>>(() => FiresecService.GetScheduleZones(filter));
		}
		public FiresecAPI.OperationResult SaveScheduleZone(ScheduleZone item, string name)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveScheduleZone(item, name));
		}
		public FiresecAPI.OperationResult MarkDeletedScheduleZone(ScheduleZone item, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedScheduleZone(item.UID, name));
		}

		public FiresecAPI.OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid employeeUID, DateTime startDateTime, DateTime endDateTime)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<List<TimeTrackDocument>>>(() => FiresecService.GetTimeTrackDocument(employeeUID, startDateTime, endDateTime));
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
			return SafeContext.Execute<OperationResult<List<TimeTrackDocumentType>>>(() => FiresecService.GetTimeTrackDocumentTypes(organisationUID));
		}
		public OperationResult AddTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.AddTimeTrackDocumentType(timeTrackDocumentType));
		}
		public OperationResult EditTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.EditTimeTrackDocumentType(timeTrackDocumentType));
		}
		public OperationResult RemoveTimeTrackDocumentType(Guid timeTrackDocumentTypeUID)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RemoveTimeTrackDocumentType(timeTrackDocumentTypeUID));
		}

		public OperationResult<Dictionary<DayTimeTrackPart, List<DayTimeTrackPart>>> FindConflictIntervals(List<DayTimeTrackPart> dayTimeTrackParts, Guid employeeGuid, DateTime currentDate)
		{
			return SafeContext.Execute(() => FiresecService.FindConflictIntervals(dayTimeTrackParts, employeeGuid, currentDate));
		}

		public OperationResult<IEnumerable<DayTimeTrackPart>> GetIntersectionIntervals(
			DayTimeTrackPart currentDayTimeTrackPart, ShortEmployee currentEmployee)
		{
			return SafeContext.Execute(() => FiresecService.GetIntersectionIntervals(currentDayTimeTrackPart, currentEmployee));
		}

		public OperationResult SaveAllTimeTracks(IEnumerable<DayTimeTrackPart> collectionToSave, ShortEmployee employee, User currentUser, IEnumerable<DayTimeTrackPart> removeDayTimeTrackParts)
		{
			return SafeContext.Execute(() => FiresecService.SaveAllTimeTracks(collectionToSave, employee, currentUser, removeDayTimeTrackParts));
		}

		public OperationResult DeletePassJournal(Guid uid)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.DeletePassJournal(uid));
		}
		public OperationResult DeleteAllPassJournalItems(DayTimeTrackPart dayTimeTrackPart)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.DeleteAllPassJournalItems(dayTimeTrackPart));
		}

		public OperationResult<DateTime> GetPassJournalMinDate()
		{
			return SafeContext.Execute<OperationResult<DateTime>>(() => FiresecService.GetPassJournalMinDate());
		}
		public OperationResult<DateTime> GetCardsMinDate()
		{
			return SafeContext.Execute<OperationResult<DateTime>>(() => FiresecService.GetCardsMinDate());
		}
	}
}