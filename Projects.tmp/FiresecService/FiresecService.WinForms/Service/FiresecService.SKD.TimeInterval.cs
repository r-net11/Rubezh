using RubezhAPI;
using RubezhAPI.Journal;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;

namespace FiresecService.Service
{
	public partial class FiresecService// : IFiresecService
	{
		public OperationResult<List<DayInterval>> GetDayIntervals(Guid clientUID, DayIntervalFilter filter)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.DayIntervalTranslator.Get(filter);
			}
		}
		public OperationResult<bool> SaveDayInterval(Guid clientUID, DayInterval item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_дневного_графика, item.Name, item.UID, clientUID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_дневного_графика, item.Name, item.UID, clientUID, JournalEventDescriptionType.Редактирование);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.DayIntervalTranslator.Save(item);
			}
		}
		public OperationResult<bool> MarkDeletedDayInterval(Guid clientUID, Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_дневного_графика, name, uid, clientUID);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.DayIntervalTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult<bool> RestoreDayInterval(Guid clientUID, Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_дневного_графика, name, uid, clientUID);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.DayIntervalTranslator.Restore(uid);
			}
		}

		public OperationResult<List<Holiday>> GetHolidays(Guid clientUID, HolidayFilter filter)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.HolidayTranslator.Get(filter);
			}
		}
		public OperationResult<bool> SaveHoliday(Guid clientUID, Holiday item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_праздничного_дня, item.Name, item.UID, clientUID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_праздничного_дня, item.Name, item.UID, clientUID, JournalEventDescriptionType.Редактирование);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.HolidayTranslator.Save(item);
			}
		}
		public OperationResult<bool> MarkDeletedHoliday(Guid clientUID, Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_праздничного_дня, name, uid, clientUID);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.HolidayTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult<bool> RestoreHoliday(Guid clientUID, Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_праздничного_дня, name, uid, clientUID);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.HolidayTranslator.Restore(uid);
			}
		}

		public OperationResult<List<ScheduleScheme>> GetScheduleSchemes(Guid clientUID, ScheduleSchemeFilter filter)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				var result = databaseService.ScheduleSchemeTranslator.Get(filter);
				return result;
			}
		}
		public OperationResult<bool> SaveScheduleScheme(Guid clientUID, ScheduleScheme item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_графика_работы_сотрудника, item.Name, item.UID, clientUID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_графика_работы_сотрудника, item.Name, item.UID, clientUID, JournalEventDescriptionType.Редактирование);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.ScheduleSchemeTranslator.Save(item);
			}
		}
		public OperationResult<bool> MarkDeletedScheduleScheme(Guid clientUID, Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_графика_работы_сотрудника, name, uid, clientUID);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.ScheduleSchemeTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult<bool> RestoreScheduleScheme(Guid clientUID, Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_графика_работы_сотрудника, name, uid, clientUID);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.ScheduleSchemeTranslator.Restore(uid);
			}
		}

		public OperationResult<List<Schedule>> GetSchedules(Guid clientUID, ScheduleFilter filter)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.ScheduleTranslator.Get(filter);
			}
		}
		public OperationResult<bool> SaveSchedule(Guid clientUID, Schedule item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_графика_работы, item.Name, item.UID, clientUID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_графика_работы, item.Name, item.UID, clientUID, JournalEventDescriptionType.Редактирование);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.ScheduleTranslator.Save(item);
			}
		}
		public OperationResult<bool> MarkDeletedSchedule(Guid clientUID, Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_графика_работы, name, uid, clientUID);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.ScheduleTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult<bool> RestoreSchedule(Guid clientUID, Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_графика_работы, name, uid, clientUID);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.ScheduleTranslator.Restore(uid);
			}
		}

		public OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid clientUID, Guid employeeUID, DateTime startDateTime, DateTime endDateTime)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TimeTrackDocumentTranslator.Get(employeeUID, startDateTime, endDateTime);
			}
		}
		public OperationResult<bool> AddTimeTrackDocument(Guid clientUID, TimeTrackDocument item)
		{
			AddJournalMessage(JournalEventNameType.Внесение_оправдательного_документа, item.DocumentNumber.ToString(), item.UID, clientUID, JournalEventDescriptionType.Редактирование);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TimeTrackDocumentTranslator.AddTimeTrackDocument(item);
			}
		}
		public OperationResult<bool> EditTimeTrackDocument(Guid clientUID, TimeTrackDocument item)
		{
			AddJournalMessage(JournalEventNameType.Внесение_оправдательного_документа, item.DocumentNumber.ToString(), item.UID, clientUID, JournalEventDescriptionType.Редактирование);
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TimeTrackDocumentTranslator.EditTimeTrackDocument(item);
			}
		}
		public OperationResult<bool> RemoveTimeTrackDocument(Guid clientUID, Guid timeTrackDocumentUID)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TimeTrackDocumentTranslator.RemoveTimeTrackDocument(timeTrackDocumentUID);
			}
		}

		public OperationResult<List<TimeTrackDocumentType>> GetTimeTrackDocumentTypes(Guid clientUID, Guid organisationUID)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TimeTrackDocumentTypeTranslator.Get(organisationUID);
			}
		}
		public OperationResult<bool> AddTimeTrackDocumentType(Guid clientUID, TimeTrackDocumentType timeTrackDocumentType)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TimeTrackDocumentTypeTranslator.AddTimeTrackDocumentType(timeTrackDocumentType);
			}
		}
		public OperationResult<bool> EditTimeTrackDocumentType(Guid clientUID, TimeTrackDocumentType timeTrackDocumentType)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TimeTrackDocumentTypeTranslator.EditTimeTrackDocumentType(timeTrackDocumentType);
			}
		}
		public OperationResult<bool> RemoveTimeTrackDocumentType(Guid clientUID, Guid timeTrackDocumentTypeUID)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.TimeTrackDocumentTypeTranslator.RemoveTimeTrackDocumentType(timeTrackDocumentTypeUID);
			}
		}

		public OperationResult<bool> AddCustomPassJournal(Guid clientUID, Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.PassJournalTranslator.AddCustomPassJournal(uid, employeeUID, zoneUID, enterTime, exitTime);
			}
		}
		public OperationResult<bool> EditPassJournal(Guid clientUID, Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.PassJournalTranslator.EditPassJournal(uid, zoneUID, enterTime, exitTime);
			}
		}
		public OperationResult<bool> DeletePassJournal(Guid clientUID, Guid uid)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.PassJournalTranslator.DeletePassJournal(uid);
			}
		}
		public OperationResult<bool> DeleteAllPassJournalItems(Guid clientUID, Guid uid, DateTime enterTime, DateTime exitTime)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.PassJournalTranslator.DeleteAllPassJournalItems(uid, enterTime, exitTime);
			}
		}

		public OperationResult<DateTime> GetPassJournalMinDate(Guid clientUID)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.PassJournalTranslator.GetMinDate();
			}
		}
		public OperationResult<DateTime> GetJournalMinDate(Guid clientUID)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.JournalTranslator.GetMinDate();
			}
		}
		public OperationResult<DateTime> GetCardsMinDate(Guid clientUID)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.CardTranslator.GetMinDate();
			}
		}
	}
}