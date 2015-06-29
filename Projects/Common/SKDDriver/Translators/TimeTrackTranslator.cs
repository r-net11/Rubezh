﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using FiresecAPI;
using FiresecAPI.SKD;
using Infrastructure.Common;

namespace SKDDriver.DataClasses
{
	public class TimeTrackTranslator
	{
		SKDDriver.DataClasses.DatabaseContext Context;
        IEnumerable<SKDDriver.DataClasses.Holiday> Holidays { get; set; }

        public TimeTrackTranslator(SKDDriver.DataClasses.DbService databaseService)
		{
			Context = databaseService.Context;
            _dbService = databaseService;
		}

		SKDDriver.DataClasses.DbService _dbService;
		DataClasses.PassJournalTranslator _PassJournalTranslator;
        List<SKDDriver.DataClasses.Employee> _Employees;
        List<SKDDriver.DataClasses.Holiday> _Holidays;
        List<SKDDriver.DataClasses.Schedule> _Schedules;
        List<SKDDriver.DataClasses.ScheduleScheme> _ScheduleSchemes;
        List<SKDDriver.DataClasses.ScheduleDay> _ScheduleDays;
        List<SKDDriver.DataClasses.ScheduleZone> _ScheduleZones;
        List<SKDDriver.DataClasses.PassJournal> _PassJournals;
        List<SKDDriver.DataClasses.DayInterval> _DayIntervals;
        List<SKDDriver.DataClasses.DayIntervalPart> _DayIntervalParts;
        List<SKDDriver.DataClasses.TimeTrackDocument> _TimeTrackDocuments;
        List<SKDDriver.DataClasses.TimeTrackDocumentType> _TimeTrackDocumentTypes;
        List<SKDDriver.DataClasses.NightSetting> _NightSettings;


		void InitializeData()
		{
			_PassJournalTranslator = _dbService.PassJournalTranslator;
			_Employees = Context.Employees.Where(x => !x.IsDeleted).ToList();
			_Holidays = Context.Holidays.Where(x => !x.IsDeleted).ToList();
			_Schedules = Context.Schedules.Where(x => !x.IsDeleted).ToList();
			_ScheduleSchemes = Context.ScheduleSchemes.Where(x => !x.IsDeleted).ToList();
			_ScheduleDays = Context.ScheduleDays.ToList();
			_ScheduleZones = Context.ScheduleZones.ToList();
			_PassJournals = _PassJournalTranslator.GetAllPassJournals();
			_DayIntervals = Context.DayIntervals.ToList();
			_DayIntervalParts = Context.DayIntervalParts.ToList();
			_TimeTrackDocuments = Context.TimeTrackDocuments.ToList();
			_TimeTrackDocumentTypes = Context.TimeTrackDocumentTypes.ToList();
			_NightSettings = Context.NightSettings.ToList();
		}

		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			InitializeData();
			//_PassJournalTranslator.InvalidatePassJournal();
			
			if (filter.OrganisationUIDs.IsNotNullOrEmpty())
				Holidays = _Holidays.Where(x => x.Date >= startDate && x.Date <= endDate && filter.OrganisationUIDs.Contains(x.OrganisationUID.Value)).ToList();
			else
				Holidays = _Holidays.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();

