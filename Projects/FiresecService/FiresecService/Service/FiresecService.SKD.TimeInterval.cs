using FiresecAPI;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using SKDDriver;
using SKDDriver.Translators;
using System;
using System.Collections.Generic;

namespace FiresecService.Service
{
	public partial class FiresecService : IFiresecService
	{
		public OperationResult<IEnumerable<DayInterval>> GetDayIntervals(DayIntervalFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DayIntervalTranslator.Get(filter);
			}
		}

		public OperationResult SaveDayInterval(DayInterval item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_дневного_графика, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_дневного_графика, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DayIntervalTranslator.Save(item);
			}
		}

		public OperationResult MarkDeletedDayInterval(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_дневного_графика, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DayIntervalTranslator.MarkDeleted(uid);
			}
		}

		public OperationResult RestoreDayInterval(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_дневного_графика, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DayIntervalTranslator.Restore(uid);
			}
		}

		public OperationResult<IEnumerable<DayIntervalPart>> GetDayIntervalParts(DayIntervalPartFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DayIntervalPartTranslator.Get(filter);
			}
		}

		public OperationResult SaveDayIntervalPart(DayIntervalPart item, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_дневного_графика, name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DayIntervalPartTranslator.Save(item);
			}
		}

		public OperationResult RemoveDayIntervalPart(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_дневного_графика, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DayIntervalPartTranslator.Delete(uid);
			}
		}

		public OperationResult<IEnumerable<Holiday>> GetHolidays(HolidayFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.HolidayTranslator.Get(filter);
			}
		}

		public OperationResult SaveHoliday(Holiday item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_праздничного_дня, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_праздничного_дня, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.HolidayTranslator.Save(item);
			}
		}

		public OperationResult MarkDeletedHoliday(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_праздничного_дня, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.HolidayTranslator.MarkDeleted(uid);
			}
		}

		public OperationResult RestoreHoliday(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_праздничного_дня, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.HolidayTranslator.Restore(uid);
			}
		}

		public OperationResult<IEnumerable<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleSchemeTranslator.Get(filter);
			}
		}

		public OperationResult SaveScheduleScheme(ScheduleScheme item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_графика_работы_сотрудника, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_графика_работы_сотрудника, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleSchemeTranslator.Save(item);
			}
		}

		public OperationResult MarkDeletedScheduleScheme(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_графика_работы_сотрудника, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleSchemeTranslator.MarkDeleted(uid);
			}
		}

		public OperationResult RestoreScheduleScheme(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_графика_работы_сотрудника, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleSchemeTranslator.Restore(uid);
			}
		}

		public OperationResult<IEnumerable<ScheduleDayInterval>> GetSheduleDayIntervals(ScheduleDayIntervalFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleDayIntervalTranslator.Get(filter);
			}
		}

		public OperationResult SaveSheduleDayInterval(ScheduleDayInterval item, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_графика_работы_сотрудника, name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleDayIntervalTranslator.Save(item);
			}
		}

		public OperationResult RemoveSheduleDayInterval(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_графика_работы_сотрудника, name);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleDayIntervalTranslator.Delete(uid);
			}
		}

		public OperationResult<IEnumerable<Schedule>> GetSchedules(ScheduleFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleTranslator.Get(filter);
			}
		}

		public OperationResult SaveSchedule(Schedule item, bool isNew)
		{
			if (isNew)
				AddJournalMessage(JournalEventNameType.Добавление_нового_графика_работы, item.Name, uid: item.UID);
			else
				AddJournalMessage(JournalEventNameType.Редактирование_графика_работы, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleTranslator.Save(item);
			}
		}

		public OperationResult MarkDeletedSchedule(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Удаление_графика_работы, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleTranslator.MarkDeleted(uid);
			}
		}

		public OperationResult<IEnumerable<ShortSchedule>> GetScheduleShortList(ScheduleFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleTranslator.GetList(filter);
			}
		}

		public OperationResult RestoreSchedule(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Восстановление_графика_работы, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleTranslator.Restore(uid);
			}
		}

		public OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleZoneTranslator.Get(filter);
			}
		}

		public OperationResult SaveScheduleZone(ScheduleZone item, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_графика_работы, name, JournalEventDescriptionType.Редактирование, uid: item.ScheduleUID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleZoneTranslator.Save(item);
			}
		}

		public OperationResult MarkDeletedScheduleZone(Guid uid, string name)
		{
			AddJournalMessage(JournalEventNameType.Редактирование_графика_работы, name, uid: uid);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleZoneTranslator.Delete(uid);
			}
		}

		public OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid employeeUID, DateTime startDateTime, DateTime endDateTime)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.TimeTrackDocumentTranslator.Get(employeeUID, startDateTime, endDateTime);
			}
		}

		public OperationResult AddTimeTrackDocument(TimeTrackDocument item)
		{
			AddJournalMessage(JournalEventNameType.Внесение_оправдательного_документа, item.DocumentNumber.ToString(), JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.TimeTrackDocumentTranslator.AddTimeTrackDocument(item);
			}
		}

		public OperationResult EditTimeTrackDocument(TimeTrackDocument item)
		{
			AddJournalMessage(JournalEventNameType.Внесение_оправдательного_документа, item.DocumentNumber.ToString(), JournalEventDescriptionType.Редактирование, uid: item.UID);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.TimeTrackDocumentTranslator.EditTimeTrackDocument(item);
			}
		}

		public OperationResult RemoveTimeTrackDocument(Guid timeTrackDocumentUID)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.TimeTrackDocumentTranslator.RemoveTimeTrackDocument(timeTrackDocumentUID);
			}
		}

		public OperationResult<List<TimeTrackDocumentType>> GetTimeTrackDocumentTypes(Guid organisationUID)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.TimeTrackDocumentTypeTranslator.Get(organisationUID);
			}
		}

		public OperationResult AddTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.TimeTrackDocumentTypeTranslator.AddTimeTrackDocumentType(timeTrackDocumentType);
			}
		}

		public OperationResult EditTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.TimeTrackDocumentTypeTranslator.EditTimeTrackDocumentType(timeTrackDocumentType);
			}
		}

		public OperationResult RemoveTimeTrackDocumentType(Guid timeTrackDocumentTypeUID)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.TimeTrackDocumentTypeTranslator.RemoveTimeTrackDocumentType(timeTrackDocumentTypeUID);
			}
		}

		public OperationResult<Dictionary<DayTimeTrackPart, List<DayTimeTrackPart>>> FindConflictIntervals(List<DayTimeTrackPart> dayTimeTrackParts, Guid employeeGuid, DateTime currentDate)
		{
			using(var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassJournalTranslator.FindConflictIntervals(dayTimeTrackParts, employeeGuid, currentDate);
			}
		}

		public OperationResult AddCustomPassJournal(Guid uid, Guid employeeUID, Guid zoneUID, DateTime? enterTime, DateTime? exitTime,
			DateTime? adjustmentDate, Guid correctedBy, bool notTakeInCalculations, bool isAddedManually, DateTime? enterTimeOriginal, DateTime? exitTimeOriginal, bool isRemoveAllIntersections)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassJournalTranslator.AddCustomPassJournal(uid, employeeUID, zoneUID, enterTime, exitTime,
					adjustmentDate, correctedBy, notTakeInCalculations, isAddedManually, enterTimeOriginal, exitTimeOriginal, isRemoveAllIntersections);
			}
		}

		public OperationResult SaveAllTimeTracks(IEnumerable<DayTimeTrackPart> collectionToSave, ShortEmployee employee)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassJournalTranslator.SaveAllTimeTracks(collectionToSave, employee);
			}
		}

		public OperationResult EditPassJournal(Guid uid, Guid zoneUID, DateTime? enterTime, DateTime? exitTime,
			bool isNeedAdjustment, DateTime? adjustmentDate, Guid correctedBy, bool notTakeInCalculations, bool isAddedManually, bool isRemoveAllIntersections, Guid employeeGuid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassJournalTranslator.EditPassJournal(uid, zoneUID, enterTime, exitTime,
					isNeedAdjustment, adjustmentDate, correctedBy, notTakeInCalculations, isAddedManually, isRemoveAllIntersections, employeeGuid);
			}
		}

		public OperationResult DeletePassJournal(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassJournalTranslator.DeletePassJournal(uid);
			}
		}

		public OperationResult DeleteAllPassJournalItems(Guid uid, DateTime enterTime, DateTime exitTime)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassJournalTranslator.DeleteAllPassJournalItems(uid, enterTime, exitTime);
			}
		}

		public OperationResult<DateTime> GetPassJournalMinDate()
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassJournalTranslator.GetMinDate();
			}
		}

		public OperationResult<DateTime> GetJournalMinDate()
		{
			using (var journalTranslator = new JounalTranslator())
			{
				return journalTranslator.GetMinDate();
			}
		}

		public OperationResult<DateTime> GetCardsMinDate()
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.CardTranslator.GetMinDate();
			}
		}

		public void CancelSKDProgress(Guid progressCallbackUID, string userName)
		{
			throw new NotImplementedException();
		}
	}
}