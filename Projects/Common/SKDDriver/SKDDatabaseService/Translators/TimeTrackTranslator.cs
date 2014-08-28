using System;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using OperationResult = FiresecAPI.OperationResult;
using FiresecAPI.SKD;
using FiresecAPI;
using System.Collections.Generic;

namespace SKDDriver.Translators
{
	public class TimeTrackTranslator
	{
		DataAccess.SKDDataContext Context;

		public TimeTrackTranslator(DataAccess.SKDDataContext context)
		{
			Context = context;
		}

		public OperationResult AddPassJournal(Guid employeeUID, Guid zoneUID)
		{
			InvalidatePassJournal();

			try
			{
				var exitPassJournal = Context.PassJournals.FirstOrDefault(x => x.EmployeeUID == employeeUID && x.ExitTime == null);
				if (exitPassJournal != null)
				{
					exitPassJournal.ExitTime = DateTime.Now;
				}
				var enterPassJournal = new DataAccess.PassJournal();
				enterPassJournal.UID = Guid.NewGuid();
				enterPassJournal.EmployeeUID = employeeUID;
				enterPassJournal.ZoneUID = zoneUID;
				enterPassJournal.EnterTime = DateTime.Now;
				enterPassJournal.ExitTime = null;
				Context.PassJournals.InsertOnSubmit(enterPassJournal);
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		void InvalidatePassJournal()
		{
			try
			{
				var hasChanges = false;
				var emptyExitPassJournals = Context.PassJournals.Where(x => x.ExitTime == null);
				foreach (var emptyExitPassJournal in emptyExitPassJournals)
				{
					var enterTime = emptyExitPassJournal.EnterTime;
					var nowTime = DateTime.Now;
					if (nowTime.Date > enterTime.Date)
					{
						emptyExitPassJournal.EnterTime = new DateTime(enterTime.Year, enterTime.Month, enterTime.Day, 23, 59, 59);
						hasChanges = true;
					}
				}
				if (hasChanges)
				{
					Context.SubmitChanges();
				}
			}
			catch { }
		}

		public void InsertPassJournalTestData()
		{
			var employees = SKDDatabaseService.EmployeeTranslator.GetList(new EmployeeFilter()).Result;
			var zoneUID = SKDManager.Zones.FirstOrDefault().UID;

			foreach (var passJournal in Context.PassJournals)
			{
				Context.PassJournals.DeleteOnSubmit(passJournal);
			}

			var random = new Random();
			foreach (var employee in employees)
			{
				for (int day = 0; day < 100; day++)
				{
					var dateTime = DateTime.Now.AddDays(-day);

					var seconds = new List<int>();
					var count = random.Next(0, 5);
					for (int i = 0; i < count * 2; i++)
					{
						var totalSeconds = random.Next(0, 24 * 60 * 60);
						seconds.Add(totalSeconds);
					}
					seconds.Sort();

					for (int i = 0; i < count * 2; i += 2)
					{
						var startTimeSpan = TimeSpan.FromSeconds(seconds[i]);
						var endTimeSpan = TimeSpan.FromSeconds(seconds[i + 1]);

						var passJournal = new DataAccess.PassJournal();
						passJournal.UID = Guid.NewGuid();
						passJournal.EmployeeUID = employee.UID;
						passJournal.ZoneUID = zoneUID;
						passJournal.EnterTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, startTimeSpan.Hours, startTimeSpan.Minutes, startTimeSpan.Seconds);
						passJournal.ExitTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, endTimeSpan.Hours, endTimeSpan.Minutes, endTimeSpan.Seconds);
						Context.PassJournals.InsertOnSubmit(passJournal);
					}
				}
			}

			Context.SubmitChanges();
		}

		IEnumerable<DataAccess.Holiday> Holidays { get; set; }

		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			InvalidatePassJournal();

			if (filter.OrganisationUIDs.IsNotNullOrEmpty())
				Holidays = Context.Holidays.Where(x => x.Date >= startDate && x.Date <= endDate && filter.OrganisationUIDs.Contains(x.OrganisationUID.Value) && !x.IsDeleted).ToList();
			else
				Holidays = Context.Holidays.Where(x => x.Date >= startDate && x.Date <= endDate && !x.IsDeleted).ToList();

