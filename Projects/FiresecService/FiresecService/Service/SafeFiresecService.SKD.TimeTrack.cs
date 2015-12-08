using RubezhAPI;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;

namespace FiresecService.Service
{
	public partial class SafeFiresecService
	{
		public OperationResult<List<DayInterval>> GetDayIntervals(DayIntervalFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetDayIntervals(filter, clientUID), "GetDayIntervals", clientUID);
		}
		public OperationResult<bool> SaveDayInterval(DayInterval item, bool isNew, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveDayInterval(item, isNew, clientUID), "SaveDayInterval", clientUID);
		}
		public OperationResult MarkDeletedDayInterval(Guid uid, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.MarkDeletedDayInterval(uid, name, clientUID), "MarkDeletedDayInterval", clientUID);
		}
		public OperationResult RestoreDayInterval(Guid uid, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.RestoreDayInterval(uid, name, clientUID), "RestoreDayInterval", clientUID);
		}

		//public OperationResult<IEnumerable<DayIntervalPart>> GetDayIntervalParts(DayIntervalPartFilter filter)
		//{
		//    return SafeOperationCall<OperationResult<IEnumerable<DayIntervalPart>>>(() => FiresecService.GetDayIntervalParts(filter));
		//}
		//public OperationResult SaveDayIntervalPart(DayIntervalPart item, string name)
		//{
		//    return SafeOperationCall(() => FiresecService.SaveDayIntervalPart(item, name));
		//}
		//public OperationResult RemoveDayIntervalPart(Guid uid, string name)
		//{
		//    return SafeOperationCall(() => FiresecService.RemoveDayIntervalPart(uid, name));
		//}

		public OperationResult<List<Holiday>> GetHolidays(HolidayFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetHolidays(filter, clientUID), "GetHolidays", clientUID);
		}
		public OperationResult<bool> SaveHoliday(Holiday item, bool isNew, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveHoliday(item, isNew, clientUID), "SaveHoliday", clientUID);
		}
		public OperationResult MarkDeletedHoliday(Guid uid, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.MarkDeletedHoliday(uid, name, clientUID), "MarkDeletedHoliday", clientUID);
		}
		public OperationResult RestoreHoliday(Guid uid, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.RestoreHoliday(uid, name, clientUID), "RestoreHoliday", clientUID);
		}

		public OperationResult<List<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetScheduleSchemes(filter, clientUID), "GetScheduleSchemes", clientUID);
		}
		public OperationResult<bool> SaveScheduleScheme(ScheduleScheme item, bool isNew, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveScheduleScheme(item, isNew, clientUID), "SaveScheduleScheme", clientUID);
		}
		public OperationResult MarkDeletedScheduleScheme(Guid uid, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.MarkDeletedScheduleScheme(uid, name, clientUID), "MarkDeletedScheduleScheme", clientUID);
		}
		public OperationResult RestoreScheduleScheme(Guid uid, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.RestoreScheduleScheme(uid, name, clientUID), "RestoreScheduleScheme", clientUID);
		}

		//public OperationResult<IEnumerable<ScheduleDayInterval>> GetSheduleDayIntervals(ScheduleDayIntervalFilter filter)
		//{
		//    return SafeOperationCall<OperationResult<IEnumerable<ScheduleDayInterval>>>(() => FiresecService.GetSheduleDayIntervals(filter));
		//}
		//public OperationResult SaveSheduleDayInterval(ScheduleDayInterval item, string name)
		//{
		//    return SafeOperationCall(() => FiresecService.SaveSheduleDayInterval(item, name));
		//}
		//public OperationResult RemoveSheduleDayInterval(Guid uid, string name)
		//{
		//    return SafeOperationCall(() => FiresecService.RemoveSheduleDayInterval(uid, name));
		//}

		public OperationResult<List<Schedule>> GetSchedules(ScheduleFilter filter, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetSchedules(filter, clientUID), "GetSchedules", clientUID);
		}
		public OperationResult<bool> SaveSchedule(Schedule item, bool isNew, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.SaveSchedule(item, isNew, clientUID), "SaveSchedule", clientUID);
		}
		public OperationResult MarkDeletedSchedule(Guid uid, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.MarkDeletedSchedule(uid, name, clientUID), "MarkDeletedSchedule", clientUID);
		}
		public OperationResult RestoreSchedule(Guid uid, string name, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.RestoreSchedule(uid, name, clientUID), "RestoreSchedule", clientUID);
		}

		//public OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter)
		//{
		//    return SafeOperationCall<OperationResult<IEnumerable<ScheduleZone>>>(() => FiresecService.GetScheduleZones(filter));
		//}
		//public OperationResult SaveScheduleZone(ScheduleZone item, string name)
		//{
		//    return SafeOperationCall(() => FiresecService.SaveScheduleZone(item, name));
		//}
		//public OperationResult MarkDeletedScheduleZone(Guid uid, string name)
		//{
		//    return SafeOperationCall(() => FiresecService.MarkDeletedScheduleZone(uid, name));
		//}

		public OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid employeeUID, DateTime startDateTime, DateTime endDateTime, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetTimeTrackDocument(employeeUID, startDateTime, endDateTime, clientUID), "GetTimeTrackDocument", clientUID);
		}
		public OperationResult AddTimeTrackDocument(TimeTrackDocument timeTrackDocument, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.AddTimeTrackDocument(timeTrackDocument, clientUID), "AddTimeTrackDocument", clientUID);
		}
		public OperationResult EditTimeTrackDocument(TimeTrackDocument timeTrackDocument, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.EditTimeTrackDocument(timeTrackDocument, clientUID), "EditTimeTrackDocument", clientUID);
		}
		public OperationResult RemoveTimeTrackDocument(Guid timeTrackDocumentUID, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.RemoveTimeTrackDocument(timeTrackDocumentUID, clientUID), "RemoveTimeTrackDocument", clientUID);
		}

		public OperationResult<List<TimeTrackDocumentType>> GetTimeTrackDocumentTypes(Guid organisationUID, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetTimeTrackDocumentTypes(organisationUID, clientUID), "GetTimeTrackDocumentTypes", clientUID);
		}
		public OperationResult AddTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.AddTimeTrackDocumentType(timeTrackDocumentType, clientUID), "AddTimeTrackDocumentType", clientUID);
		}
		public OperationResult EditTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.EditTimeTrackDocumentType(timeTrackDocumentType, clientUID), "EditTimeTrackDocumentType", clientUID);
		}
		public OperationResult RemoveTimeTrackDocumentType(Guid timeTrackDocumentTypeUID, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.RemoveTimeTrackDocumentType(timeTrackDocumentTypeUID, clientUID), "RemoveTimeTrackDocumentType", clientUID);
		}

		public OperationResult AddCustomPassJournal(Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.AddCustomPassJournal(uid, employeeUID, zoneUID, enterTime, exitTime, clientUID), "AddCustomPassJournal", clientUID);
		}
		public OperationResult EditPassJournal(Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.EditPassJournal(uid, zoneUID, enterTime, exitTime, clientUID), "EditPassJournal", clientUID);
		}
		public OperationResult DeletePassJournal(Guid uid, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.DeletePassJournal(uid, clientUID), "DeletePassJournal", clientUID);
		}
		public OperationResult DeleteAllPassJournalItems(Guid uid, DateTime enterTime, DateTime exitTime, Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.DeleteAllPassJournalItems(uid, enterTime, exitTime, clientUID), "DeleteAllPassJournalItems", clientUID);
		}

		public OperationResult<DateTime> GetPassJournalMinDate(Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetPassJournalMinDate(clientUID), "GetPassJournalMinDate", clientUID);
		}
		public OperationResult<DateTime> GetJournalMinDate(Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetJournalMinDate(clientUID), "GetJournalMinDate", clientUID);
		}
		public OperationResult<DateTime> GetCardsMinDate(Guid clientUID)
		{
			return SafeOperationCall(() => FiresecService.GetCardsMinDate(clientUID), "GetCardsMinDate", clientUID);
		}
	}
}