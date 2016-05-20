using RubezhAPI;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;

namespace RubezhClient
{
	public partial class SafeRubezhService
	{
		public RubezhAPI.OperationResult<List<DayInterval>> GetDayIntervals(DayIntervalFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetDayIntervals(RubezhServiceFactory.UID, filter);
			}, "GetDayIntervals");
		}
		public RubezhAPI.OperationResult<bool> SaveDayInterval(DayInterval item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveDayInterval(RubezhServiceFactory.UID, item, isNew);
			}, "SaveDayInterval");
		}
		public RubezhAPI.OperationResult<bool> MarkDeletedDayInterval(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.MarkDeletedDayInterval(RubezhServiceFactory.UID, uid, name);
			}, "MarkDeletedDayInterval");
		}
		public RubezhAPI.OperationResult<bool> RestoreDayInterval(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.RestoreDayInterval(RubezhServiceFactory.UID, uid, name);
			}, "RestoreDayInterval");
		}

		public RubezhAPI.OperationResult<List<Holiday>> GetHolidays(HolidayFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetHolidays(RubezhServiceFactory.UID, filter);
			}, "GetHolidays");
		}
		public RubezhAPI.OperationResult<bool> SaveHoliday(Holiday item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveHoliday(RubezhServiceFactory.UID, item, isNew);
			}, "SaveHoliday");
		}
		public RubezhAPI.OperationResult<bool> MarkDeletedHoliday(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.MarkDeletedHoliday(RubezhServiceFactory.UID, uid, name);
			}, "MarkDeletedHoliday");
		}
		public RubezhAPI.OperationResult<bool> RestoreHoliday(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.RestoreHoliday(RubezhServiceFactory.UID, uid, name);
			}, "RestoreHoliday");
		}

		public RubezhAPI.OperationResult<List<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetScheduleSchemes(RubezhServiceFactory.UID, filter);
			}, "GetScheduleSchemes");
		}
		public RubezhAPI.OperationResult<bool> SaveScheduleScheme(ScheduleScheme item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveScheduleScheme(RubezhServiceFactory.UID, item, isNew);
			}, "SaveScheduleScheme");
		}
		public RubezhAPI.OperationResult<bool> MarkDeletedScheduleScheme(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.MarkDeletedScheduleScheme(RubezhServiceFactory.UID, uid, name);
			}, "MarkDeletedScheduleScheme");
		}
		public RubezhAPI.OperationResult<bool> RestoreScheduleScheme(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.RestoreScheduleScheme(RubezhServiceFactory.UID, uid, name);
			}, "RestoreScheduleScheme");
		}

		public RubezhAPI.OperationResult<List<Schedule>> GetSchedules(ScheduleFilter filter)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetSchedules(RubezhServiceFactory.UID, filter);
			}, "GetSchedules");
		}
		public RubezhAPI.OperationResult<bool> SaveSchedule(Schedule item, bool isNew)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.SaveSchedule(RubezhServiceFactory.UID, item, isNew);
			}, "SaveSchedule");
		}
		public RubezhAPI.OperationResult<bool> MarkDeletedSchedule(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.MarkDeletedSchedule(RubezhServiceFactory.UID, uid, name);
			}, "MarkDeletedSchedule");
		}
		public OperationResult<bool> RestoreSchedule(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.RestoreSchedule(RubezhServiceFactory.UID, uid, name);
			}, "RestoreSchedule");
		}

		public RubezhAPI.OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid employeeUID, DateTime startDateTime, DateTime endDateTime)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetTimeTrackDocument(RubezhServiceFactory.UID, employeeUID, startDateTime, endDateTime);
			}, "GetTimeTrackDocument");
		}
		public OperationResult<bool> AddTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.AddTimeTrackDocument(RubezhServiceFactory.UID, timeTrackDocument);
			}, "AddTimeTrackDocument");
		}
		public OperationResult<bool> EditTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.EditTimeTrackDocument(RubezhServiceFactory.UID, timeTrackDocument);
			}, "EditTimeTrackDocument");
		}
		public OperationResult<bool> RemoveTimeTrackDocument(Guid timeTrackDocumentUID)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.RemoveTimeTrackDocument(RubezhServiceFactory.UID, timeTrackDocumentUID);
			}, "RemoveTimeTrackDocument");
		}

		public OperationResult<List<TimeTrackDocumentType>> GetTimeTrackDocumentTypes(Guid organisationUID)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetTimeTrackDocumentTypes(RubezhServiceFactory.UID, organisationUID);
			}, "GetTimeTrackDocumentTypes");
		}
		public OperationResult<bool> AddTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.AddTimeTrackDocumentType(RubezhServiceFactory.UID, timeTrackDocumentType);
			}, "AddTimeTrackDocumentType");
		}
		public OperationResult<bool> EditTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.EditTimeTrackDocumentType(RubezhServiceFactory.UID, timeTrackDocumentType);
			}, "EditTimeTrackDocumentType");
		}
		public OperationResult<bool> RemoveTimeTrackDocumentType(Guid timeTrackDocumentTypeUID)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.RemoveTimeTrackDocumentType(RubezhServiceFactory.UID, timeTrackDocumentTypeUID);
			}, "RemoveTimeTrackDocumentType");
		}

		public OperationResult<bool> AddCustomPassJournal(Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.AddCustomPassJournal(RubezhServiceFactory.UID, uid, employeeUID, zoneUID, enterTime, exitTime);
			}, "AddCustomPassJournal");
		}
		public OperationResult<bool> EditPassJournal(Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.EditPassJournal(RubezhServiceFactory.UID, uid, zoneUID, enterTime, exitTime);
			}, "EditPassJournal");
		}
		public OperationResult<bool> DeletePassJournal(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.DeletePassJournal(RubezhServiceFactory.UID, uid);
			}, "DeletePassJournal");
		}
		public OperationResult<bool> DeleteAllPassJournalItems(Guid uid, DateTime enterTime, DateTime exitTime)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.DeleteAllPassJournalItems(RubezhServiceFactory.UID, uid, enterTime, exitTime);
			}, "DeleteAllPassJournalItems");
		}

		public OperationResult<DateTime> GetPassJournalMinDate()
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetPassJournalMinDate(RubezhServiceFactory.UID);
			}, "GetPassJournalMinDate");
		}
		public OperationResult<DateTime> GetCardsMinDate()
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetCardsMinDate(RubezhServiceFactory.UID);
			}, "GetCardsMinDate");
		}
	}
}