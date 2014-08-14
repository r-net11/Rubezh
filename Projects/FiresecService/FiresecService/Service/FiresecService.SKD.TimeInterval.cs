using System.Collections.Generic;
using FiresecAPI.EmployeeTimeIntervals;
using SKDDriver;
using System;
using FiresecAPI.Journal;

namespace FiresecService.Service
{
	public partial class FiresecService : FiresecAPI.IFiresecService
	{
		public FiresecAPI.OperationResult<IEnumerable<DayInterval>> GetDayIntervals(DayIntervalFilter filter)
		{
			return SKDDatabaseService.DayIntervalTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveDayInterval(DayInterval item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_дневного_графика);
			return SKDDatabaseService.DayIntervalTranslator.Save(item);
		}
		public FiresecAPI.OperationResult MarkDeletedDayInterval(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_дневного_графика);
			return SKDDatabaseService.DayIntervalTranslator.MarkDeleted(uid);
		}

		public FiresecAPI.OperationResult<IEnumerable<DayIntervalPart>> GetDayIntervalParts(DayIntervalPartFilter filter)
		{
			return SKDDatabaseService.DayIntervalPartTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveDayIntervalPart(DayIntervalPart item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_дневного_графика);
			return SKDDatabaseService.DayIntervalPartTranslator.Save(item);
		}
		public FiresecAPI.OperationResult MarkDeletedDayIntervalPart(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_дневного_графика);
			return SKDDatabaseService.DayIntervalPartTranslator.MarkDeleted(uid);
		}

		public FiresecAPI.OperationResult<IEnumerable<Holiday>> GetHolidays(HolidayFilter filter)
		{
			return SKDDatabaseService.HolidayTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveHoliday(Holiday item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_праздничного_дня);
			return SKDDatabaseService.HolidayTranslator.Save(item);
		}
		public FiresecAPI.OperationResult MarkDeletedHoliday(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_праздничного_дня);
			return SKDDatabaseService.HolidayTranslator.MarkDeleted(uid);
		}

		public FiresecAPI.OperationResult<IEnumerable<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			return SKDDatabaseService.ScheduleSchemeTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveScheduleScheme(ScheduleScheme item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_схемы_работы);
			return SKDDatabaseService.ScheduleSchemeTranslator.Save(item);
		}
		public FiresecAPI.OperationResult MarkDeletedScheduleScheme(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_схемы_работы);
			return SKDDatabaseService.ScheduleSchemeTranslator.MarkDeleted(uid);
		}

		public FiresecAPI.OperationResult<IEnumerable<ScheduleDayInterval>> GetSheduleDayIntervals(ScheduleDayIntervalFilter filter)
		{
			return SKDDatabaseService.ScheduleDayIntervalTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveSheduleDayInterval(ScheduleDayInterval item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_схемы_работы);
			return SKDDatabaseService.ScheduleDayIntervalTranslator.Save(item);
		}
		public FiresecAPI.OperationResult MarkDeletedSheduleDayInterval(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_схемы_работы);
			return SKDDatabaseService.ScheduleDayIntervalTranslator.MarkDeleted(uid);
		}

		public FiresecAPI.OperationResult<IEnumerable<Schedule>> GetSchedules(ScheduleFilter filter)
		{
			return SKDDatabaseService.ScheduleTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveSchedule(Schedule item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_графика_работы);
			return SKDDatabaseService.ScheduleTranslator.Save(item);
		}
		public FiresecAPI.OperationResult MarkDeletedSchedule(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_графика_работы);
			return SKDDatabaseService.ScheduleTranslator.MarkDeleted(uid);
		}
		public FiresecAPI.OperationResult<IEnumerable<ShortSchedule>> GetScheduleShortList(ScheduleFilter filter)
		{
			return SKDDatabaseService.ScheduleTranslator.GetList(filter);
		}
		
		public FiresecAPI.OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter)
		{
			return SKDDatabaseService.ScheduleZoneTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveScheduleZone(ScheduleZone item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_графика_работы);
			return SKDDatabaseService.ScheduleZoneTranslator.Save(item);
		}
		public FiresecAPI.OperationResult MarkDeletedScheduleZone(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_графика_работы);
			return SKDDatabaseService.ScheduleZoneTranslator.MarkDeleted(uid);
		}

		public FiresecAPI.OperationResult<FiresecAPI.SKD.TimeTrackDocument> GetTimeTrackDocument(DateTime dateTime, Guid employeeUID)
		{
			return SKDDatabaseService.TimeTrackDocumentTranslator.Get(dateTime, employeeUID);
		}
		public FiresecAPI.OperationResult SaveTimeTrackDocument(FiresecAPI.SKD.TimeTrackDocument item)
		{
			AddSKDJournalMessage(JournalEventNameType.Внесение_оправдательного_документа);
			return SKDDatabaseService.TimeTrackDocumentTranslator.Save(item);
		}
	}
}