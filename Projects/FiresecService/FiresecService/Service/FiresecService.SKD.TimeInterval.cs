using System.Collections.Generic;
using FiresecAPI.EmployeeTimeIntervals;
using SKDDriver;
using System;
using FiresecAPI.Journal;

namespace FiresecService.Service
{
	public partial class FiresecService : FiresecAPI.IFiresecService
	{
		public FiresecAPI.OperationResult<IEnumerable<NamedInterval>> GetNamedIntervals(NamedIntervalFilter filter)
		{
			return SKDDatabaseService.NamedIntervalTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveNamedInterval(NamedInterval item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_дневного_графика);
			return SKDDatabaseService.NamedIntervalTranslator.Save(item);
		}
		public FiresecAPI.OperationResult MarkDeletedNamedInterval(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_дневного_графика);
			return SKDDatabaseService.NamedIntervalTranslator.MarkDeleted(uid);
		}

		public FiresecAPI.OperationResult<IEnumerable<TimeInterval>> GetTimeIntervals(TimeIntervalFilter filter)
		{
			return SKDDatabaseService.TimeIntervalTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveTimeInterval(TimeInterval item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_дневного_графика);
			return SKDDatabaseService.TimeIntervalTranslator.Save(item);
		}
		public FiresecAPI.OperationResult MarkDeletedTimeInterval(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_дневного_графика);
			return SKDDatabaseService.TimeIntervalTranslator.MarkDeleted(uid);
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

		public FiresecAPI.OperationResult<IEnumerable<DayInterval>> GetDayIntervals(DayIntervalFilter filter)
		{
			return SKDDatabaseService.DayIntervalTranslator.Get(filter);
		}
		public FiresecAPI.OperationResult SaveDayInterval(DayInterval item)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_схемы_работы);
			return SKDDatabaseService.DayIntervalTranslator.Save(item);
		}
		public FiresecAPI.OperationResult MarkDeletedDayInterval(Guid uid)
		{
			AddSKDJournalMessage(JournalEventNameType.Редактирование_схемы_работы);
			return SKDDatabaseService.DayIntervalTranslator.MarkDeleted(uid);
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

		public FiresecAPI.OperationResult<FiresecAPI.SKD.TimeTrackException> GetTimeTrackException(DateTime dateTime, Guid employeeUID)
		{
			return SKDDatabaseService.TimeTrackExceptionTranslator.Get(dateTime, employeeUID);
		}
		public FiresecAPI.OperationResult SaveTimeTrackException(FiresecAPI.SKD.TimeTrackException item)
		{
			AddSKDJournalMessage(JournalEventNameType.Внесение_оправдательного_документа);
			return SKDDatabaseService.TimeTrackExceptionTranslator.Save(item);
		}
	}
}