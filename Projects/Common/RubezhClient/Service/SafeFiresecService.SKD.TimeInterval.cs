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
					return firesecService.GetDayIntervals(filter, FiresecServiceFactory.UID);
			}, "GetDayIntervals");
		}
		public RubezhAPI.OperationResult<bool> SaveDayInterval(DayInterval item, bool isNew)
		{
			return SafeOperationCall<RubezhAPI.OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveDayInterval(item, isNew, FiresecServiceFactory.UID);
			}, "SaveDayInterval");
		}
		public RubezhAPI.OperationResult MarkDeletedDayInterval(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedDayInterval(uid, name, FiresecServiceFactory.UID);
			}, "MarkDeletedDayInterval");
		}
		public RubezhAPI.OperationResult RestoreDayInterval(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreDayInterval(uid, name, FiresecServiceFactory.UID);
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
			return SafeOperationCall<RubezhAPI.OperationResult<List<Holiday>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetHolidays(filter, FiresecServiceFactory.UID);
			}, "GetHolidays");
		}
		public RubezhAPI.OperationResult<bool> SaveHoliday(Holiday item, bool isNew)
		{
			return SafeOperationCall<RubezhAPI.OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveHoliday(item, isNew, FiresecServiceFactory.UID);
			}, "SaveHoliday");
		}
		public RubezhAPI.OperationResult MarkDeletedHoliday(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedHoliday(uid, name, FiresecServiceFactory.UID);
			}, "MarkDeletedHoliday");
		}
		public RubezhAPI.OperationResult RestoreHoliday(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreHoliday(uid, name, FiresecServiceFactory.UID);
			}, "RestoreHoliday");
		}

		public RubezhAPI.OperationResult<List<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			return SafeOperationCall<RubezhAPI.OperationResult<List<ScheduleScheme>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetScheduleSchemes(filter, FiresecServiceFactory.UID);
			}, "GetScheduleSchemes");
		}
		public RubezhAPI.OperationResult<bool> SaveScheduleScheme(ScheduleScheme item, bool isNew)
		{
			return SafeOperationCall<RubezhAPI.OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveScheduleScheme(item, isNew, FiresecServiceFactory.UID);
			}, "SaveScheduleScheme");
		}
		public RubezhAPI.OperationResult MarkDeletedScheduleScheme(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedScheduleScheme(uid, name, FiresecServiceFactory.UID);
			}, "MarkDeletedScheduleScheme");
		}
		public RubezhAPI.OperationResult RestoreScheduleScheme(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreScheduleScheme(uid, name, FiresecServiceFactory.UID);
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
			return SafeOperationCall<RubezhAPI.OperationResult<List<Schedule>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetSchedules(filter, FiresecServiceFactory.UID);
			}, "GetSchedules");
		}
		public RubezhAPI.OperationResult<bool> SaveSchedule(Schedule item, bool isNew)
		{
			return SafeOperationCall<RubezhAPI.OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.SaveSchedule(item, isNew, FiresecServiceFactory.UID);
			}, "SaveSchedule");
		}
		public RubezhAPI.OperationResult MarkDeletedSchedule(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedSchedule(uid, name, FiresecServiceFactory.UID);
			}, "MarkDeletedSchedule");
		}
		public RubezhAPI.OperationResult RestoreSchedule(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreSchedule(uid, name, FiresecServiceFactory.UID);
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
			return SafeOperationCall<RubezhAPI.OperationResult<List<TimeTrackDocument>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetTimeTrackDocument(employeeUID, startDateTime, endDateTime, FiresecServiceFactory.UID);
			}, "GetTimeTrackDocument");
		}
		public OperationResult AddTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.AddTimeTrackDocument(timeTrackDocument, FiresecServiceFactory.UID);
			}, "AddTimeTrackDocument");
		}
		public OperationResult EditTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.EditTimeTrackDocument(timeTrackDocument, FiresecServiceFactory.UID);
			}, "EditTimeTrackDocument");
		}
		public OperationResult RemoveTimeTrackDocument(Guid timeTrackDocumentUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RemoveTimeTrackDocument(timeTrackDocumentUID, FiresecServiceFactory.UID);
			}, "RemoveTimeTrackDocument");
		}

		public OperationResult<List<TimeTrackDocumentType>> GetTimeTrackDocumentTypes(Guid organisationUID)
		{
			return SafeOperationCall<OperationResult<List<TimeTrackDocumentType>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetTimeTrackDocumentTypes(organisationUID, FiresecServiceFactory.UID);
			}, "GetTimeTrackDocumentTypes");
		}
		public OperationResult AddTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.AddTimeTrackDocumentType(timeTrackDocumentType, FiresecServiceFactory.UID);
			}, "AddTimeTrackDocumentType");
		}
		public OperationResult EditTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.EditTimeTrackDocumentType(timeTrackDocumentType, FiresecServiceFactory.UID);
			}, "EditTimeTrackDocumentType");
		}
		public OperationResult RemoveTimeTrackDocumentType(Guid timeTrackDocumentTypeUID)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RemoveTimeTrackDocumentType(timeTrackDocumentTypeUID, FiresecServiceFactory.UID);
			}, "RemoveTimeTrackDocumentType");
		}

		public OperationResult AddCustomPassJournal(Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.AddCustomPassJournal(uid, employeeUID, zoneUID, enterTime, exitTime, FiresecServiceFactory.UID);
			}, "AddCustomPassJournal");
		}
		public OperationResult EditPassJournal(Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.EditPassJournal(uid, zoneUID, enterTime, exitTime, FiresecServiceFactory.UID);
			}, "EditPassJournal");
		}
		public OperationResult DeletePassJournal(Guid uid)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.DeletePassJournal(uid, FiresecServiceFactory.UID);
			}, "DeletePassJournal");
		}
		public OperationResult DeleteAllPassJournalItems(Guid uid, DateTime enterTime, DateTime exitTime)
		{
			return SafeOperationCall<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.DeleteAllPassJournalItems(uid, enterTime, exitTime, FiresecServiceFactory.UID);
			}, "DeleteAllPassJournalItems");
		}

		public OperationResult<DateTime> GetPassJournalMinDate()
		{
			return SafeOperationCall<OperationResult<DateTime>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetPassJournalMinDate(FiresecServiceFactory.UID);
			}, "GetPassJournalMinDate");
		}
		public OperationResult<DateTime> GetCardsMinDate()
		{
			return SafeOperationCall<OperationResult<DateTime>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetCardsMinDate(FiresecServiceFactory.UID);
			}, "GetCardsMinDate");
		}
	}
}