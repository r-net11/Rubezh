using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using SKDDriver;

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
		public OperationResult SaveDayInterval(DayInterval item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_дневного_графика);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DayIntervalTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedDayInterval(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_дневного_графика);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DayIntervalTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult RestoreDayInterval(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_дневного_графика);
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
		public OperationResult SaveDayIntervalPart(DayIntervalPart item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_дневного_графика);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.DayIntervalPartTranslator.Save(item);
			}
		}
		public OperationResult RemoveDayIntervalPart(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_дневного_графика);
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
		public OperationResult SaveHoliday(Holiday item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_праздничного_дня);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.HolidayTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedHoliday(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_праздничного_дня);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.HolidayTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult RestoreHoliday(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_праздничного_дня);
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
		public OperationResult SaveScheduleScheme(ScheduleScheme item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_схемы_работы);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleSchemeTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedScheduleScheme(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_схемы_работы);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleSchemeTranslator.MarkDeleted(uid);
			}
		}
		public OperationResult RestoreScheduleScheme(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_схемы_работы);
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
		public OperationResult SaveSheduleDayInterval(ScheduleDayInterval item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_схемы_работы);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleDayIntervalTranslator.Save(item);
			}
		}
		public OperationResult RemoveSheduleDayInterval(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_схемы_работы);
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
		public OperationResult SaveSchedule(Schedule item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_графика_работы);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedSchedule(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_графика_работы);
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
		public OperationResult RestoreSchedule(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_графика_работы);
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
		public OperationResult SaveScheduleZone(ScheduleZone item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_графика_работы);
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.ScheduleZoneTranslator.Save(item);
			}
		}
		public OperationResult MarkDeletedScheduleZone(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_графика_работы);
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
		public OperationResult AddTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.TimeTrackDocumentTranslator.AddTimeTrackDocument(timeTrackDocument);
			}
		}
		public OperationResult EditTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			using (var databaseService = new SKDDatabaseService())
			{
				return databaseService.TimeTrackDocumentTranslator.EditTimeTrackDocument(timeTrackDocument);
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
	}
}