using FiresecAPI;
using FiresecAPI.Journal;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecService.Service.Validators;
using StrazhDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using DayInterval = FiresecAPI.SKD.DayInterval;
using DayIntervalPart = FiresecAPI.SKD.DayIntervalPart;
using DayIntervalPartValidator = FiresecService.Service.Validators.DayIntervalPartValidator;
using DayIntervalValidator = FiresecService.Service.Validators.DayIntervalValidator;
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
			// Валидируем дневной график
			var result = DayIntervalValidator.ValidateAddingOrEditing(item, isNew);
			if (result.HasError)
				return result;

			// Сохраняем дневной график
			using (var databaseService = new SKDDatabaseService())
			{
				result = databaseService.DayIntervalTranslator.Save(item);
			}

			// Генерируем соответствующую запись в журнале событий
			if (!result.HasError)
			{
				if (isNew)
					// для нового дневного графика
					AddJournalMessage(JournalEventNameType.Добавление_нового_дневного_графика, item.Name, uid: item.UID);
				else
					// для отредактированного дневного графика
					AddJournalMessage(JournalEventNameType.Редактирование_дневного_графика, item.Name, uid: item.UID);
			}

			return result;
		}

		public OperationResult MarkDeletedDayInterval(Guid uid, string name)
		{
			OperationResult operationResult;
			using (var databaseService = new SKDDatabaseService())
			{
				operationResult = databaseService.DayIntervalTranslator.MarkDeleted(uid);
			}
			if (!operationResult.HasError)
				AddJournalMessage(JournalEventNameType.Удаление_дневного_графика, name, uid: uid);
			return operationResult;
		}

		public OperationResult RestoreDayInterval(Guid uid, string name)
		{
			OperationResult operationResult;
			using (var databaseService = new SKDDatabaseService())
			{
				operationResult = databaseService.DayIntervalTranslator.Restore(uid);
			}
			if (!operationResult.HasError)
				AddJournalMessage(JournalEventNameType.Восстановление_дневного_графика, name, uid: uid);
			return operationResult;
		}

		public OperationResult<IEnumerable<DayIntervalPart>> GetDayIntervalParts(DayIntervalPartFilter filter)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DayIntervalPartTranslator.Get(filter);
			}
		}

		public OperationResult SaveDayIntervalPart(DayIntervalPart item, bool isNew, string name)
		{
			// Валидируем временной интервал дневного графика
			var result = DayIntervalPartValidator.ValidateAddingOrEditing(item, isNew);
			if (result.HasError)
				return result;

			// Сохраняем временной интервал дневного графика
			using (var databaseService = new SKDDatabaseService())
			{
				result = databaseService.DayIntervalPartTranslator.Save(item);
			}

			// Вставляем соответствующую запись в журнал событий
			if (!result.HasError)
				AddJournalMessage(JournalEventNameType.Редактирование_дневного_графика, name, uid: item.UID);

			return result;
		}

		public OperationResult RemoveDayIntervalPart(Guid uid, string name)
		{
			// Валидируем удаляемый временной интервал дневного графика
			var result = DayIntervalPartValidator.ValidateDeleting(uid);
			if (result.HasError)
				return result;

			// Удаляем временной интервал дневного графика
			using (var databaseService = new SKDDatabaseService())
			{
				result = databaseService.DayIntervalPartTranslator.Delete(uid);
			}

			// Вставляем соответствующую запись в журнал событий
			if (!result.HasError)
				AddJournalMessage(JournalEventNameType.Редактирование_дневного_графика, name, uid: uid);

			return result;
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
			OperationResult operationResult;
			using (var databaseService = new SKDDatabaseService())
			{
				operationResult = databaseService.HolidayTranslator.Save(item);
			}
			if (!operationResult.HasError)
			{
				if (isNew)
					AddJournalMessage(JournalEventNameType.Добавление_нового_праздничного_дня, item.Name, uid: item.UID);
				else
					AddJournalMessage(JournalEventNameType.Редактирование_праздничного_дня, item.Name, uid: item.UID);
			}
			return operationResult;
		}

		public OperationResult MarkDeletedHoliday(Guid uid, string name)
		{
			OperationResult operationResult;
			using (var databaseService = new SKDDatabaseService())
			{
				operationResult = databaseService.HolidayTranslator.MarkDeleted(uid);
			}
			if (!operationResult.HasError)
				AddJournalMessage(JournalEventNameType.Удаление_праздничного_дня, name, uid: uid);
			return operationResult;
		}

		public OperationResult RestoreHoliday(Guid uid, string name)
		{
			OperationResult operationResult;
			using (var databaseService = new SKDDatabaseService())
			{
				operationResult = databaseService.HolidayTranslator.Restore(uid);
			}
			if (!operationResult.HasError)
				AddJournalMessage(JournalEventNameType.Восстановление_праздничного_дня, name, uid: uid);
			return operationResult;
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
			OperationResult operationResult;
			using (var databaseService = new SKDDatabaseService())
			{
				operationResult = databaseService.ScheduleSchemeTranslator.Save(item);
			}
			if (!operationResult.HasError)
			{
				if (isNew)
					AddJournalMessage(JournalEventNameType.Добавление_нового_графика_работы_сотрудника, item.Name, uid: item.UID);
				else
					AddJournalMessage(JournalEventNameType.Редактирование_графика_работы_сотрудника, item.Name, uid: item.UID);
			}
			return operationResult;
		}

		public OperationResult MarkDeletedScheduleScheme(Guid uid, string name)
		{
			OperationResult operationResult;
			using (var databaseService = new SKDDatabaseService())
			{
				operationResult = databaseService.ScheduleSchemeTranslator.MarkDeleted(uid);
			}
			if (!operationResult.HasError)
				AddJournalMessage(JournalEventNameType.Удаление_графика_работы_сотрудника, name, uid: uid);
			return operationResult;
		}

		public OperationResult RestoreScheduleScheme(Guid uid, string name)
		{
			OperationResult operationResult;
			using (var databaseService = new SKDDatabaseService())
			{
				operationResult = databaseService.ScheduleSchemeTranslator.Restore(uid);
			}
			if (!operationResult.HasError)
				AddJournalMessage(JournalEventNameType.Восстановление_графика_работы_сотрудника, name, uid: uid);
			return operationResult;
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
			OperationResult operationResult;
			using (var databaseService = new SKDDatabaseService())
			{
				// Валидируем Схему графика работы
				operationResult = ScheduleDayIntervalValidator.ValidateAddingOrEditing(item);
				if (operationResult.HasError)
					return operationResult;
				
				// Сохраняем изменения в БД
				operationResult = databaseService.ScheduleDayIntervalTranslator.Save(item);
			}
			// Если ошибок нет, то оставляем соответствующее сообщение в журнале событий
			if (!operationResult.HasError)
				AddJournalMessage(JournalEventNameType.Редактирование_графика_работы_сотрудника, name, uid: item.UID);
			
			return operationResult;
		}

		public OperationResult RemoveSheduleDayInterval(Guid uid, string name)
		{
			OperationResult operationResult;
			using (var databaseService = new SKDDatabaseService())
			{
				// Валидируем Схему графика работы
				operationResult = ScheduleDayIntervalValidator.ValidateDeleting(uid);
				if (operationResult.HasError)
					return operationResult;

				// Сохраняем изменения в БД
				operationResult = databaseService.ScheduleDayIntervalTranslator.Delete(uid);
			}
			// Если ошибок нет, то оставляем соответствующее сообщение в журнале событий
			if (!operationResult.HasError)
				AddJournalMessage(JournalEventNameType.Удаление_графика_работы_сотрудника, name);

			return operationResult;
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
			OperationResult operationResult;
			using (var databaseService = new SKDDatabaseService())
			{
				operationResult = databaseService.ScheduleTranslator.Save(item);
			}
			if (!operationResult.HasError)
			{
				if (isNew)
					AddJournalMessage(JournalEventNameType.Добавление_нового_графика_работы, item.Name, uid: item.UID);
				else
					AddJournalMessage(JournalEventNameType.Редактирование_графика_работы, item.Name, JournalEventDescriptionType.Редактирование, uid: item.UID);
			}
			return operationResult;
		}

		public OperationResult MarkDeletedSchedule(Guid uid, string name)
		{
			OperationResult operationResult;
			using (var databaseService = new SKDDatabaseService())
			{
				operationResult = databaseService.ScheduleTranslator.MarkDeleted(uid);
			}
			if (!operationResult.HasError)
				AddJournalMessage(JournalEventNameType.Удаление_графика_работы, name, uid: uid);
			return operationResult;
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
			OperationResult operationResult;
			using (var databaseService = new SKDDatabaseService())
			{
				operationResult = databaseService.ScheduleTranslator.Restore(uid);
			}
			if (!operationResult.HasError)
				AddJournalMessage(JournalEventNameType.Восстановление_графика_работы, name, uid: uid);
			return operationResult;
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
			OperationResult operationResult;
			using (var databaseService = new SKDDatabaseService())
			{
				operationResult = databaseService.ScheduleZoneTranslator.Save(item);
			}
			if (!operationResult.HasError)
				AddJournalMessage(JournalEventNameType.Редактирование_графика_работы, name, uid: item.ScheduleUID);
			return operationResult;
		}

		public OperationResult MarkDeletedScheduleZone(Guid uid, string name)
		{
			OperationResult operationResult;
			using (var databaseService = new SKDDatabaseService())
			{
				operationResult = databaseService.ScheduleZoneTranslator.Delete(uid);
			}
			if (!operationResult.HasError)
				AddJournalMessage(JournalEventNameType.Редактирование_графика_работы, name, uid: uid);
			return operationResult;
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
			OperationResult operationResult;
			using (var databaseService = new SKDDatabaseService())
			{
				operationResult = databaseService.TimeTrackDocumentTranslator.AddTimeTrackDocument(item);
			}
			if (!operationResult.HasError)
				AddJournalMessage(JournalEventNameType.AddTimeTrackDocument, item.JournalEventName, uid: item.UID);
			return operationResult;
		}

		public OperationResult EditTimeTrackDocument(TimeTrackDocument item)
		{
			OperationResult operationResult;
			using (var databaseService = new SKDDatabaseService())
			{
				operationResult = databaseService.TimeTrackDocumentTranslator.EditTimeTrackDocument(item);
			}
			if (!operationResult.HasError)
				AddJournalMessage(JournalEventNameType.EditTimeTrackDocument, item.JournalEventName, uid: item.UID);
			return operationResult;
		}

		public OperationResult RemoveTimeTrackDocument(Guid timeTrackDocumentUID)
		{
			OperationResult operationResult;
			TimeTrackDocument timeTrackDocument;
			using (var databaseService = new SKDDatabaseService())
			{
				timeTrackDocument = databaseService.TimeTrackDocumentTranslator.Get(timeTrackDocumentUID);
				operationResult = databaseService.TimeTrackDocumentTranslator.RemoveTimeTrackDocument(timeTrackDocumentUID);
			}
			if (!operationResult.HasError && timeTrackDocument != null)
				AddJournalMessage(JournalEventNameType.RemoveTimeTrackDocument, timeTrackDocument.JournalEventName, uid: timeTrackDocument.UID);
			return operationResult;
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

		public OperationResult CheckDocumentType(TimeTrackDocumentType timeTrackDocumentType, Guid organisationUID)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.TimeTrackDocumentTranslator.CheckDocumentType(timeTrackDocumentType, organisationUID);
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

		private void SetJournalMessageForTimeTrackPart(DayTimeTrackPart dayTimeTrackPart, ShortEmployee employee, User currentUser, DateTime? resetAdjustmentDate)
		{
			if (resetAdjustmentDate.HasValue) return;

			if ((dayTimeTrackPart.TimeTrackActions & TimeTrackActions.Adding) == TimeTrackActions.Adding)
			{
				AddJournalMessage(JournalEventNameType.Добавление_интервала,
								"Интервал рабочего времени (" + employee.FIO + ")",
								JournalEventDescriptionType.NULL,
								"Интервал добавлен (" + dayTimeTrackPart.EnterDateTime + " - " + dayTimeTrackPart.ExitDateTime + ", зона " + dayTimeTrackPart.TimeTrackZone.Name + ")",
								currentUser.Name);
			}
			if ((dayTimeTrackPart.TimeTrackActions & TimeTrackActions.EditBorders) == TimeTrackActions.EditBorders)
			{
				AddJournalMessage(JournalEventNameType.Изменение_границы_интервала,
								"Интервал рабочего времени (" + employee.FIO + ")",
								JournalEventDescriptionType.NULL,
								"Границы интервала изменены (" + dayTimeTrackPart.EnterDateTime + " - " + dayTimeTrackPart.ExitDateTime + ", зона " + dayTimeTrackPart.TimeTrackZone.Name + ")",
								currentUser.Name);
			}
			if ((dayTimeTrackPart.TimeTrackActions & TimeTrackActions.Remove) == TimeTrackActions.Remove)
			{
				AddJournalMessage(JournalEventNameType.Удаление_интервала,
					null, JournalEventDescriptionType.Удаление,
					"Интервал удален (" + dayTimeTrackPart.EnterDateTime + "-" + dayTimeTrackPart.ExitDateTime + ", зона" + dayTimeTrackPart.TimeTrackZone.Name + ")");
			}
			if ((dayTimeTrackPart.TimeTrackActions & TimeTrackActions.TurnOffCalculation) == TimeTrackActions.TurnOffCalculation)
			{
				AddJournalMessage(JournalEventNameType.Снятие_неУчитывать_в_расчетах,
								"Интервал рабочего времени (" + employee.FIO + ")",
								JournalEventDescriptionType.NULL,
								"Интервал добавлен в расчеты (" + dayTimeTrackPart.EnterDateTime + " - " + dayTimeTrackPart.ExitDateTime + ", зона " + dayTimeTrackPart.TimeTrackZone.Name + ")",
								currentUser.Name);
			}
			if ((dayTimeTrackPart.TimeTrackActions & TimeTrackActions.TurnOnCalculation) == TimeTrackActions.TurnOnCalculation)
			{
				AddJournalMessage(JournalEventNameType.Установка_неУчитывать_в_расчетах,
								"Интервал рабочего времени (" + employee.FIO + ")",
								JournalEventDescriptionType.NULL,
								"Интервал исключен из расчетов (" + dayTimeTrackPart.EnterDateTime + " - " + dayTimeTrackPart.ExitDateTime + ", зона " + dayTimeTrackPart.TimeTrackZone.Name + ")",
								currentUser.Name);
			}
		}

		public OperationResult SaveAllTimeTracks(IEnumerable<DayTimeTrackPart> collectionToSave, ShortEmployee employee, User currentUser, IEnumerable<DayTimeTrackPart> removedDayTimeTrackParts, DateTime? resetAdjustmentsDate)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				if (removedDayTimeTrackParts.Any())
				{
					foreach (var removedDayTimeTrackPart in removedDayTimeTrackParts)
					{
						if (!DeletePassJournal(removedDayTimeTrackPart.UID).HasError)
						{
							SetJournalMessageForTimeTrackPart(removedDayTimeTrackPart, employee, currentUser, resetAdjustmentsDate);
						}
					}
				}

				foreach (var dayTimeTrackPart in collectionToSave)
				{
					if (dayTimeTrackPart.IsNew)
					{
						if (!databaseService.PassJournalTranslator.AddCustomPassJournal(dayTimeTrackPart, employee).HasError)
						{
							SetJournalMessageForTimeTrackPart(dayTimeTrackPart, employee, currentUser, resetAdjustmentsDate);
						}

					}
					else
					{
						if (!databaseService.PassJournalTranslator.EditPassJournal(dayTimeTrackPart, employee).HasError)
						{
							SetJournalMessageForTimeTrackPart(dayTimeTrackPart, employee, currentUser, resetAdjustmentsDate);
						}
					}
				}

				if(resetAdjustmentsDate.HasValue)
					AddJournalMessage(JournalEventNameType.ResetAdjustmentInterval,
								employee.FIO,
								descriptionText: "Сброс корректировок " + resetAdjustmentsDate.Value.Date.ToShortDateString(),
								userName: currentUser.Name);

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
			OperationResult operationResult;
			using (var databaseService = new SKDDatabaseService())
			{
				operationResult = databaseService.PassJournalTranslator.DeleteAllPassJournalItems(dayTimeTrackPart);
			}
			if (!operationResult.HasError)
				AddJournalMessage(JournalEventNameType.Удаление_интервала, null, JournalEventDescriptionType.Удаление,
					"Интервал удален (" + dayTimeTrackPart.EnterDateTime + "-" + dayTimeTrackPart.ExitDateTime + ", зона" + dayTimeTrackPart.TimeTrackZone.Name + ")");
			return operationResult;
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