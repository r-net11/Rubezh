using System;
using System.Collections.Generic;
using RubezhAPI;
using RubezhAPI.Journal;
using RubezhAPI.SKD;
using System.Threading;

namespace FiresecService.Service
{
	public partial class FiresecService : IFiresecService
	{
		public OperationResult<List<DayInterval>> GetDayIntervals(DayIntervalFilter filter)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.DayIntervalTranslator.Get(filter);
			}
		}
		public OperationResult<bool> SaveDayInterval(DayInterval item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_дневного_графика, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_дневного_графика, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.DayIntervalTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedDayInterval(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_дневного_графика, name, uid: uid);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.DayIntervalTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult RestoreDayInterval(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_дневного_графика, name, uid: uid);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.DayIntervalTranslator.Restore(uid);
			}
		}

		public OperationResult<List<Holiday>> GetHolidays(HolidayFilter filter)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.HolidayTranslator.Get(filter);
			}
		}
		public OperationResult<bool> SaveHoliday(Holiday item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_праздничного_дня, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_праздничного_дня, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.HolidayTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedHoliday(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_праздничного_дня, name, uid: uid);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.HolidayTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult RestoreHoliday(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_праздничного_дня, name, uid: uid);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.HolidayTranslator.Restore(uid);
			}
		}

		public OperationResult<List<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				var result = databaseService.ScheduleSchemeTranslator.Get(filter);
				return result;
			}
		}
		public OperationResult<bool> SaveScheduleScheme(ScheduleScheme item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_графика_работы_сотрудника, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_графика_работы_сотрудника, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.ScheduleSchemeTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedScheduleScheme(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_графика_работы_сотрудника, name, uid: uid);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.ScheduleSchemeTranslator.MarkDeleted(uid);
			}
		}		public OperationResult RestoreScheduleScheme(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_графика_работы_сотрудника, name, uid: uid);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.ScheduleSchemeTranslator.Restore(uid);
			}
		}

		public OperationResult<List<Schedule>> GetSchedules(ScheduleFilter filter)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.ScheduleTranslator.Get(filter);
			}
		}
		public OperationResult<bool> SaveSchedule(Schedule item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_графика_работы, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_графика_работы, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.ScheduleTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedSchedule(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_графика_работы, name, uid: uid);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.ScheduleTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult RestoreSchedule(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_графика_работы, name, uid: uid);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.ScheduleTranslator.Restore(uid);
			}
		}

		public OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid employeeUID, DateTime startDateTime, DateTime endDateTime)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TimeTrackDocumentTranslator.Get(employeeUID, startDateTime, endDateTime);
			}
		}
		public OperationResult AddTimeTrackDocument(TimeTrackDocument item)
		{
			AddJournalMessage(JournalEventNameType.Внесение_оправдательного_документа, item.DocumentNumber.ToString(), JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TimeTrackDocumentTranslator.AddTimeTrackDocument(item);
			}
		}
		public OperationResult EditTimeTrackDocument(TimeTrackDocument item)
		{
			AddJournalMessage(JournalEventNameType.Внесение_оправдательного_документа, item.DocumentNumber.ToString(), JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TimeTrackDocumentTranslator.EditTimeTrackDocument(item);
			}
		}
		public OperationResult RemoveTimeTrackDocument(Guid timeTrackDocumentUID)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TimeTrackDocumentTranslator.RemoveTimeTrackDocument(timeTrackDocumentUID);
			}
		}

		public OperationResult<List<TimeTrackDocumentType>> GetTimeTrackDocumentTypes(Guid organisationUID)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TimeTrackDocumentTypeTranslator.Get(organisationUID);
			}
		}
		public OperationResult AddTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TimeTrackDocumentTypeTranslator.AddTimeTrackDocumentType(timeTrackDocumentType);
			}
		}
		public OperationResult EditTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TimeTrackDocumentTypeTranslator.EditTimeTrackDocumentType(timeTrackDocumentType);
			}
		}
		public OperationResult RemoveTimeTrackDocumentType(Guid timeTrackDocumentTypeUID)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TimeTrackDocumentTypeTranslator.RemoveTimeTrackDocumentType(timeTrackDocumentTypeUID);
			}
		}

		public OperationResult AddCustomPassJournal(Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.PassJournalTranslator.AddCustomPassJournal(uid, employeeUID, zoneUID, enterTime, exitTime);
			}
		}
		public OperationResult EditPassJournal(Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.PassJournalTranslator.EditPassJournal(uid, zoneUID, enterTime, exitTime);
			}
		}
		public OperationResult DeletePassJournal(Guid uid)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.PassJournalTranslator.DeletePassJournal(uid);
			}
		}
		public OperationResult DeleteAllPassJournalItems(Guid uid, DateTime enterTime, DateTime exitTime)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.PassJournalTranslator.DeleteAllPassJournalItems(uid, enterTime, exitTime);
			}
		}

		public OperationResult<DateTime> GetPassJournalMinDate()
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.PassJournalTranslator.GetMinDate();
			}
		}
		public OperationResult<DateTime> GetJournalMinDate()
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.JournalTranslator.GetMinDate();
			}
		}
		public OperationResult<DateTime> GetCardsMinDate()
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.CardTranslator.GetMinDate();
			}
		}

		public string[] GetOpcDaServerNames()
		{
			throw new NotImplementedException();
		}

		public OperationResult<RubezhAPI.Automation.OpcDaServer> GetOpcDaServerGroupAndTags(string serverName)
		{
			throw new NotImplementedException();
		}

		public OperationResult ConnectToOpcDaServer(RubezhAPI.Automation.OpcDaServer server)
		{
			throw new NotImplementedException();
		}

		public OperationResult DisconnectFromOpcDaServer(RubezhAPI.Automation.OpcDaServer server)
		{
			throw new NotImplementedException();
		}

		public OperationResult<OpcClientSdk.OpcServerStatus> GetOpcDaServerStatus(RubezhAPI.Automation.OpcDaServer server)
		{
			throw new NotImplementedException();
		}

		public OperationResult<OpcClientSdk.Da.TsCDaItemValueResult[]> ReadOpcDaServerTags(RubezhAPI.Automation.OpcDaServer server)
		{
			throw new NotImplementedException();
		}

		public OperationResult WriteOpcDaServerTags(RubezhAPI.Automation.OpcDaServer server, OpcClientSdk.Da.TsCDaItemValueResult[] tagValues)
		{
			throw new NotImplementedException();
		}
	}
}