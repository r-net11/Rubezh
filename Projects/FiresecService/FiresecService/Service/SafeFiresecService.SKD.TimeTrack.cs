using System.Threading.Tasks;
using Common;
using StrazhAPI;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;

namespace FiresecService.Service
{
	public partial class SafeFiresecService
	{
		public OperationResult<IEnumerable<DayInterval>> GetDayIntervals(DayIntervalFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<DayInterval>>>(() => FiresecService.GetDayIntervals(filter));
		}

		public OperationResult SaveDayInterval(DayInterval item, bool isNew)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveDayInterval(item, isNew));
		}

		public OperationResult MarkDeletedDayInterval(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedDayInterval(uid, name));
		}

		public OperationResult RestoreDayInterval(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestoreDayInterval(uid, name));
		}

		public OperationResult<IEnumerable<DayIntervalPart>> GetDayIntervalParts(DayIntervalPartFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<DayIntervalPart>>>(() => FiresecService.GetDayIntervalParts(filter));
		}

		public OperationResult SaveDayIntervalPart(DayIntervalPart item, bool isNew, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveDayIntervalPart(item, isNew, name));
		}

		public OperationResult RemoveDayIntervalPart(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RemoveDayIntervalPart(uid, name));
		}

		public OperationResult<IEnumerable<Holiday>> GetHolidays(HolidayFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Holiday>>>(() => FiresecService.GetHolidays(filter));
		}

		public OperationResult SaveHoliday(Holiday item, bool isNew)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveHoliday(item, isNew));
		}

		public OperationResult MarkDeletedHoliday(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedHoliday(uid, name));
		}

		public OperationResult RestoreHoliday(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestoreHoliday(uid, name));
		}

		public OperationResult<IEnumerable<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<ScheduleScheme>>>(() => FiresecService.GetScheduleSchemes(filter));
		}

		public OperationResult SaveScheduleScheme(ScheduleScheme item, bool isNew)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveScheduleScheme(item, isNew));
		}

		public OperationResult MarkDeletedScheduleScheme(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedScheduleScheme(uid, name));
		}

		public OperationResult RestoreScheduleScheme(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestoreScheduleScheme(uid, name));
		}

		public OperationResult<IEnumerable<ScheduleDayInterval>> GetSheduleDayIntervals(ScheduleDayIntervalFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<ScheduleDayInterval>>>(() => FiresecService.GetSheduleDayIntervals(filter));
		}

		public OperationResult SaveSheduleDayInterval(ScheduleDayInterval item, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveSheduleDayInterval(item, name));
		}

		public OperationResult RemoveSheduleDayInterval(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RemoveSheduleDayInterval(uid, name));
		}

		public OperationResult<IEnumerable<Schedule>> GetSchedules(ScheduleFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<Schedule>>>(() => FiresecService.GetSchedules(filter));
		}

		public OperationResult SaveSchedule(Schedule item, bool isNew)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveSchedule(item, isNew));
		}

		public OperationResult MarkDeletedSchedule(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedSchedule(uid, name));
		}

		public OperationResult<IEnumerable<ShortSchedule>> GetScheduleShortList(ScheduleFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<ShortSchedule>>>(() => FiresecService.GetScheduleShortList(filter));
		}

		public OperationResult RestoreSchedule(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RestoreSchedule(uid, name));
		}

		public OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter)
		{
			return SafeContext.Execute<OperationResult<IEnumerable<ScheduleZone>>>(() => FiresecService.GetScheduleZones(filter));
		}

		public OperationResult SaveScheduleZone(ScheduleZone item, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.SaveScheduleZone(item, name));
		}

		public OperationResult MarkDeletedScheduleZone(Guid uid, string name)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.MarkDeletedScheduleZone(uid, name));
		}

		public OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid employeeUID, DateTime startDateTime, DateTime endDateTime)
		{
			return SafeContext.Execute(() => FiresecService.GetTimeTrackDocument(employeeUID, startDateTime, endDateTime));
		}

		public OperationResult AddTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.AddTimeTrackDocument(timeTrackDocument));
		}

		public OperationResult EditTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.EditTimeTrackDocument(timeTrackDocument));
		}

		public OperationResult RemoveTimeTrackDocument(Guid timeTrackDocumentUID)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RemoveTimeTrackDocument(timeTrackDocumentUID));
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

		public OperationResult CheckDocumentType(TimeTrackDocumentType timeTrackDocumentType, Guid organisationUID)
		{
			return SafeContext.Execute(() => FiresecService.CheckDocumentType(timeTrackDocumentType, organisationUID));
		}

		public OperationResult RemoveTimeTrackDocumentType(Guid timeTrackDocumentTypeUID)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RemoveTimeTrackDocumentType(timeTrackDocumentTypeUID));
		}

		public OperationResult<bool> CheckForCanForseCloseInterval(Guid openedIntervalGuid)
		{
			return SafeContext.Execute(() => FiresecService.CheckForCanForseCloseInterval(openedIntervalGuid));
		}

		public OperationResult SaveAllTimeTracks(IEnumerable<DayTimeTrackPart> collectionToSave, ShortEmployee employee, User currentUser, IEnumerable<DayTimeTrackPart> removedDayTimeTrackParts)
		{
			return SafeContext.Execute(() => FiresecService.SaveAllTimeTracks(collectionToSave, employee, currentUser, removedDayTimeTrackParts));
		}

		public OperationResult DeletePassJournal(Guid uid)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.DeletePassJournal(uid));
		}

		public OperationResult DeleteAllPassJournalItems(DayTimeTrackPart dayTimeTrackPart)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.DeleteAllPassJournalItems(dayTimeTrackPart));
		}

		public OperationResult<IEnumerable<DayTimeTrackPart>> GetIntersectionIntervals(
			DayTimeTrackPart currentDayTimeTrackPart,
			ShortEmployee currentEmployee)
		{
			return SafeContext.Execute(() => FiresecService.GetIntersectionIntervals(currentDayTimeTrackPart, currentEmployee));
		}

		public OperationResult<DateTime> GetPassJournalMinDate()
		{
			return SafeContext.Execute<OperationResult<DateTime>>(() => FiresecService.GetPassJournalMinDate());
		}

		public OperationResult<DateTime> GetJournalMinDate()
		{
			return SafeContext.Execute<OperationResult<DateTime>>(() => FiresecService.GetJournalMinDate());
		}

		public OperationResult<DateTime> GetCardsMinDate()
		{
			return SafeContext.Execute<OperationResult<DateTime>>(() => FiresecService.GetCardsMinDate());
		}
	}
}