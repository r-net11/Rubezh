﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;

namespace SKDDriver.Translators
{
	public class TimeTrackTranslator
	{
		SKDDatabaseService DatabaseService;
		DataAccess.SKDDataContext Context;
		IEnumerable<DataAccess.Holiday> Holidays { get; set; }

		public TimeTrackTranslator(SKDDatabaseService databaseService)
		{
			DatabaseService = databaseService;
			Context = databaseService.Context;
		}

		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			using (var passJournalTranslator = new PassJournalTranslator())
			{
				passJournalTranslator.InvalidatePassJournal();
			}

			if (filter.OrganisationUIDs.IsNotNullOrEmpty())
				Holidays = Context.Holidays.Where(x => x.Date >= startDate && x.Date <= endDate && filter.OrganisationUIDs.Contains(x.OrganisationUID.Value) && !x.IsDeleted).ToList();
			else
				Holidays = Context.Holidays.Where(x => x.Date >= startDate && x.Date <= endDate && !x.IsDeleted).ToList();

			try
			{
				var operationResult = DatabaseService.EmployeeTranslator.GetList(filter);
				if (operationResult.HasError)
					return new OperationResult<TimeTrackResult>(operationResult.Error);

				var timeTrackResult = new TimeTrackResult();
				foreach (var shortEmployee in operationResult.Result)
				{
					var timeTrackEmployeeResult = GetEmployeeTimeTrack(shortEmployee, startDate, endDate);
					timeTrackEmployeeResult.ShortEmployee = shortEmployee;
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

					var documentsOperationResult = DatabaseService.TimeTrackDocumentTranslator.Get(shortEmployee.UID, startDate, endDate);
					if (!documentsOperationResult.HasError)
					{
						var documents = documentsOperationResult.Result;
						foreach (var document in documents)
						{
							document.TimeTrackDocumentType = TimeTrackDocumentTypesCollection.TimeTrackDocumentTypes.FirstOrDefault(x => x.Code == document.DocumentCode);
							if (document.TimeTrackDocumentType == null)
							{
								var documentTypesResult = DatabaseService.TimeTrackDocumentTypeTranslator.Get(shortEmployee.OrganisationUID);
								if (documentTypesResult.Result != null)
								{
									document.TimeTrackDocumentType = documentTypesResult.Result.FirstOrDefault(x => x.Code == document.DocumentCode);
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
				return new OperationResult<TimeTrackResult> { Result = timeTrackResult };
			}
			catch (Exception e)
			{
				return new OperationResult<TimeTrackResult>(e.Message);
			}
		}

		TimeTrackEmployeeResult GetEmployeeTimeTrack(ShortEmployee shortEmployee, DateTime startDate, DateTime endDate)
		{
			var employee = Context.Employees.FirstOrDefault(x => x.UID == shortEmployee.UID && !x.IsDeleted);
			if (employee == null)
				return new TimeTrackEmployeeResult("Не найден сотрудник");

			var schedule = Context.Schedules.FirstOrDefault(x => x.UID == employee.ScheduleUID && !x.IsDeleted);
			if (schedule == null)
				return new TimeTrackEmployeeResult("Не найден график");
			if (schedule.ScheduleSchemeUID == null)
				return new TimeTrackEmployeeResult("Не найдена схема работы");
			var scheduleScheme = Context.ScheduleSchemes.FirstOrDefault(x => x.UID == schedule.ScheduleSchemeUID.Value && !x.IsDeleted);
			if (scheduleScheme == null)
				return new TimeTrackEmployeeResult("Не найдена схема работы");
			var days = Context.ScheduleDays.Where(x => x.ScheduleSchemeUID == scheduleScheme.UID).ToList();
			var scheduleZones = Context.ScheduleZones.Where(x => x.ScheduleUID == schedule.UID).ToList();
			var nightSettings = DatabaseService.NightSettingsTranslator.GetByOrganisation(employee.OrganisationUID.Value).Result;

			var timeTrackEmployeeResult = new TimeTrackEmployeeResult();
			timeTrackEmployeeResult.ScheduleName = schedule.Name;

			using (var passJournalTranslator = new PassJournalTranslator())
			{
				for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
				{
					if (employee.ScheduleStartDate > date)
					{
						timeTrackEmployeeResult.DayTimeTracks.Add(new DayTimeTrack("До начала действия графика") { Date = date });
						continue;
					}

					var dayTimeTrack = passJournalTranslator.GetRealTimeTrack(employee, schedule, scheduleScheme, scheduleZones, date);
					dayTimeTrack.NightSettings = nightSettings;
					dayTimeTrack.IsIgnoreHoliday = schedule.IsIgnoreHoliday;
					dayTimeTrack.IsOnlyFirstEnter = schedule.IsOnlyFirstEnter;
					dayTimeTrack.AllowedLate = TimeSpan.FromSeconds(schedule.AllowedLate);
					dayTimeTrack.AllowedEarlyLeave = TimeSpan.FromSeconds(schedule.AllowedEarlyLeave);

					var plannedTimeTrackPart = GetPlannedTimeTrackPart(employee, schedule, scheduleScheme, days, date);
					dayTimeTrack.PlannedTimeTrackParts = plannedTimeTrackPart.TimeTrackParts;
					dayTimeTrack.IsHoliday = plannedTimeTrackPart.IsHoliday;
					dayTimeTrack.HolidayReduction = plannedTimeTrackPart.HolidayReduction;
					dayTimeTrack.SlideTime = plannedTimeTrackPart.SlideTime;
					dayTimeTrack.Error = plannedTimeTrackPart.Error;

					timeTrackEmployeeResult.DayTimeTracks.Add(dayTimeTrack);
				}
			}
			return timeTrackEmployeeResult;
		}

		PlannedTimeTrackPart GetPlannedTimeTrackPart(DataAccess.Employee employee, DataAccess.Schedule schedule, DataAccess.ScheduleScheme scheduleScheme, IEnumerable<DataAccess.ScheduleDay> days, DateTime date)
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

			var intervals = new List<DataAccess.DayIntervalPart>();
			DataAccess.DayInterval dayInterval = null;
			if (day.DayIntervalUID != null)
			{
				dayInterval = Context.DayIntervals.FirstOrDefault(x => x.UID == day.DayIntervalUID && !x.IsDeleted);
				if (dayInterval == null)
					return new PlannedTimeTrackPart("Не найден дневной интервал");
				intervals = Context.DayIntervalParts.Where(x => x.DayIntervalUID == dayInterval.UID).ToList();
			}

			TimeTrackPart nightTimeTrackPart = null;
			{
				SKDDriver.DataAccess.ScheduleDay previousDay = null;
				if (dayNo > 0)
					previousDay = days.FirstOrDefault(x => x.Number == dayNo - 1);
				else
					previousDay = days.FirstOrDefault(x => x.Number == days.Count() - 1);
				if (previousDay != null)
				{
					if (previousDay.DayIntervalUID != null)
					{
						var previousDayInterval = Context.DayIntervals.FirstOrDefault(x => x.UID == previousDay.DayIntervalUID && !x.IsDeleted);
						if (previousDayInterval != null)
						{
							var previousIntervals = Context.DayIntervalParts.Where(x => x.DayIntervalUID == previousDayInterval.UID).ToList();
							var nightInterval = previousIntervals.FirstOrDefault(x => x.EndTime > 60 * 60 * 24);
							if (nightInterval != null)
							{
								nightTimeTrackPart = new TimeTrackPart();
								nightTimeTrackPart.StartTime = new TimeSpan();
								nightTimeTrackPart.EndTime = TimeSpan.FromSeconds(nightInterval.EndTime - 60 * 60 * 24);
								nightTimeTrackPart.StartsInPreviousDay = true;
								nightTimeTrackPart.DayName = previousDayInterval.Name;
							}
						}
					}
				}
			}

			var result = new PlannedTimeTrackPart();
			if (dayInterval != null)
				result.SlideTime = TimeSpan.FromSeconds(dayInterval.SlideTime);

			if (!schedule.IsIgnoreHoliday)
			{
				var holiday = Holidays.FirstOrDefault(x => x.Date == date && x.Type == (int)HolidayType.Holiday && x.OrganisationUID == employee.OrganisationUID && !x.IsDeleted);
				if (holiday != null)
				{
					result.IsHoliday = true;
				}
				holiday = Holidays.FirstOrDefault(x => x.Date == date && x.Type == (int)HolidayType.BeforeHoliday && x.OrganisationUID == employee.OrganisationUID && !x.IsDeleted);
				if (holiday != null)
				{
					result.HolidayReduction = holiday.Reduction;
				}
				holiday = Holidays.FirstOrDefault(x => x.TransferDate == date && x.Type == (int)HolidayType.WorkingHoliday && x.OrganisationUID == employee.OrganisationUID && !x.IsDeleted);
				if (holiday != null)
				{
					result.IsHoliday = true;
				}
			}

			if (!result.IsHoliday)
			{
				foreach (var tableInterval in intervals)
				{
					var timeTrackPart = new TimeTrackPart();
					timeTrackPart.StartTime = TimeSpan.FromSeconds(tableInterval.BeginTime);
					timeTrackPart.EndTime = TimeSpan.FromSeconds(Math.Min(tableInterval.EndTime, 60 * 60 * 24 - 1));
					if (tableInterval.EndTime > 60 * 60 * 24)
						timeTrackPart.EndsInNextDay = true;
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
				var lastTimeTrack = result.TimeTrackParts.LastOrDefault();
				if (lastTimeTrack != null)
				{
					var reductionTimeSpan = TimeSpan.FromSeconds(result.HolidayReduction);
					if (lastTimeTrack.Delta.TotalHours > reductionTimeSpan.TotalHours)
					{
						lastTimeTrack.EndTime = lastTimeTrack.EndTime.Subtract(reductionTimeSpan);
					}
					else
					{
						result.TimeTrackParts.Remove(lastTimeTrack);
					}
				}
			}

			return result;
		}
	}
}