using RubezhAPI;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;

namespace RubezhClient
{
	public partial class SafeFiresecService
	{
		public RubezhAPI.OperationResult<List<DayInterval>> GetDayIntervals(DayIntervalFilter filter)
		{
			return SafeOperationCall<RubezhAPI.OperationResult<List<DayInterval>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetDayIntervals(FiresecServiceFactory.UID, filter);
			}, "GetDayIntervals");
		}
		public RubezhAPI.OperationResult<bool> SaveDayInterval(DayInterval item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveDayInterval(FiresecServiceFactory.UID, item, isNew);
			}, "SaveDayInterval");
		}
		public RubezhAPI.OperationResult MarkDeletedDayInterval(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedDayInterval(FiresecServiceFactory.UID, uid, name);
			}, "MarkDeletedDayInterval");
		}
		public RubezhAPI.OperationResult RestoreDayInterval(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreDayInterval(FiresecServiceFactory.UID, uid, name);
			}, "RestoreDayInterval");
		}

		//public RubezhAPI.OperationResult<IEnumerable<DayIntervalPart>> GetDayIntervalParts(DayIntervalPartFilter filter)
		//{
		//    return SafeOperationCall<RubezhAPI.OperationResult<IEnumerable<DayIntervalPart>>>(() =>
		//{
		//	var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
		//	using (firesecService as IDisposable)
		//		return firesecService.GetDayIntervalParts(filter); }, "GetDayIntervalParts");
		//}
		//public RubezhAPI.OperationResult SaveDayIntervalPart(DayIntervalPart item, string name)
		//{
		//    return SafeOperationCall<RubezhAPI.OperationResult>(() =>
		//{
		//	var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
		//	using (firesecService as IDisposable)
		//		return firesecService.SaveDayIntervalPart(item, name); }, "SaveDayIntervalPart");
		//}
		//public RubezhAPI.OperationResult RemoveDayIntervalPart(DayIntervalPart item, string name)
		//{
		//    return SafeOperationCall(() =>
		//{
		//	var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
		//	using (firesecService as IDisposable)
		//		return firesecService.RemoveDayIntervalPart(item.UID, name); }, "RemoveDayIntervalPart");
		//}

		public RubezhAPI.OperationResult<List<Holiday>> GetHolidays(HolidayFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetHolidays(FiresecServiceFactory.UID, filter);
			}, "GetHolidays");
		}
		public RubezhAPI.OperationResult<bool> SaveHoliday(Holiday item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveHoliday(FiresecServiceFactory.UID, item, isNew);
			}, "SaveHoliday");
		}
		public RubezhAPI.OperationResult MarkDeletedHoliday(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedHoliday(FiresecServiceFactory.UID, uid, name);
			}, "MarkDeletedHoliday");
		}
		public RubezhAPI.OperationResult RestoreHoliday(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreHoliday(FiresecServiceFactory.UID, uid, name);
			}, "RestoreHoliday");
		}

		public RubezhAPI.OperationResult<List<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetScheduleSchemes(FiresecServiceFactory.UID, filter);
			}, "GetScheduleSchemes");
		}
		public RubezhAPI.OperationResult<bool> SaveScheduleScheme(ScheduleScheme item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveScheduleScheme(FiresecServiceFactory.UID, item, isNew);
			}, "SaveScheduleScheme");
		}
		public RubezhAPI.OperationResult MarkDeletedScheduleScheme(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedScheduleScheme(FiresecServiceFactory.UID, uid, name);
			}, "MarkDeletedScheduleScheme");
		}
		public RubezhAPI.OperationResult RestoreScheduleScheme(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreScheduleScheme(FiresecServiceFactory.UID, uid, name);
			}, "RestoreScheduleScheme");
		}

		//public RubezhAPI.OperationResult<IEnumerable<ScheduleDayInterval>> GetSheduleDayIntervals(ScheduleDayIntervalFilter filter)
		//{
		//    return SafeOperationCall<RubezhAPI.OperationResult<IEnumerable<ScheduleDayInterval>>>(() =>
		//{
		//	var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
		//	using (firesecService as IDisposable)
		//		return firesecService.GetSheduleDayIntervals(filter); }, "GetSheduleDayIntervals");
		//}
		//public RubezhAPI.OperationResult SaveSheduleDayInterval(ScheduleDayInterval item, string name)
		//{
		//    return SafeOperationCall<RubezhAPI.OperationResult>(() =>
		//{
		//	var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
		//	using (firesecService as IDisposable)
		//		return firesecService.SaveSheduleDayInterval(item, name); }, "SaveSheduleDayInterval");
		//}
		//public RubezhAPI.OperationResult RemoveSheduleDayInterval(ScheduleDayInterval item, string name)
		//{
		//    return SafeOperationCall(() =>
		//	{
		//		var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
		//		using (firesecService as IDisposable)
		//			return firesecService.RemoveSheduleDayInterval(item.UID, name); }, "RemoveSheduleDayInterval");
		////}

		public RubezhAPI.OperationResult<List<Schedule>> GetSchedules(ScheduleFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetSchedules(FiresecServiceFactory.UID, filter);
			}, "GetSchedules");
		}
		public RubezhAPI.OperationResult<bool> SaveSchedule(Schedule item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveSchedule(FiresecServiceFactory.UID, item, isNew);
			}, "SaveSchedule");
		}
		public RubezhAPI.OperationResult MarkDeletedSchedule(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedSchedule(FiresecServiceFactory.UID, uid, name);
			}, "MarkDeletedSchedule");
		}
		public RubezhAPI.OperationResult RestoreSchedule(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreSchedule(FiresecServiceFactory.UID, uid, name);
			}, "RestoreSchedule");
		}

		//public RubezhAPI.OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter)
		//{
		//    return SafeOperationCall<RubezhAPI.OperationResult<IEnumerable<ScheduleZone>>>(() =>
		//{
		//	var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
		//	using (firesecService as IDisposable)
		//		return firesecService.GetScheduleZones(filter); }, "GetScheduleZones");
		//}
		//public RubezhAPI.OperationResult SaveScheduleZone(ScheduleZone item, string name)
		//{
		//    return SafeOperationCall<RubezhAPI.OperationResult>(() =>
		//{
		//	var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
		//	using (firesecService as IDisposable)
		//		return firesecService.SaveScheduleZone(item, name); }, "SaveScheduleZone");
		//}
		//public RubezhAPI.OperationResult MarkDeletedScheduleZone(ScheduleZone item, string name)
		//{
		//    return SafeOperationCall(() =>
		//{
		//	var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
		//	using (firesecService as IDisposable)
		//		return firesecService.MarkDeletedScheduleZone(item.UID, name); }, "MarkDeletedScheduleZone");
		//}

		public RubezhAPI.OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid employeeUID, DateTime startDateTime, DateTime endDateTime)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetTimeTrackDocument(FiresecServiceFactory.UID, employeeUID, startDateTime, endDateTime);
			}, "GetTimeTrackDocument");
		}
		public OperationResult AddTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.AddTimeTrackDocument(FiresecServiceFactory.UID, timeTrackDocument);
			}, "AddTimeTrackDocument");
		}
		public OperationResult EditTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.EditTimeTrackDocument(FiresecServiceFactory.UID, timeTrackDocument);
			}, "EditTimeTrackDocument");
		}
		public OperationResult RemoveTimeTrackDocument(Guid timeTrackDocumentUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RemoveTimeTrackDocument(FiresecServiceFactory.UID, timeTrackDocumentUID);
			}, "RemoveTimeTrackDocument");
		}

		public OperationResult<List<TimeTrackDocumentType>> GetTimeTrackDocumentTypes(Guid organisationUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetTimeTrackDocumentTypes(FiresecServiceFactory.UID, organisationUID);
			}, "GetTimeTrackDocumentTypes");
		}
		public OperationResult AddTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.AddTimeTrackDocumentType(FiresecServiceFactory.UID, timeTrackDocumentType);
			}, "AddTimeTrackDocumentType");
		}
		public OperationResult EditTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.EditTimeTrackDocumentType(FiresecServiceFactory.UID, timeTrackDocumentType);
			}, "EditTimeTrackDocumentType");
		}
		public OperationResult RemoveTimeTrackDocumentType(Guid timeTrackDocumentTypeUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RemoveTimeTrackDocumentType(FiresecServiceFactory.UID, timeTrackDocumentTypeUID);
			}, "RemoveTimeTrackDocumentType");
		}

		public OperationResult AddCustomPassJournal(Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.AddCustomPassJournal(FiresecServiceFactory.UID, uid, employeeUID, zoneUID, enterTime, exitTime);
			}, "AddCustomPassJournal");
		}
		public OperationResult EditPassJournal(Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.EditPassJournal(FiresecServiceFactory.UID, uid, zoneUID, enterTime, exitTime);
			}, "EditPassJournal");
		}
		public OperationResult DeletePassJournal(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.DeletePassJournal(FiresecServiceFactory.UID, uid);
			}, "DeletePassJournal");
		}
		public OperationResult DeleteAllPassJournalItems(Guid uid, DateTime enterTime, DateTime exitTime)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.DeleteAllPassJournalItems(FiresecServiceFactory.UID, uid, enterTime, exitTime);
			}, "DeleteAllPassJournalItems");
		}

		public OperationResult<DateTime> GetPassJournalMinDate()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetPassJournalMinDate(FiresecServiceFactory.UID);
			}, "GetPassJournalMinDate");
		}
		public OperationResult<DateTime> GetCardsMinDate()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetCardsMinDate(FiresecServiceFactory.UID);
			}, "GetCardsMinDate");
		}
	}
}