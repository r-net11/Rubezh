using Infrastructure.Common.Windows;
using RubezhAPI;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace RubezhDAL.DataClasses
{
	public class TimeTrackTranslator
	{

		public TimeTrackTranslator(RubezhDAL.DataClasses.DbService databaseService)
		{
			Context = databaseService.Context;
			DbService = databaseService;
		}

		DbService DbService;
		DatabaseContext Context;

		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var operationResult = DbService.EmployeeTranslator.ShortTranslator.Get(filter);
				if (operationResult.HasError)
					throw new Exception(operationResult.Error);
				var employees = DbService.EmployeeTranslator.GetFilteredTableItems(filter, GetEmployeeTableItems());
				var employeeUIDs = employees.Select(x => x.UID).ToList();
				var passJournals = Context.PassJournals.Where(x => x.EmployeeUID != null && employeeUIDs.Contains(x.EmployeeUID.Value)).ToList();
				var timeTrackResult = new TimeTrackResult();
				foreach (var employee in employees)
				{
					var timeTrackEmployeeResult = GetEmployeeTimeTrack(employee, startDate, endDate, passJournals);
					timeTrackEmployeeResult.ShortEmployee = DbService.EmployeeTranslator.ShortTranslator.Translate(employee);
					if (timeTrackEmployeeResult.Error != null)
					{
						for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
						{
							var dayTimeTrack = new DayTimeTrack();
							dayTimeTrack.Error = timeTrackEmployeeResult.Error;
							dayTimeTrack.Date = date;
							timeTrackEmployeeResult.DayTimeTracks.Add(dayTimeTrack);
						}
					}

					var documentsOperationResult = DbService.TimeTrackDocumentTranslator.Get(employee.UID, startDate, endDate, employee.TimeTrackDocuments);
					if (!documentsOperationResult.HasError)
					{
						var documents = documentsOperationResult.Result;
						foreach (var document in documents)
						{
							document.TimeTrackDocumentType = TimeTrackDocumentTypesCollection.TimeTrackDocumentTypes.FirstOrDefault(x => x.Code == document.DocumentCode);
							if (document.TimeTrackDocumentType == null)
							{
								var documentType = employee.Organisation.TimeTrackDocumnetTypes.FirstOrDefault(x => x.DocumentCode == document.DocumentCode);
								if (documentType != null)
								{
									document.TimeTrackDocumentType = DbService.TimeTrackDocumentTypeTranslator.Translate(documentType);
								}
							}
							if (document.TimeTrackDocumentType != null)
							{
								timeTrackEmployeeResult.Documents.Add(document);
							}
						}

						foreach (var document in timeTrackEmployeeResult.Documents)
						{
							for (DateTime date = document.StartDateTime; date < new DateTime(document.EndDateTime.Year, document.EndDateTime.Month, document.EndDateTime.Day).AddDays(1); date = date.AddDays(1))
							{
								var dayTimeTracks = timeTrackEmployeeResult.DayTimeTracks.FirstOrDefault(x => x.Date.Date == date.Date);
								if (dayTimeTracks != null)
								{
									dayTimeTracks.Documents.Add(document);
								}
							}
						}
					}

					timeTrackResult.TimeTrackEmployeeResults.Add(timeTrackEmployeeResult);
				}
				return timeTrackResult;
			});
		}

		public Stream GetTimeTracksStream(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			var timeTracksResult = GetTimeTracks(filter, startDate, endDate).Result;
			var serializer = new DataContractSerializer(typeof(TimeTrackResult));
			var folderName = AppDataFolderHelper.GetFolder("TempServer");
			if (!Directory.Exists(folderName))
				Directory.CreateDirectory(folderName);
			var fileName = Path.Combine(folderName, "TimeTrackResult.xml");
			using (var fileStream = File.Open(fileName, FileMode.Create))
			{
				serializer.WriteObject(fileStream, timeTracksResult);
			}
			return new FileStream(fileName, FileMode.Open, FileAccess.Read);
		}

		IQueryable<Employee> GetEmployeeTableItems()
		{
			return DbService.EmployeeTranslator.ShortTranslator.GetTableItems()
				.Include(x => x.Organisation.NightSettings)
				.Include(x => x.Organisation.Holidays)
				.Include(x => x.Organisation.TimeTrackDocumnetTypes)
				.Include(x => x.TimeTrackDocuments)
				.Include(x => x.Schedule.ScheduleScheme.ScheduleDays.Select(y => y.DayInterval.DayIntervalParts))
				.Include(x => x.Schedule.ScheduleZones);

		}
		TimeTrackEmployeeResult GetEmployeeTimeTrack(Employee employee, DateTime startDate, DateTime endDate, List<PassJournal> passJournals)
		{

			if (employee == null)
				return new TimeTrackEmployeeResult("Не найден сотрудник");

			var schedule = employee.Schedule;
			if (schedule == null || schedule.IsDeleted)
				return new TimeTrackEmployeeResult("Не найден график");

			var scheduleScheme = employee.Schedule.ScheduleScheme;
			if (scheduleScheme == null || scheduleScheme.IsDeleted)
				return new TimeTrackEmployeeResult("Не найдена схема работы");

			var days = scheduleScheme.ScheduleDays;
			var scheduleZones = schedule.ScheduleZones;
			var nightSettings = employee.Organisation.NightSettings.FirstOrDefault();

			var timeTrackEmployeeResult = new TimeTrackEmployeeResult();
			timeTrackEmployeeResult.ScheduleName = schedule.Name;

			var holidays = employee.Organisation.Holidays.Where(x => x.Date >= startDate && x.Date <= endDate);

			for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
			{
				if (employee.ScheduleStartDate.Date > date.Date)
				{
					timeTrackEmployeeResult.DayTimeTracks.Add(new DayTimeTrack("До начала действия графика") { Date = date });
					continue;
				}

				var dayTimeTrack = DbService.PassJournalTranslator.GetRealTimeTrack(employee.UID, scheduleZones, date, passJournals);
				dayTimeTrack.NightSettings = nightSettings != null ? DbService.NightSettingTranslator.Transalte(nightSettings) : null;
				dayTimeTrack.IsIgnoreHoliday = schedule.IsIgnoreHoliday;
				dayTimeTrack.IsOnlyFirstEnter = schedule.IsOnlyFirstEnter;
				dayTimeTrack.AllowedLate = schedule.AllowedLateTimeSpan;
				dayTimeTrack.AllowedEarlyLeave = schedule.AllowedEarlyLeaveTimeSpan;

				var realDate = date;
				var ignoreHolidays = false;
				var holiday = holidays.FirstOrDefault(x => x.Date == date && x.Type == (int)HolidayType.WorkingHoliday && !x.IsDeleted);
				if (holiday != null)
				{
					if (holiday.TransferDate.HasValue)
					{
						realDate = holiday.TransferDate.Value;
						ignoreHolidays = true;
					}
				}

				var plannedTimeTrackPart = GetPlannedTimeTrackPart(employee, schedule, scheduleScheme, days, realDate, ignoreHolidays, holidays);
				dayTimeTrack.PlannedTimeTrackParts = plannedTimeTrackPart.TimeTrackParts;
				dayTimeTrack.IsHoliday = plannedTimeTrackPart.IsHoliday;
				dayTimeTrack.HolidayReduction = plannedTimeTrackPart.HolidayReduction;
				dayTimeTrack.SlideTime = plannedTimeTrackPart.SlideTime;
				dayTimeTrack.Error = plannedTimeTrackPart.Error;

				timeTrackEmployeeResult.DayTimeTracks.Add(dayTimeTrack);
			}
			return timeTrackEmployeeResult;
		}

		PlannedTimeTrackPart GetPlannedTimeTrackPart(Employee employee, Schedule schedule, ScheduleScheme scheduleScheme, IEnumerable<ScheduleDay> days, DateTime date, bool ignoreHolidays, IEnumerable<Holiday> holidays)
		{
			var scheduleSchemeType = (ScheduleSchemeType)scheduleScheme.Type;

			int dayNo = -1;
			switch (scheduleSchemeType)
			{
				case ScheduleSchemeType.Week:
					dayNo = (int)date.DayOfWeek - 1;
					if (dayNo == -1)
						dayNo = 6;
					break;
				case ScheduleSchemeType.SlideDay:
					var daysCount = days.Count();
					var ticksDelta = new TimeSpan(date.Date.Ticks - employee.ScheduleStartDate.Date.Ticks);
					var daysDelta = Math.Abs((int)ticksDelta.TotalDays);
					dayNo = daysDelta % daysCount;
					break;
				case ScheduleSchemeType.Month:
					dayNo = (int)date.Day - 1;
					break;
			}
			var day = days.FirstOrDefault(x => x.Number == dayNo);
			if (day == null)
				return new PlannedTimeTrackPart("Не найден день");

			var dayInterval = day.DayInterval;
			if (day.DayInterval == null || day.DayInterval.IsDeleted)
				return new PlannedTimeTrackPart("Не найден дневной интервал");

			TimeTrackPart nightTimeTrackPart = null;
			ScheduleDay previousDay = null;
			if (dayNo > 0)
				previousDay = days.FirstOrDefault(x => x.Number == dayNo - 1);
			else
				previousDay = days.FirstOrDefault(x => x.Number == days.Count() - 1);
			if (previousDay != null)
			{
				if (previousDay.DayIntervalUID != null)
				{
					var previousDayInterval = previousDay.DayInterval;
					if (previousDayInterval != null && !previousDayInterval.IsDeleted)
					{
						var previousIntervals = previousDayInterval.DayIntervalParts;
						var nightInterval = previousIntervals.FirstOrDefault(x => x.EndTimeTotalSeconds < x.BeginTimeTotalSeconds);
						if (nightInterval != null)
						{
							nightTimeTrackPart = new TimeTrackPart();
							nightTimeTrackPart.StartTime = new TimeSpan();
							nightTimeTrackPart.EndTime = new TimeSpan(0, 0, 0, (int)nightInterval.EndTimeTotalSeconds);
							nightTimeTrackPart.StartsInPreviousDay = true;
							nightTimeTrackPart.DayName = previousDayInterval.Name;
						}
					}
				}
			}

			var result = new PlannedTimeTrackPart();
			if (dayInterval != null)
				result.SlideTime = dayInterval.SlideTimeSpan;

			if (!ignoreHolidays)
			{
				if (!schedule.IsIgnoreHoliday)
				{
					var holiday = holidays.FirstOrDefault(x => x.Date == date && x.Type == (int)HolidayType.Holiday && !x.IsDeleted);
					if (holiday != null)
					{
						result.IsHoliday = true;
					}
					holiday = holidays.FirstOrDefault(x => x.Date == date && x.Type == (int)HolidayType.BeforeHoliday && !x.IsDeleted);
					if (holiday != null)
					{
						result.HolidayReduction = (int)holiday.ReductionTimeSpan.TotalSeconds;
					}
					holiday = holidays.FirstOrDefault(x => x.TransferDate == date && x.Type == (int)HolidayType.WorkingHoliday && !x.IsDeleted);
					if (holiday != null)
					{
						result.IsHoliday = true;
					}
				}
			}

			if (!result.IsHoliday)
			{
				foreach (var tableInterval in dayInterval.DayIntervalParts)
				{
					var timeTrackPart = new TimeTrackPart();
					timeTrackPart.StartTime = new TimeSpan(0, 0, 0, (int)tableInterval.BeginTimeTotalSeconds);
					if (tableInterval.EndTimeTotalSeconds < tableInterval.BeginTimeTotalSeconds)
						timeTrackPart.EndsInNextDay = true;
					if (timeTrackPart.EndsInNextDay)
						timeTrackPart.EndTime = new TimeSpan(24, 0, 0);
					else
						timeTrackPart.EndTime = new TimeSpan(0, 0, 0, (int)tableInterval.EndTimeTotalSeconds);
					timeTrackPart.DayName = dayInterval.Name;
					result.TimeTrackParts.Add(timeTrackPart);
				}
				if (nightTimeTrackPart != null)
				{
					result.TimeTrackParts.Add(nightTimeTrackPart);
				}
				result.TimeTrackParts = result.TimeTrackParts.OrderBy(x => x.StartTime.Ticks).ToList();
			}
			if (result.HolidayReduction > 0)
			{
				var reductionTimeSpan = TimeSpan.FromSeconds(result.HolidayReduction);
				switch (result.SlideTime.TotalSeconds > reductionTimeSpan.TotalSeconds)
				{
					case true: result.SlideTime -= reductionTimeSpan; break;
					case false: result.SlideTime = TimeSpan.Zero; break;
				}
				while (reductionTimeSpan.TotalSeconds > 0 && result.TimeTrackParts.Count > 0)
				{
					var lastTimeTrack = result.TimeTrackParts.LastOrDefault();
					if (lastTimeTrack != null)
					{
						if (lastTimeTrack.Delta.TotalHours > reductionTimeSpan.TotalHours)
						{
							lastTimeTrack.EndTime = lastTimeTrack.EndTime.Subtract(reductionTimeSpan);
							reductionTimeSpan = TimeSpan.Zero;
						}
						else
						{
							result.TimeTrackParts.Remove(lastTimeTrack);
							reductionTimeSpan -= lastTimeTrack.Delta;
						}
					}
				}
			}
			return result;
		}
	}
}