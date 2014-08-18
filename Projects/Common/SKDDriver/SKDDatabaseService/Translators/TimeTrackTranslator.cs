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

		public OperationResult<TimeTrackResult> GetTimeTracks(EmployeeFilter filter, DateTime startDate, DateTime endDate)
		{
			InvalidatePassJournal();

			try
			{
				var operationResult = SKDDatabaseService.EmployeeTranslator.GetList(filter);
				if (operationResult.HasError)
					return new OperationResult<TimeTrackResult>(operationResult.Error);

				var timeTrackResult = new TimeTrackResult();

				foreach (var shortEmployee in operationResult.Result)
				{
					var timeTrackEmployeeResult = new TimeTrackEmployeeResult();
					timeTrackResult.TimeTrackEmployeeResults.Add(timeTrackEmployeeResult);
					timeTrackEmployeeResult.ShortEmployee = shortEmployee;

					var employee = Context.Employees.FirstOrDefault(x => x.UID == shortEmployee.UID && !x.IsDeleted);
					if (employee == null)
						return new OperationResult<TimeTrackResult>("Не найден сотрудник");
					var schedule = Context.Schedules.FirstOrDefault(x => x.UID == employee.ScheduleUID && !x.IsDeleted);
					if (schedule == null)
						return new OperationResult<TimeTrackResult>("Не найден график");
					var scheduleScheme = Context.ScheduleSchemes.FirstOrDefault(x => x.UID == schedule.ScheduleSchemeUID.Value && !x.IsDeleted);
					if (scheduleScheme == null)
						return new OperationResult<TimeTrackResult>("Не найдена схема работы");
					var days = Context.ScheduleDays.Where(x => x.ScheduleSchemeUID == scheduleScheme.UID && !x.IsDeleted).ToList();
					var scheduleZones = Context.ScheduleZones.Where(x => x.ScheduleUID == schedule.UID && !x.IsDeleted).ToList();
					var holidaySettings = SKDDatabaseService.HolidaySettingsTranslator.GetByOrganisation(employee.OrganisationUID.Value).Result;

					var plannedTimeTrackCollection = new PlannedTimeTrackCollection();
					timeTrackEmployeeResult.IsIgnoreHoliday = schedule.IsIgnoreHoliday;
					timeTrackEmployeeResult.IsOnlyFirstEnter = schedule.IsOnlyFirstEnter;
					timeTrackEmployeeResult.AllowedLate = TimeSpan.FromSeconds(schedule.AllowedLate);
					timeTrackEmployeeResult.AllowedEarlyLeave = TimeSpan.FromSeconds(schedule.AllowedEarlyLeave);

					for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
					{
						var dayTimeTrack = GetTimeTrack(employee, schedule, scheduleScheme, scheduleZones, date);
						timeTrackEmployeeResult.DayTimeTracks.Add(dayTimeTrack);
						dayTimeTrack.HolidaySettings = holidaySettings;

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
					}
				}
				return new OperationResult<TimeTrackResult> { Result = timeTrackResult };
			}
			catch (Exception e)
			{
				return new OperationResult<TimeTrackResult>(e.Message);
			}
		}

		DayTimeTrack GetTimeTrack(DataAccess.Employee employee, DataAccess.Schedule schedule, DataAccess.ScheduleScheme scheduleScheme, IEnumerable<DataAccess.ScheduleZone> scheduleZones, DateTime date)
		{
			var passJournals = Context.PassJournals.Where(x => x.EmployeeUID == employee.UID && x.EnterTime != null && x.EnterTime.Date == date.Date).ToList();
			if (passJournals == null)
				passJournals = new List<DataAccess.PassJournal>();

			var dayTimeTrack = new DayTimeTrack();
			dayTimeTrack.EmployeeUID = employee.UID;
			dayTimeTrack.Date = date;

			foreach (var passJournal in passJournals)
			{
				var scheduleZone = scheduleZones.FirstOrDefault(x => x.ZoneUID == passJournal.ZoneUID && !x.IsDeleted);
				if (scheduleZone != null)
				{
					if (passJournal.ExitTime.HasValue)
					{
						var timeTrackPart = new TimeTrackPart();
						timeTrackPart.StartTime = passJournal.EnterTime.TimeOfDay;
						timeTrackPart.EndTime = passJournal.ExitTime.Value.TimeOfDay;
						timeTrackPart.ZoneUID = passJournal.ZoneUID;
						dayTimeTrack.RealTimeTrackParts.Add(timeTrackPart);
					}
				}
			}
			dayTimeTrack.RealTimeTrackParts = dayTimeTrack.RealTimeTrackParts.OrderBy(x => x.StartTime.Ticks).ToList();

			var operationResult = SKDDatabaseService.TimeTrackDocumentTranslator.Get(date, employee.UID);
			if (!operationResult.HasError)
			{
				dayTimeTrack.TimeTrackDocument = operationResult.Result;
			}

			return dayTimeTrack;
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
					var ticksDelta = new TimeSpan(date.Ticks - employee.ScheduleStartDate.Ticks);
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

			List<DataAccess.DayIntervalPart> intervals = new List<DataAccess.DayIntervalPart>();
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
				var holiday = Context.Holidays.FirstOrDefault(x => x.Date == date && x.Type == (int)HolidayType.Holiday && x.OrganisationUID == employee.OrganisationUID && !x.IsDeleted);
				if (holiday != null)
				{
					result.IsHoliday = true;
				}
				holiday = Context.Holidays.FirstOrDefault(x => x.Date == date && x.Type == (int)HolidayType.BeforeHoliday && x.OrganisationUID == employee.OrganisationUID && !x.IsDeleted);
				if (holiday != null)
				{
					result.HolidayReduction = holiday.Reduction;
				}
				holiday = Context.Holidays.FirstOrDefault(x => x.TransferDate == date && x.Type == (int)HolidayType.WorkingHoliday && x.OrganisationUID == employee.OrganisationUID && !x.IsDeleted);
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
				var reductionTimeSpan = new TimeSpan(Math.BigMul(result.HolidayReduction, 10000000));
				var lastTimeTrack = result.TimeTrackParts.LastOrDefault();
				if (lastTimeTrack != null)
				{
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