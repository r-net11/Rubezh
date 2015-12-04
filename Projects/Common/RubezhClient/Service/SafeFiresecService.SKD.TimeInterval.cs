using Common;
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
			return SafeContext.Execute<RubezhAPI.OperationResult<List<DayInterval>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetDayIntervals(filter);
			});
		}
		public RubezhAPI.OperationResult<bool> SaveDayInterval(DayInterval item, bool isNew)
		{
			return SafeContext.Execute<RubezhAPI.OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveDayInterval(item, isNew);
			});
		}
		public RubezhAPI.OperationResult MarkDeletedDayInterval(Guid uid, string name)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedDayInterval(uid, name);
			});
		}
		public RubezhAPI.OperationResult RestoreDayInterval(Guid uid, string name)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.RestoreDayInterval(uid, name);
			});
		}

		//public RubezhAPI.OperationResult<IEnumerable<DayIntervalPart>> GetDayIntervalParts(DayIntervalPartFilter filter)
		//{
		//    return SafeContext.Execute<RubezhAPI.OperationResult<IEnumerable<DayIntervalPart>>>(() =>
		//{
		//	var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
		//	using (firesecService as IDisposable)
		//		return firesecService.GetDayIntervalParts(filter); });
		//}
		//public RubezhAPI.OperationResult SaveDayIntervalPart(DayIntervalPart item, string name)
		//{
		//    return SafeContext.Execute<RubezhAPI.OperationResult>(() =>
		//{
		//	var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
		//	using (firesecService as IDisposable)
		//		return firesecService.SaveDayIntervalPart(item, name); });
		//}
		//public RubezhAPI.OperationResult RemoveDayIntervalPart(DayIntervalPart item, string name)
		//{
		//    return SafeContext.Execute(() =>
		//{
		//	var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
		//	using (firesecService as IDisposable)
		//		return firesecService.RemoveDayIntervalPart(item.UID, name); });
		//}

		public RubezhAPI.OperationResult<List<Holiday>> GetHolidays(HolidayFilter filter)
		{
			return SafeContext.Execute<RubezhAPI.OperationResult<List<Holiday>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetHolidays(filter);
			});
		}
		public RubezhAPI.OperationResult<bool> SaveHoliday(Holiday item, bool isNew)
		{
			return SafeContext.Execute<RubezhAPI.OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveHoliday(item, isNew);
			});
		}
		public RubezhAPI.OperationResult MarkDeletedHoliday(Guid uid, string name)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedHoliday(uid, name);
			});
		}
		public RubezhAPI.OperationResult RestoreHoliday(Guid uid, string name)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.RestoreHoliday(uid, name);
			});
		}

		public RubezhAPI.OperationResult<List<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			return SafeContext.Execute<RubezhAPI.OperationResult<List<ScheduleScheme>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetScheduleSchemes(filter);
			});
		}
		public RubezhAPI.OperationResult<bool> SaveScheduleScheme(ScheduleScheme item, bool isNew)
		{
			return SafeContext.Execute<RubezhAPI.OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveScheduleScheme(item, isNew);
			});
		}
		public RubezhAPI.OperationResult MarkDeletedScheduleScheme(Guid uid, string name)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedScheduleScheme(uid, name);
			});
		}
		public RubezhAPI.OperationResult RestoreScheduleScheme(Guid uid, string name)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.RestoreScheduleScheme(uid, name);
			});
		}

		//public RubezhAPI.OperationResult<IEnumerable<ScheduleDayInterval>> GetSheduleDayIntervals(ScheduleDayIntervalFilter filter)
		//{
		//    return SafeContext.Execute<RubezhAPI.OperationResult<IEnumerable<ScheduleDayInterval>>>(() =>
		//{
		//	var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
		//	using (firesecService as IDisposable)
		//		return firesecService.GetSheduleDayIntervals(filter); });
		//}
		//public RubezhAPI.OperationResult SaveSheduleDayInterval(ScheduleDayInterval item, string name)
		//{
		//    return SafeContext.Execute<RubezhAPI.OperationResult>(() =>
		//{
		//	var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
		//	using (firesecService as IDisposable)
		//		return firesecService.SaveSheduleDayInterval(item, name); });
		//}
		//public RubezhAPI.OperationResult RemoveSheduleDayInterval(ScheduleDayInterval item, string name)
		//{
		//    return SafeContext.Execute(() =>
		//	{
		//		var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
		//		using (firesecService as IDisposable)
		//			return firesecService.RemoveSheduleDayInterval(item.UID, name); });
		////}

		public RubezhAPI.OperationResult<List<Schedule>> GetSchedules(ScheduleFilter filter)
		{
			return SafeContext.Execute<RubezhAPI.OperationResult<List<Schedule>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetSchedules(filter);
			});
		}
		public RubezhAPI.OperationResult<bool> SaveSchedule(Schedule item, bool isNew)
		{
			return SafeContext.Execute<RubezhAPI.OperationResult<bool>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.SaveSchedule(item, isNew);
			});
		}
		public RubezhAPI.OperationResult MarkDeletedSchedule(Guid uid, string name)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedSchedule(uid, name);
			});
		}
		public RubezhAPI.OperationResult RestoreSchedule(Guid uid, string name)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.RestoreSchedule(uid, name);
			});
		}

		//public RubezhAPI.OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter)
		//{
		//    return SafeContext.Execute<RubezhAPI.OperationResult<IEnumerable<ScheduleZone>>>(() =>
		//{
		//	var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
		//	using (firesecService as IDisposable)
		//		return firesecService.GetScheduleZones(filter); });
		//}
		//public RubezhAPI.OperationResult SaveScheduleZone(ScheduleZone item, string name)
		//{
		//    return SafeContext.Execute<RubezhAPI.OperationResult>(() =>
		//{
		//	var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
		//	using (firesecService as IDisposable)
		//		return firesecService.SaveScheduleZone(item, name); });
		//}
		//public RubezhAPI.OperationResult MarkDeletedScheduleZone(ScheduleZone item, string name)
		//{
		//    return SafeContext.Execute(() =>
		//{
		//	var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
		//	using (firesecService as IDisposable)
		//		return firesecService.MarkDeletedScheduleZone(item.UID, name); });
		//}

		public RubezhAPI.OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid employeeUID, DateTime startDateTime, DateTime endDateTime)
		{
			return SafeContext.Execute<RubezhAPI.OperationResult<List<TimeTrackDocument>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetTimeTrackDocument(employeeUID, startDateTime, endDateTime);
			});
		}
		public OperationResult AddTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.AddTimeTrackDocument(timeTrackDocument);
			});
		}
		public OperationResult EditTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.EditTimeTrackDocument(timeTrackDocument);
			});
		}
		public OperationResult RemoveTimeTrackDocument(Guid timeTrackDocumentUID)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.RemoveTimeTrackDocument(timeTrackDocumentUID);
			});
		}

		public OperationResult<List<TimeTrackDocumentType>> GetTimeTrackDocumentTypes(Guid organisationUID)
		{
			return SafeContext.Execute<OperationResult<List<TimeTrackDocumentType>>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetTimeTrackDocumentTypes(organisationUID);
			});
		}
		public OperationResult AddTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			return SafeContext.Execute<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.AddTimeTrackDocumentType(timeTrackDocumentType);
			});
		}
		public OperationResult EditTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			return SafeContext.Execute<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.EditTimeTrackDocumentType(timeTrackDocumentType);
			});
		}
		public OperationResult RemoveTimeTrackDocumentType(Guid timeTrackDocumentTypeUID)
		{
			return SafeContext.Execute<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.RemoveTimeTrackDocumentType(timeTrackDocumentTypeUID);
			});
		}

		public OperationResult AddCustomPassJournal(Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			return SafeContext.Execute<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.AddCustomPassJournal(uid, employeeUID, zoneUID, enterTime, exitTime);
			});
		}
		public OperationResult EditPassJournal(Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			return SafeContext.Execute<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.EditPassJournal(uid, zoneUID, enterTime, exitTime);
			});
		}
		public OperationResult DeletePassJournal(Guid uid)
		{
			return SafeContext.Execute<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.DeletePassJournal(uid);
			});
		}
		public OperationResult DeleteAllPassJournalItems(Guid uid, DateTime enterTime, DateTime exitTime)
		{
			return SafeContext.Execute<OperationResult>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.DeleteAllPassJournalItems(uid, enterTime, exitTime);
			});
		}

		public OperationResult<DateTime> GetPassJournalMinDate()
		{
			return SafeContext.Execute<OperationResult<DateTime>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetPassJournalMinDate();
			});
		}
		public OperationResult<DateTime> GetCardsMinDate()
		{
			return SafeContext.Execute<OperationResult<DateTime>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetCardsMinDate();
			});
		}
	}
}