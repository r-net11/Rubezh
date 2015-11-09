using System.Threading.Tasks;
using FiresecAPI;
using FiresecAPI.Journal;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using SKDDriver;
using SKDDriver.Translators;
using System;
using System.Collections.Generic;
using System.Linq;
using DayInterval = FiresecAPI.SKD.DayInterval;
using DayIntervalPart = FiresecAPI.SKD.DayIntervalPart;
using Holiday = FiresecAPI.SKD.Holiday;
using Schedule = FiresecAPI.SKD.Schedule;
using ScheduleScheme = FiresecAPI.SKD.ScheduleScheme;
using ScheduleZone = FiresecAPI.SKD.ScheduleZone;
using TimeTrackDocument = FiresecAPI.SKD.TimeTrackDocument;
using TimeTrackDocumentType = FiresecAPI.SKD.TimeTrackDocumentType;

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

		public OperationResult<IEnumerable<TimeTrackDocumentType>> GetSystemDocumentTypes()
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.TimeTrackDocumentTypeTranslator.GetSystemDocumentTypes();
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

		public OperationResult<bool> CheckForCanForseCloseInterval(Guid openedIntervalGuid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassJournalTranslator.CheckForCanForseCloseInterval(openedIntervalGuid);
			}
		}

		public OperationResult<List<DayTimeTrackPart>> GetMissedIntervals(DateTime currentDate, ShortEmployee currentEmployee)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassJournalTranslator.GetMissedIntervals(currentDate, currentEmployee);
			}
		}

		public OperationResult SaveAllTimeTracks(IEnumerable<DayTimeTrackPart> collectionToSave, ShortEmployee employee, User currentUser, IEnumerable<DayTimeTrackPart> removedDayTimeTrackParts)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				if (removedDayTimeTrackParts.Any()) databaseService.PassJournalTranslator.RemoveSelectedIntervals(removedDayTimeTrackParts);

				foreach (var dayTimeTrackPart in collectionToSave)
				{
					if (dayTimeTrackPart.IsNew)
					{
						databaseService.PassJournalTranslator.AddCustomPassJournal(dayTimeTrackPart, employee);
						AddJournalMessage(JournalEventNameType.Добавление_интервала,
										"Интервал рабочего времени (" + employee.FIO + ")",
										JournalEventDescriptionType.NULL,
										"Интервал добавлен (" + dayTimeTrackPart.EnterDateTime + " - " + dayTimeTrackPart.ExitDateTime + ", зона " + dayTimeTrackPart.TimeTrackZone.Name + ")",
										currentUser.Name);

					}
					else
					{
						bool? setAdjustmentFlag;
						bool setBordersChangedFlag;
						bool setForceClosedFlag;

						databaseService.PassJournalTranslator.EditPassJournal(dayTimeTrackPart, employee, out setAdjustmentFlag, out setBordersChangedFlag, out setForceClosedFlag);

						if (setAdjustmentFlag == true && !setForceClosedFlag)
							AddJournalMessage(JournalEventNameType.Установка_неУчитывать_в_расчетах,
										"Интервал рабочего времени (" + employee.FIO + ")",
										JournalEventDescriptionType.NULL,
										"Интервал исключен из расчетов (" + dayTimeTrackPart.EnterDateTime + " - " + dayTimeTrackPart.ExitDateTime + ", зона " + dayTimeTrackPart.TimeTrackZone.Name + ")",
										currentUser.Name);
						else if(setAdjustmentFlag == false)
							AddJournalMessage(JournalEventNameType.Снятие_неУчитывать_в_расчетах,
										"Интервал рабочего времени (" + employee.FIO + ")",
										JournalEventDescriptionType.NULL,
										"Интервал добавлен в расчеты (" + dayTimeTrackPart.EnterDateTime + " - " + dayTimeTrackPart.ExitDateTime + ", зона " + dayTimeTrackPart.TimeTrackZone.Name + ")",
										currentUser.Name);

						if (setForceClosedFlag)
							AddJournalMessage(JournalEventNameType.Закрытие_интервала,
										"Интервал рабочего времени (" + employee.FIO + ")",
										JournalEventDescriptionType.NULL,
										"Интервал принудительно закрыт (" + dayTimeTrackPart.EnterDateTime + " - " + dayTimeTrackPart.ExitDateTime + ", зона " + dayTimeTrackPart.TimeTrackZone.Name + ")",
										currentUser.Name);
						else if (setBordersChangedFlag)
							AddJournalMessage(JournalEventNameType.Изменение_границы_интервала,
										"Интервал рабочего времени (" + employee.FIO + ")",
										JournalEventDescriptionType.NULL,
										"Границы интервала изменены (" + dayTimeTrackPart.EnterDateTime + " - " + dayTimeTrackPart.ExitDateTime + ", зона " + dayTimeTrackPart.TimeTrackZone.Name + ")",
										currentUser.Name);
					}
				}
				return new OperationResult();
			}
		}

		public OperationResult DeletePassJournal(Guid uid)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassJournalTranslator.DeletePassJournal(uid);
			}
		}

		public OperationResult DeleteAllPassJournalItems(DayTimeTrackPart dayTimeTrackPart)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				AddJournalMessage(JournalEventNameType.Удаление_интервала, null, JournalEventDescriptionType.Удаление,
					"Интервал удален (" + dayTimeTrackPart.EnterDateTime + "-" + dayTimeTrackPart.ExitDateTime + ", зона" + dayTimeTrackPart.TimeTrackZone.Name + ")");
				return databaseService.PassJournalTranslator.DeleteAllPassJournalItems(dayTimeTrackPart);
			}
		}

		public OperationResult<IEnumerable<DayTimeTrackPart>> GetIntersectionIntervals(
			DayTimeTrackPart currentDayTimeTrackPart,
			ShortEmployee currentEmployee)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.PassJournalTranslator.GetIntersectionIntervals(currentDayTimeTrackPart, currentEmployee);
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
			using (var journalTranslator = new JournalTranslator())
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
	}
}