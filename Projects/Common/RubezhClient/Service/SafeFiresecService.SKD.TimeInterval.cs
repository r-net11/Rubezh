using System;
using System.Collections.Generic;
using Common;
using RubezhAPI;
using RubezhAPI.SKD;

namespace RubezhClient
{
	public partial class SafeFiresecService
	{
		public RubezhAPI.OperationResult<List<DayInterval>> GetDayIntervals(DayIntervalFilter filter)
		{
			return SafeContext.Execute<RubezhAPI.OperationResult<List<DayInterval>>>(() => FiresecService.GetDayIntervals(filter));
		}
		public RubezhAPI.OperationResult<bool> SaveDayInterval(DayInterval item, bool isNew)
		{
			return SafeContext.Execute<RubezhAPI.OperationResult<bool>>(() => FiresecService.SaveDayInterval(item, isNew));
		}
		public RubezhAPI.OperationResult MarkDeletedDayInterval(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedDayInterval(uid, name));
		}
		public RubezhAPI.OperationResult RestoreDayInterval(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreDayInterval(uid, name));
		}
	
		//public RubezhAPI.OperationResult<IEnumerable<DayIntervalPart>> GetDayIntervalParts(DayIntervalPartFilter filter)
		//{
		//    return SafeContext.Execute<RubezhAPI.OperationResult<IEnumerable<DayIntervalPart>>>(() => FiresecService.GetDayIntervalParts(filter));
		//}
		//public RubezhAPI.OperationResult SaveDayIntervalPart(DayIntervalPart item, string name)
		//{
		//    return SafeContext.Execute<RubezhAPI.OperationResult>(() => FiresecService.SaveDayIntervalPart(item, name));
		//}
		//public RubezhAPI.OperationResult RemoveDayIntervalPart(DayIntervalPart item, string name)
		//{
		//    return SafeContext.Execute(() => FiresecService.RemoveDayIntervalPart(item.UID, name));
		//}

		public RubezhAPI.OperationResult<List<Holiday>> GetHolidays(HolidayFilter filter)
		{
			return SafeContext.Execute<RubezhAPI.OperationResult<List<Holiday>>>(() => FiresecService.GetHolidays(filter));
		}
		public RubezhAPI.OperationResult<bool> SaveHoliday(Holiday item, bool isNew)
		{
			return SafeContext.Execute<RubezhAPI.OperationResult<bool>>(() => FiresecService.SaveHoliday(item, isNew));
		}
		public RubezhAPI.OperationResult MarkDeletedHoliday(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedHoliday(uid, name));
		}
		public RubezhAPI.OperationResult RestoreHoliday(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreHoliday(uid, name));
		}

		public RubezhAPI.OperationResult<List<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			return SafeContext.Execute<RubezhAPI.OperationResult<List<ScheduleScheme>>>(() => FiresecService.GetScheduleSchemes(filter));
		}
		public RubezhAPI.OperationResult<bool> SaveScheduleScheme(ScheduleScheme item, bool isNew)
		{
			return SafeContext.Execute<RubezhAPI.OperationResult<bool>>(() => FiresecService.SaveScheduleScheme(item, isNew));
		}
		public RubezhAPI.OperationResult MarkDeletedScheduleScheme(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedScheduleScheme(uid, name));
		}
		public RubezhAPI.OperationResult RestoreScheduleScheme(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreScheduleScheme(uid, name));
		}

		//public RubezhAPI.OperationResult<IEnumerable<ScheduleDayInterval>> GetSheduleDayIntervals(ScheduleDayIntervalFilter filter)
		//{
		//    return SafeContext.Execute<RubezhAPI.OperationResult<IEnumerable<ScheduleDayInterval>>>(() => FiresecService.GetSheduleDayIntervals(filter));
		//}
		//public RubezhAPI.OperationResult SaveSheduleDayInterval(ScheduleDayInterval item, string name)
		//{
		//    return SafeContext.Execute<RubezhAPI.OperationResult>(() => FiresecService.SaveSheduleDayInterval(item, name));
		//}
		//public RubezhAPI.OperationResult RemoveSheduleDayInterval(ScheduleDayInterval item, string name)
		//{
		//    return SafeContext.Execute(() => FiresecService.RemoveSheduleDayInterval(item.UID, name));
		//}

		public RubezhAPI.OperationResult<List<Schedule>> GetSchedules(ScheduleFilter filter)
		{
			return SafeContext.Execute<RubezhAPI.OperationResult<List<Schedule>>>(() => FiresecService.GetSchedules(filter));
		}
		public RubezhAPI.OperationResult<bool> SaveSchedule(Schedule item, bool isNew)
		{
			return SafeContext.Execute<RubezhAPI.OperationResult<bool>>(() => FiresecService.SaveSchedule(item, isNew));
		}
		public RubezhAPI.OperationResult MarkDeletedSchedule(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedSchedule(uid, name));
		}
		public RubezhAPI.OperationResult RestoreSchedule(Guid uid, string name)
		{
			return SafeContext.Execute(() => FiresecService.RestoreSchedule(uid, name));
		}

		//public RubezhAPI.OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter)
		//{
		//    return SafeContext.Execute<RubezhAPI.OperationResult<IEnumerable<ScheduleZone>>>(() => FiresecService.GetScheduleZones(filter));
		//}
		//public RubezhAPI.OperationResult SaveScheduleZone(ScheduleZone item, string name)
		//{
		//    return SafeContext.Execute<RubezhAPI.OperationResult>(() => FiresecService.SaveScheduleZone(item, name));
		//}
		//public RubezhAPI.OperationResult MarkDeletedScheduleZone(ScheduleZone item, string name)
		//{
		//    return SafeContext.Execute(() => FiresecService.MarkDeletedScheduleZone(item.UID, name));
		//}

		public RubezhAPI.OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid employeeUID, DateTime startDateTime, DateTime endDateTime)
		{
			return SafeContext.Execute<RubezhAPI.OperationResult<List<TimeTrackDocument>>>(() => FiresecService.GetTimeTrackDocument(employeeUID, startDateTime, endDateTime));
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

		public OperationResult AddCustomPassJournal(Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.AddCustomPassJournal(uid, employeeUID, zoneUID, enterTime, exitTime));
		}
		public OperationResult EditPassJournal(Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.EditPassJournal(uid, zoneUID, enterTime, exitTime));
		}
		public OperationResult DeletePassJournal(Guid uid)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.DeletePassJournal(uid));
		}
		public OperationResult DeleteAllPassJournalItems(Guid uid, DateTime enterTime, DateTime exitTime)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.DeleteAllPassJournalItems(uid, enterTime, exitTime));
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