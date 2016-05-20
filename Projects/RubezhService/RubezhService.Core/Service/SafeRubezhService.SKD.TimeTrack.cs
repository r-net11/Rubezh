using RubezhAPI;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;

namespace RubezhService.Service
{
	public partial class SafeRubezhService
	{
		public OperationResult<List<DayInterval>> GetDayIntervals(Guid clientUID, DayIntervalFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetDayIntervals(clientUID, filter), "GetDayIntervals");
		}
		public OperationResult<bool> SaveDayInterval(Guid clientUID, DayInterval item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveDayInterval(clientUID, item, isNew), "SaveDayInterval");
		}
		public OperationResult<bool> MarkDeletedDayInterval(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.MarkDeletedDayInterval(clientUID, uid, name), "MarkDeletedDayInterval");
		}
		public OperationResult<bool> RestoreDayInterval(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.RestoreDayInterval(clientUID, uid, name), "RestoreDayInterval");
		}

		//public OperationResult<IEnumerable<DayIntervalPart>> GetDayIntervalParts(DayIntervalPartFilter filter)
		//{
		//    return SafeOperationCall<OperationResult<IEnumerable<DayIntervalPart>>>(() => RubezhService.GetDayIntervalParts(filter));
		//}
		//public OperationResult SaveDayIntervalPart(DayIntervalPart item, string name)
		//{
		//    return SafeOperationCall(clientUID, () => RubezhService.SaveDayIntervalPart(item, name));
		//}
		//public OperationResult RemoveDayIntervalPart(Guid uid, string name)
		//{
		//    return SafeOperationCall(clientUID, () => RubezhService.RemoveDayIntervalPart(uid, name));
		//}

		public OperationResult<List<Holiday>> GetHolidays(Guid clientUID, HolidayFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetHolidays(clientUID, filter), "GetHolidays");
		}
		public OperationResult<bool> SaveHoliday(Guid clientUID, Holiday item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveHoliday(clientUID, item, isNew), "SaveHoliday");
		}
		public OperationResult<bool> MarkDeletedHoliday(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.MarkDeletedHoliday(clientUID, uid, name), "MarkDeletedHoliday");
		}
		public OperationResult<bool> RestoreHoliday(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.RestoreHoliday(clientUID, uid, name), "RestoreHoliday");
		}

		public OperationResult<List<ScheduleScheme>> GetScheduleSchemes(Guid clientUID, ScheduleSchemeFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetScheduleSchemes(clientUID, filter), "GetScheduleSchemes");
		}
		public OperationResult<bool> SaveScheduleScheme(Guid clientUID, ScheduleScheme item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveScheduleScheme(clientUID, item, isNew), "SaveScheduleScheme");
		}
		public OperationResult<bool> MarkDeletedScheduleScheme(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.MarkDeletedScheduleScheme(clientUID, uid, name), "MarkDeletedScheduleScheme");
		}
		public OperationResult<bool> RestoreScheduleScheme(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.RestoreScheduleScheme(clientUID, uid, name), "RestoreScheduleScheme");
		}

		//public OperationResult<IEnumerable<ScheduleDayInterval>> GetSheduleDayIntervals(ScheduleDayIntervalFilter filter)
		//{
		//    return SafeOperationCall<OperationResult<IEnumerable<ScheduleDayInterval>>>(() => RubezhService.GetSheduleDayIntervals(filter));
		//}
		//public OperationResult SaveSheduleDayInterval(ScheduleDayInterval item, string name)
		//{
		//    return SafeOperationCall(clientUID, () => RubezhService.SaveSheduleDayInterval(item, name));
		//}
		//public OperationResult RemoveSheduleDayInterval(Guid uid, string name)
		//{
		//    return SafeOperationCall(clientUID, () => RubezhService.RemoveSheduleDayInterval(uid, name));
		//}

		public OperationResult<List<Schedule>> GetSchedules(Guid clientUID, ScheduleFilter filter)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetSchedules(clientUID, filter), "GetSchedules");
		}
		public OperationResult<bool> SaveSchedule(Guid clientUID, Schedule item, bool isNew)
		{
			return SafeOperationCall(clientUID, () => RubezhService.SaveSchedule(clientUID, item, isNew), "SaveSchedule");
		}
		public OperationResult<bool> MarkDeletedSchedule(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.MarkDeletedSchedule(clientUID, uid, name), "MarkDeletedSchedule");
		}
		public OperationResult<bool> RestoreSchedule(Guid clientUID, Guid uid, string name)
		{
			return SafeOperationCall(clientUID, () => RubezhService.RestoreSchedule(clientUID, uid, name), "RestoreSchedule");
		}

		//public OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter)
		//{
		//    return SafeOperationCall<OperationResult<IEnumerable<ScheduleZone>>>(() => RubezhService.GetScheduleZones(filter));
		//}
		//public OperationResult SaveScheduleZone(ScheduleZone item, string name)
		//{
		//    return SafeOperationCall(clientUID, () => RubezhService.SaveScheduleZone(item, name));
		//}
		//public OperationResult MarkDeletedScheduleZone(Guid uid, string name)
		//{
		//    return SafeOperationCall(clientUID, () => RubezhService.MarkDeletedScheduleZone(uid, name));
		//}

		public OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid clientUID, Guid employeeUID, DateTime startDateTime, DateTime endDateTime)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetTimeTrackDocument(clientUID, employeeUID, startDateTime, endDateTime), "GetTimeTrackDocument");
		}
		public OperationResult<bool> AddTimeTrackDocument(Guid clientUID, TimeTrackDocument timeTrackDocument)
		{
			return SafeOperationCall(clientUID, () => RubezhService.AddTimeTrackDocument(clientUID, timeTrackDocument), "AddTimeTrackDocument");
		}
		public OperationResult<bool> EditTimeTrackDocument(Guid clientUID, TimeTrackDocument timeTrackDocument)
		{
			return SafeOperationCall(clientUID, () => RubezhService.EditTimeTrackDocument(clientUID, timeTrackDocument), "EditTimeTrackDocument");
		}
		public OperationResult<bool> RemoveTimeTrackDocument(Guid clientUID, Guid timeTrackDocumentUID)
		{
			return SafeOperationCall(clientUID, () => RubezhService.RemoveTimeTrackDocument(clientUID, timeTrackDocumentUID), "RemoveTimeTrackDocument");
		}

		public OperationResult<List<TimeTrackDocumentType>> GetTimeTrackDocumentTypes(Guid clientUID, Guid organisationUID)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetTimeTrackDocumentTypes(clientUID, organisationUID), "GetTimeTrackDocumentTypes");
		}
		public OperationResult<bool> AddTimeTrackDocumentType(Guid clientUID, TimeTrackDocumentType timeTrackDocumentType)
		{
			return SafeOperationCall(clientUID, 
				() => RubezhService.AddTimeTrackDocumentType(clientUID, timeTrackDocumentType), "AddTimeTrackDocumentType");
		}
		public OperationResult<bool> EditTimeTrackDocumentType(Guid clientUID, TimeTrackDocumentType timeTrackDocumentType)
		{
			return SafeOperationCall(clientUID, () => RubezhService.EditTimeTrackDocumentType(clientUID, timeTrackDocumentType), "EditTimeTrackDocumentType");
		}
		public OperationResult<bool> RemoveTimeTrackDocumentType(Guid clientUID, Guid timeTrackDocumentTypeUID)
		{
			return SafeOperationCall(clientUID, () => RubezhService.RemoveTimeTrackDocumentType(clientUID, timeTrackDocumentTypeUID), "RemoveTimeTrackDocumentType");
		}

		public OperationResult<bool> AddCustomPassJournal(Guid clientUID, Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			return SafeOperationCall(clientUID, () => RubezhService.AddCustomPassJournal(clientUID, uid, employeeUID, zoneUID, enterTime, exitTime), "AddCustomPassJournal");
		}
		public OperationResult<bool> EditPassJournal(Guid clientUID, Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			return SafeOperationCall(clientUID, () => RubezhService.EditPassJournal(clientUID, uid, zoneUID, enterTime, exitTime), "EditPassJournal");
		}
		public OperationResult<bool> DeletePassJournal(Guid clientUID, Guid uid)
		{
			return SafeOperationCall(clientUID, () => RubezhService.DeletePassJournal(clientUID, uid), "DeletePassJournal");
		}
		public OperationResult<bool> DeleteAllPassJournalItems(Guid clientUID, Guid uid, DateTime enterTime, DateTime exitTime)
		{
			return SafeOperationCall(clientUID, () => RubezhService.DeleteAllPassJournalItems(clientUID, uid, enterTime, exitTime), "DeleteAllPassJournalItems");
		}

		public OperationResult<DateTime> GetPassJournalMinDate(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetPassJournalMinDate(clientUID), "GetPassJournalMinDate");
		}
		public OperationResult<DateTime> GetJournalMinDate(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetJournalMinDate(clientUID), "GetJournalMinDate");
		}
		public OperationResult<DateTime> GetCardsMinDate(Guid clientUID)
		{
			return SafeOperationCall(clientUID, () => RubezhService.GetCardsMinDate(clientUID), "GetCardsMinDate");
		}
	}
}