			try
			{
                var operationResult = _dbService.EmployeeTranslator.ShortTranslator.Get(filter);
				if (operationResult.HasError)
					return OperationResult<TimeTrackResult>.FromError(operationResult.Errors);

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

					var documentsOperationResult = _dbService.TimeTrackDocumentTranslator.Get(shortEmployee.UID, startDate, endDate, _TimeTrackDocuments);
					if (!documentsOperationResult.HasError)
					{
						var documents = documentsOperationResult.Result;
						foreach (var document in documents)
						{
							document.TimeTrackDocumentType = TimeTrackDocumentTypesCollection.TimeTrackDocumentTypes.FirstOrDefault(x => x.Code == document.DocumentCode);
							if (document.TimeTrackDocumentType == null)
							{
                                var documentTypesResult = _dbService.TimeTrackDocumentTypeTranslator.Get(shortEmployee.OrganisationUID, _TimeTrackDocumentTypes);
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
				return new OperationResult<TimeTrackResult>(timeTrackResult);
			}
			catch (Exception e)
			{
				return OperationResult<TimeTrackResult>.FromError(e.Message);
			}
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

		TimeTrackEmployeeResult GetEmployeeTimeTrack(ShortEmployee shortEmployee, DateTime startDate, DateTime endDate)
		{
			var employee = _Employees.FirstOrDefault(x => x.UID == shortEmployee.UID);
			if (employee == null)
				return new TimeTrackEmployeeResult("Не найден сотрудник");

			var schedule = _Schedules.FirstOrDefault(x => x.UID == employee.ScheduleUID);
			if (schedule == null)
				return new TimeTrackEmployeeResult("Не найден график");

			if (schedule.ScheduleSchemeUID == null)
				return new TimeTrackEmployeeResult("Не найдена схема работы");

			var scheduleScheme = _ScheduleSchemes.FirstOrDefault(x => x.UID == schedule.ScheduleSchemeUID.Value);
			if (scheduleScheme == null)
				return new TimeTrackEmployeeResult("Не найдена схема работы");

			var days = _ScheduleDays.Where(x => x.ScheduleSchemeUID == scheduleScheme.UID).ToList();
			var scheduleZones = _ScheduleZones.Where(x => x.ScheduleUID == schedule.UID).ToList();
            NightSettings nightSettings = new NightSettings();// DatabaseService.NightSettingsTranslator.GetByOrganisation(employee.OrganisationUID.Value, _NightSettings).Result;

			var timeTrackEmployeeResult = new TimeTrackEmployeeResult();
			timeTrackEmployeeResult.ScheduleName = schedule.Name;

			for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
			{
				if (employee.ScheduleStartDate.Date > date.Date)
				{
					timeTrackEmployeeResult.DayTimeTracks.Add(new DayTimeTrack("До начала действия графика") { Date = date });
					continue;
				}

				var dayTimeTrack = _PassJournalTranslator.GetRealTimeTrack(employee.UID, scheduleZones.Select(x => x.UID), date, _PassJournals);
				dayTimeTrack.NightSettings = nightSettings;
				dayTimeTrack.IsIgnoreHoliday = schedule.IsIgnoreHoliday;
				dayTimeTrack.IsOnlyFirstEnter = schedule.IsOnlyFirstEnter;
				dayTimeTrack.AllowedLate = TimeSpan.FromSeconds(schedule.AllowedLate);
				dayTimeTrack.AllowedEarlyLeave = TimeSpan.FromSeconds(schedule.AllowedEarlyLeave);

				var realDate = date;
				var ignoreHolidays = false;
				var holiday = Holidays.FirstOrDefault(x => x.Date == date && x.Type == (int)HolidayType.WorkingHoliday && x.OrganisationUID == employee.OrganisationUID && !x.IsDeleted);
				if (holiday != null)
				{
					if (holiday.TransferDate.HasValue)
					{
						realDate = holiday.TransferDate.Value;
						ignoreHolidays = true;
					}
				}

				var plannedTimeTrackPart = GetPlannedTimeTrackPart(employee, schedule, scheduleScheme, days, realDate, ignoreHolidays);
				dayTimeTrack.PlannedTimeTrackParts = plannedTimeTrackPart.TimeTrackParts;
				dayTimeTrack.IsHoliday = plannedTimeTrackPart.IsHoliday;
				dayTimeTrack.HolidayReduction = plannedTimeTrackPart.HolidayReduction;
				dayTimeTrack.SlideTime = plannedTimeTrackPart.SlideTime;
				dayTimeTrack.Error = plannedTimeTrackPart.Error;

				timeTrackEmployeeResult.DayTimeTracks.Add(dayTimeTrack);
			}
			return timeTrackEmployeeResult;
		}

        PlannedTimeTrackPart GetPlannedTimeTrackPart(SKDDriver.DataClasses.Employee employee, 
            SKDDriver.DataClasses.Schedule schedule,
            SKDDriver.DataClasses.ScheduleScheme scheduleScheme,
            IEnumerable<SKDDriver.DataClasses.ScheduleDay> days, DateTime date, bool ignoreHolidays)
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

            var intervals = new List<SKDDriver.DataClasses.DayIntervalPart>();
            SKDDriver.DataClasses.DayInterval dayInterval = null;
			if (day.DayIntervalUID != null)
			{
				dayInterval = _DayIntervals.FirstOrDefault(x => x.UID == day.DayIntervalUID && !x.IsDeleted);
				if (dayInterval == null)
					return new PlannedTimeTrackPart("Не найден дневной интервал");
				intervals = _DayIntervalParts.Where(x => x.DayIntervalUID == dayInterval.UID).ToList();
			}

			TimeTrackPart nightTimeTrackPart = null;
			{
                SKDDriver.DataClasses.ScheduleDay previousDay = null;
				if (dayNo > 0)
					previousDay = days.FirstOrDefault(x => x.Number == dayNo - 1);
				else
					previousDay = days.FirstOrDefault(x => x.Number == days.Count() - 1);
				if (previousDay != null)
				{
					if (previousDay.DayIntervalUID != null)
					{
						var previousDayInterval = _DayIntervals.FirstOrDefault(x => x.UID == previousDay.DayIntervalUID && !x.IsDeleted);
						if (previousDayInterval != null)
						{
							var previousIntervals = _DayIntervalParts.Where(x => x.DayIntervalUID == previousDayInterval.UID).ToList();
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

			if (!ignoreHolidays)
			{
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