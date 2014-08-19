using System.Collections.Generic;
using FiresecAPI.SKD;
using SKDDriver;
using System;
using FiresecAPI.Journal;
using FiresecAPI;

namespace FiresecService.Service
{
	public partial class FiresecService : IFiresecService
	{
		public OperationResult<IEnumerable<DayInterval>> GetDayIntervals(DayIntervalFilter filter)
		{
			return SKDDatabaseService.DayIntervalTranslator.Get(filter);
		}
		public OperationResult SaveDayInterval(DayInterval item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_дневного_графика);
			return SKDDatabaseService.DayIntervalTranslator.Save(item);
		}
		public OperationResult MarkDeletedDayInterval(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_дневного_графика);
			return SKDDatabaseService.DayIntervalTranslator.MarkDeleted(uid);
		}

		public OperationResult<IEnumerable<DayIntervalPart>> GetDayIntervalParts(DayIntervalPartFilter filter)
		{
			return SKDDatabaseService.DayIntervalPartTranslator.Get(filter);
		}
		public OperationResult SaveDayIntervalPart(DayIntervalPart item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_дневного_графика);
			return SKDDatabaseService.DayIntervalPartTranslator.Save(item);
		}
		public OperationResult MarkDeletedDayIntervalPart(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_дневного_графика);
			return SKDDatabaseService.DayIntervalPartTranslator.MarkDeleted(uid);
		}

		public OperationResult<IEnumerable<Holiday>> GetHolidays(HolidayFilter filter)
		{
			return SKDDatabaseService.HolidayTranslator.Get(filter);
		}
		public OperationResult SaveHoliday(Holiday item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_праздничного_дня);
			return SKDDatabaseService.HolidayTranslator.Save(item);
		}
		public OperationResult MarkDeletedHoliday(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_праздничного_дня);
			return SKDDatabaseService.HolidayTranslator.MarkDeleted(uid);
		}

		public OperationResult<IEnumerable<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			return SKDDatabaseService.ScheduleSchemeTranslator.Get(filter);
		}
		public OperationResult SaveScheduleScheme(ScheduleScheme item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_схемы_работы);
			return SKDDatabaseService.ScheduleSchemeTranslator.Save(item);
		}
		public OperationResult MarkDeletedScheduleScheme(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_схемы_работы);
			return SKDDatabaseService.ScheduleSchemeTranslator.MarkDeleted(uid);
		}

		public OperationResult<IEnumerable<ScheduleDayInterval>> GetSheduleDayIntervals(ScheduleDayIntervalFilter filter)
		{
			return SKDDatabaseService.ScheduleDayIntervalTranslator.Get(filter);
		}
		public OperationResult SaveSheduleDayInterval(ScheduleDayInterval item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_схемы_работы);
			return SKDDatabaseService.ScheduleDayIntervalTranslator.Save(item);
		}
		public OperationResult MarkDeletedSheduleDayInterval(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_схемы_работы);
			return SKDDatabaseService.ScheduleDayIntervalTranslator.MarkDeleted(uid);
		}

		public OperationResult<IEnumerable<Schedule>> GetSchedules(ScheduleFilter filter)
		{
			return SKDDatabaseService.ScheduleTranslator.Get(filter);
		}
		public OperationResult SaveSchedule(Schedule item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_графика_работы);
			return SKDDatabaseService.ScheduleTranslator.Save(item);
		}
		public OperationResult MarkDeletedSchedule(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_графика_работы);
			return SKDDatabaseService.ScheduleTranslator.MarkDeleted(uid);
		}
		public OperationResult<IEnumerable<ShortSchedule>> GetScheduleShortList(ScheduleFilter filter)
		{
			return SKDDatabaseService.ScheduleTranslator.GetList(filter);
		}
		
		public OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter)
		{
			return SKDDatabaseService.ScheduleZoneTranslator.Get(filter);
		}
		public OperationResult SaveScheduleZone(ScheduleZone item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_графика_работы);
			return SKDDatabaseService.ScheduleZoneTranslator.Save(item);
		}
		public OperationResult MarkDeletedScheduleZone(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_графика_работы);
			return SKDDatabaseService.ScheduleZoneTranslator.MarkDeleted(uid);
		}

		public OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid employeeUID, DateTime startDateTime, DateTime endDateTime)
		{
			return SKDDatabaseService.TimeTrackDocumentTranslator.Get(employeeUID, startDateTime, endDateTime);
		}
		public OperationResult AddTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return SKDDatabaseService.TimeTrackDocumentTranslator.AddTimeTrackDocument(timeTrackDocument);
		}
		public OperationResult EditTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return SKDDatabaseService.TimeTrackDocumentTranslator.EditTimeTrackDocument(timeTrackDocument);
		}
		public OperationResult RemoveTimeTrackDocument(Guid timeTrackDocumentUID)
		{
			return SKDDatabaseService.TimeTrackDocumentTranslator.RemoveTimeTrackDocument(timeTrackDocumentUID);
		}
	}
}