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
			return SafeOperationCall(() =>
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
		public RubezhAPI.OperationResult<bool> MarkDeletedDayInterval(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedDayInterval(FiresecServiceFactory.UID, uid, name);
			}, "MarkDeletedDayInterval");
		}
		public RubezhAPI.OperationResult<bool> RestoreDayInterval(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreDayInterval(FiresecServiceFactory.UID, uid, name);
			}, "RestoreDayInterval");
		}

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
		public RubezhAPI.OperationResult<bool> MarkDeletedHoliday(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedHoliday(FiresecServiceFactory.UID, uid, name);
			}, "MarkDeletedHoliday");
		}
		public RubezhAPI.OperationResult<bool> RestoreHoliday(Guid uid, string name)
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
		public RubezhAPI.OperationResult<bool> MarkDeletedScheduleScheme(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedScheduleScheme(FiresecServiceFactory.UID, uid, name);
			}, "MarkDeletedScheduleScheme");
		}
		public RubezhAPI.OperationResult<bool> RestoreScheduleScheme(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreScheduleScheme(FiresecServiceFactory.UID, uid, name);
			}, "RestoreScheduleScheme");
		}

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
		public RubezhAPI.OperationResult<bool> MarkDeletedSchedule(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.MarkDeletedSchedule(FiresecServiceFactory.UID, uid, name);
			}, "MarkDeletedSchedule");
		}
		public OperationResult<bool> RestoreSchedule(Guid uid, string name)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RestoreSchedule(FiresecServiceFactory.UID, uid, name);
			}, "RestoreSchedule");
		}

		public RubezhAPI.OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid employeeUID, DateTime startDateTime, DateTime endDateTime)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetTimeTrackDocument(FiresecServiceFactory.UID, employeeUID, startDateTime, endDateTime);
			}, "GetTimeTrackDocument");
		}
		public OperationResult<bool> AddTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.AddTimeTrackDocument(FiresecServiceFactory.UID, timeTrackDocument);
			}, "AddTimeTrackDocument");
		}
		public OperationResult<bool> EditTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.EditTimeTrackDocument(FiresecServiceFactory.UID, timeTrackDocument);
			}, "EditTimeTrackDocument");
		}
		public OperationResult<bool> RemoveTimeTrackDocument(Guid timeTrackDocumentUID)
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
		public OperationResult<bool> AddTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.AddTimeTrackDocumentType(FiresecServiceFactory.UID, timeTrackDocumentType);
			}, "AddTimeTrackDocumentType");
		}
		public OperationResult<bool> EditTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.EditTimeTrackDocumentType(FiresecServiceFactory.UID, timeTrackDocumentType);
			}, "EditTimeTrackDocumentType");
		}
		public OperationResult<bool> RemoveTimeTrackDocumentType(Guid timeTrackDocumentTypeUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RemoveTimeTrackDocumentType(FiresecServiceFactory.UID, timeTrackDocumentTypeUID);
			}, "RemoveTimeTrackDocumentType");
		}

		public OperationResult<bool> AddCustomPassJournal(Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.AddCustomPassJournal(FiresecServiceFactory.UID, uid, employeeUID, zoneUID, enterTime, exitTime);
			}, "AddCustomPassJournal");
		}
		public OperationResult<bool> EditPassJournal(Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.EditPassJournal(FiresecServiceFactory.UID, uid, zoneUID, enterTime, exitTime);
			}, "EditPassJournal");
		}
		public OperationResult<bool> DeletePassJournal(Guid uid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.DeletePassJournal(FiresecServiceFactory.UID, uid);
			}, "DeletePassJournal");
		}
		public OperationResult<bool> DeleteAllPassJournalItems(Guid uid, DateTime enterTime, DateTime exitTime)
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