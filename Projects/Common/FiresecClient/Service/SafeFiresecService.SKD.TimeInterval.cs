﻿using System;
using System.Collections.Generic;
using Common;
using FiresecAPI.SKD;
using FiresecAPI;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		public FiresecAPI.OperationResult<IEnumerable<DayInterval>> GetDayIntervals(DayIntervalFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<DayInterval>>>(() => FiresecService.GetDayIntervals(filter));
		}
		public FiresecAPI.OperationResult SaveDayInterval(DayInterval item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveDayInterval(item));
		}
		public FiresecAPI.OperationResult MarkDeletedDayInterval(DayInterval item)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedDayInterval(item.UID));
		}
	
		public FiresecAPI.OperationResult<IEnumerable<DayIntervalPart>> GetDayIntervalParts(DayIntervalPartFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<DayIntervalPart>>>(() => FiresecService.GetDayIntervalParts(filter));
		}
		public FiresecAPI.OperationResult SaveDayIntervalPart(DayIntervalPart item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveDayIntervalPart(item));
		}
		public FiresecAPI.OperationResult MarkDeletedDayIntervalPart(DayIntervalPart item)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedDayIntervalPart(item.UID));
		}

		public FiresecAPI.OperationResult<IEnumerable<Holiday>> GetHolidays(HolidayFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<Holiday>>>(() => FiresecService.GetHolidays(filter));
		}
		public FiresecAPI.OperationResult SaveHoliday(Holiday item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveHoliday(item));
		}
		public FiresecAPI.OperationResult MarkDeletedHoliday(Holiday item)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedHoliday(item.UID));
		}

		public FiresecAPI.OperationResult<IEnumerable<ScheduleScheme>> GetScheduleSchemes(ScheduleSchemeFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<ScheduleScheme>>>(() => FiresecService.GetScheduleSchemes(filter));
		}
		public FiresecAPI.OperationResult SaveScheduleScheme(ScheduleScheme item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveScheduleScheme(item));
		}
		public FiresecAPI.OperationResult MarkDeletedScheduleScheme(ScheduleScheme item)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedScheduleScheme(item.UID));
		}

		public FiresecAPI.OperationResult<IEnumerable<ScheduleDayInterval>> GetSheduleDayIntervals(ScheduleDayIntervalFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<ScheduleDayInterval>>>(() => FiresecService.GetSheduleDayIntervals(filter));
		}
		public FiresecAPI.OperationResult SaveSheduleDayInterval(ScheduleDayInterval item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveSheduleDayInterval(item));
		}
		public FiresecAPI.OperationResult MarkDeletedSheduleDayInterval(ScheduleDayInterval item)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedSheduleDayInterval(item.UID));
		}

		public FiresecAPI.OperationResult<IEnumerable<Schedule>> GetSchedules(ScheduleFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<Schedule>>>(() => FiresecService.GetSchedules(filter));
		}
		public FiresecAPI.OperationResult SaveSchedule(Schedule item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveSchedule(item));
		}
		public FiresecAPI.OperationResult MarkDeletedSchedule(Schedule item)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedSchedule(item.UID));
		}
		public FiresecAPI.OperationResult<IEnumerable<ShortSchedule>> GetScheduleShortList(ScheduleFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<ShortSchedule>>>(() => FiresecService.GetScheduleShortList(filter));
		}

		public FiresecAPI.OperationResult<IEnumerable<ScheduleZone>> GetScheduleZones(ScheduleZoneFilter filter)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<IEnumerable<ScheduleZone>>>(() => FiresecService.GetScheduleZones(filter));
		}
		public FiresecAPI.OperationResult SaveScheduleZone(ScheduleZone item)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult>(() => FiresecService.SaveScheduleZone(item));
		}
		public FiresecAPI.OperationResult MarkDeletedScheduleZone(ScheduleZone item)
		{
			return SafeContext.Execute(() => FiresecService.MarkDeletedScheduleZone(item.UID));
		}

		public FiresecAPI.OperationResult<List<TimeTrackDocument>> GetTimeTrackDocument(Guid employeeUID, DateTime startDateTime, DateTime endDateTime)
		{
			return SafeContext.Execute<FiresecAPI.OperationResult<List<TimeTrackDocument>>>(() => FiresecService.GetTimeTrackDocument(employeeUID, startDateTime, endDateTime));
		}
		public OperationResult AddTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return SafeContext.Execute(() => FiresecService.AddTimeTrackDocument(timeTrackDocument));
		}
		public OperationResult EditTimeTrackDocument(TimeTrackDocument timeTrackDocument)
		{
			return SafeContext.Execute(() => FiresecService.EditTimeTrackDocument(timeTrackDocument));
		}
		public OperationResult RemoveTimeTrackDocument(Guid timeTrackDocumentUID)
		{
			return SafeContext.Execute(() => FiresecService.RemoveTimeTrackDocument(timeTrackDocumentUID));
		}

		public OperationResult<List<TimeTrackDocumentType>> GetTimeTrackDocumentTypes(Guid organisationUID)
		{
			return SafeContext.Execute<OperationResult<List<TimeTrackDocumentType>>>(() => FiresecService.GetTimeTrackDocumentTypes(organisationUID));
		}
		public OperationResult AddTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.AddTimeTrackDocumentType(timeTrackDocumentType));
		}
		public OperationResult EditTimeTrackDocumentType(TimeTrackDocumentType timeTrackDocumentType)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.EditTimeTrackDocumentType(timeTrackDocumentType));
		}
		public OperationResult RemoveTimeTrackDocumentType(Guid timeTrackDocumentTypeUID)
		{
			return SafeContext.Execute<OperationResult>(() => FiresecService.RemoveTimeTrackDocumentType(timeTrackDocumentTypeUID));
		}
	}
}