			try
			{
				var operationResult = SKDDatabaseService.EmployeeTranslator.GetList(filter);
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

					var documentsOperationResult = SKDDatabaseService.TimeTrackDocumentTranslator.Get(shortEmployee.UID, startDate, endDate);
					if (!documentsOperationResult.HasError)
					{
						var documents = documentsOperationResult.Result;
						foreach (var document in documents)
						{
							document.TimeTrackDocumentType = TimeTrackDocumentTypesCollection.TimeTrackDocumentTypes.FirstOrDefault(x => x.Code == document.DocumentCode);
							if (document.TimeTrackDocumentType == null)
							{
								var documentTypesResult = SKDDatabaseService.TimeTrackDocumentTypeTranslator.Get(shortEmployee.OrganisationUID);
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
							for (DateTime date = document.StartDateTime; date <= document.EndDateTime; date = date.AddDays(1))
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
			var scheduleScheme = Context.ScheduleSchemes.FirstOrDefault(x => x.UID == schedule.ScheduleSchemeUID.Value && !x.IsDeleted);
			if (scheduleScheme == null)
				return new TimeTrackEmployeeResult("Не найдена схема работы");
			var days = Context.ScheduleDays.Where(x => x.ScheduleSchemeUID == scheduleScheme.UID && !x.IsDeleted).ToList();
			var scheduleZones = Context.ScheduleZones.Where(x => x.ScheduleUID == schedule.UID && !x.IsDeleted).ToList();
			var nightSettings = SKDDatabaseService.NightSettingsTranslator.GetByOrganisation(employee.OrganisationUID.Value).Result;

			var timeTrackEmployeeResult = new TimeTrackEmployeeResult();
			timeTrackEmployeeResult.ScheduleName = schedule.Name;

			for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
			{
				var dayTimeTrack = GetRealTimeTrack(employee, schedule, scheduleScheme, scheduleZones, date);
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
				intervals = Context.DayIntervalParts.Where(x => x.DayIntervalUID == dayInterval.UID && !x.IsDeleted).ToList();
			}

			TimeTrackPart nightTimeTrackPart = null;
			{
				SKDDriver.DataAccess.ScheduleDay previousDay = null;
				if (dayNo > 0)
					previousDay = days.FirstOrDefault(x => x.Number == dayNo - 1 && !x.IsDeleted);
				else
					previousDay = days.FirstOrDefault(x => x.Number == days.Count() - 1 && !x.IsDeleted);
				if (previousDay != null)
				{
					if (previousDay.DayIntervalUID != null)
					{
						var previousDayInterval = Context.DayIntervals.FirstOrDefault(x => x.UID == previousDay.DayIntervalUID && !x.IsDeleted);
						if (previousDayInterval != null)
						{
							var previousIntervals = Context.DayIntervalParts.Where(x => x.DayIntervalUID == previousDayInterval.UID && !x.IsDeleted).ToList();
							var nightInterval = previousIntervals.FirstOrDefault(x => x.EndTime > 60 * 60 * 24);
							if (nightInterval != null)
							{
								nightTimeTrackPart = new TimeTrackPart();
								nightTimeTrackPart.StartTime = new TimeSpan();
								nightTimeTrackPart.EndTime = new TimeSpan(Math.BigMul(nightInterval.EndTime - 60 * 60 * 24, 10000000));
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
					timeTrackPart.StartTime = new TimeSpan(Math.BigMul(tableInterval.BeginTime, 10000000));
					timeTrackPart.EndTime = new TimeSpan(Math.BigMul(Math.Min(tableInterval.EndTime, 60 * 60 * 24 - 1), 10000000));
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
					var reductionTimeSpan = new TimeSpan(Math.BigMul(result.HolidayReduction, 10000000));
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

		DayTimeTrack GetRealTimeTrack(DataAccess.Employee employee, DataAccess.Schedule schedule, DataAccess.ScheduleScheme scheduleScheme, IEnumerable<DataAccess.ScheduleZone> scheduleZones, DateTime date)
		{
			var dayTimeTrack = new DayTimeTrack();
			dayTimeTrack.Date = date;

			var passJournals = Context.PassJournals.Where(x => x.EmployeeUID == employee.UID && x.EnterTime != null && x.EnterTime.Date == date.Date).ToList();
			if (passJournals != null)
			{
				foreach (var passJournal in passJournals)
				{
					var scheduleZone = scheduleZones.FirstOrDefault(x => x.ZoneUID == passJournal.ZoneUID && !x.IsDeleted);
					if (scheduleZone != null)
					{
						if (passJournal.ExitTime.HasValue)
						{
							var timeTrackPart = new TimeTrackPart()
							{
								StartTime = passJournal.EnterTime.TimeOfDay,
								EndTime = passJournal.ExitTime.Value.TimeOfDay,
								ZoneUID = passJournal.ZoneUID
							};
							dayTimeTrack.RealTimeTrackParts.Add(timeTrackPart);
						}
					}
				}
			}
			dayTimeTrack.RealTimeTrackParts = dayTimeTrack.RealTimeTrackParts.OrderBy(x => x.StartTime.Ticks).ToList();

			return dayTimeTrack;
		}

		//****************************************************************************************

		//PlannedSheduleDays GetPlannedSheduleDays(DataAccess.Employee employee, DataAccess.Schedule schedule, DataAccess.ScheduleScheme scheduleScheme, IEnumerable<DataAccess.ScheduleDay> days)
		//{
		//    var plannedSheduleDays = new PlannedSheduleDays();

		//    var daysCount = 0;
		//    switch ((ScheduleSchemeType)scheduleScheme.Type)
		//    {
		//        case ScheduleSchemeType.Week:
		//            daysCount = 7;
		//            break;
		//        case ScheduleSchemeType.SlideDay:
		//            daysCount = 7;
		//            break;
		//        case ScheduleSchemeType.Month:
		//            daysCount = 31;
		//            break;
		//    }

		//    foreach (var day in days)
		//    {
		//        var plannedSheduleDay = new PlannedSheduleDay();
		//        plannedSheduleDays.PlannedSheduleDays.Add(plannedSheduleDay);
		//        plannedSheduleDay.DayNo = day.Number;
		//    }

		//    foreach (var day in days)
		//    {
		//        var plannedSheduleDay = plannedSheduleDays.PlannedSheduleDays.FirstOrDefault(x => x.DayNo == day.Number);


		//        //int dayNo = -1;
		//        //switch ((ScheduleSchemeType)scheduleScheme.Type)
		//        //{
		//        //    case ScheduleSchemeType.Week:
		//        //        dayNo = (int)date.DayOfWeek - 1;
		//        //        if (dayNo == -1)
		//        //            dayNo = 6;
		//        //        break;
		//        //    case ScheduleSchemeType.SlideDay:
		//        //        var daysCount = days.Count();
		//        //        var ticksDelta = new TimeSpan(date.Ticks - employee.ScheduleStartDate.Ticks);
		//        //        var daysDelta = Math.Abs((int)ticksDelta.TotalDays);
		//        //        dayNo = daysDelta % daysCount;
		//        //        break;
		//        //    case ScheduleSchemeType.Month:
		//        //        dayNo = (int)date.Day - 1;
		//        //        break;
		//        //}
		//        //var day = days.FirstOrDefault(x => x.Number == dayNo);
		//        //if (day == null)
		//        //    return new PlannedTimeTrackPart("Не найден день");

		//        var intervals = new List<DataAccess.DayIntervalPart>();
		//        DataAccess.DayInterval dayInterval = null;
		//        if (day.DayIntervalUID != null)
		//        {
		//            dayInterval = Context.DayIntervals.FirstOrDefault(x => x.UID == day.DayIntervalUID && !x.IsDeleted);
		//            if (dayInterval == null)
		//                return new PlannedSheduleDays("Не найден дневной интервал");
		//            intervals = Context.DayIntervalParts.Where(x => x.DayIntervalUID == dayInterval.UID && !x.IsDeleted).ToList();
		//        }

		//        TimeTrackPart nightTimeTrackPart = null;
		//        {
		//            SKDDriver.DataAccess.ScheduleDay previousDay = null;
		//            if (dayNo > 0)
		//                previousDay = days.FirstOrDefault(x => x.Number == dayNo - 1 && !x.IsDeleted);
		//            else
		//                previousDay = days.FirstOrDefault(x => x.Number == days.Count() - 1 && !x.IsDeleted);
		//            if (previousDay != null)
		//            {
		//                if (previousDay.DayIntervalUID != null)
		//                {
		//                    var previousDayInterval = Context.DayIntervals.FirstOrDefault(x => x.UID == previousDay.DayIntervalUID && !x.IsDeleted);
		//                    if (previousDayInterval != null)
		//                    {
		//                        var previousIntervals = Context.DayIntervalParts.Where(x => x.DayIntervalUID == previousDayInterval.UID && !x.IsDeleted).ToList();
		//                        var nightInterval = previousIntervals.FirstOrDefault(x => x.EndTime > 60 * 60 * 24);
		//                        if (nightInterval != null)
		//                        {
		//                            nightTimeTrackPart = new TimeTrackPart();
		//                            nightTimeTrackPart.StartTime = new TimeSpan();
		//                            nightTimeTrackPart.EndTime = new TimeSpan(Math.BigMul(nightInterval.EndTime - 60 * 60 * 24, 10000000));
		//                        }
		//                    }
		//                }
		//            }
		//        }

		//        var result = new PlannedTimeTrackPart();
		//        if (dayInterval != null)
		//            result.SlideTime = TimeSpan.FromSeconds(dayInterval.SlideTime);

		//        return plannedSheduleDays;
		//    }
		//}
	}
}