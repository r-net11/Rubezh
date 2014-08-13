using System;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI.EmployeeTimeIntervals;
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
			var employeeUID = SKDDatabaseService.EmployeeTranslator.GetList(new EmployeeFilter()).Result.FirstOrDefault().UID;
			var zoneUID = SKDManager.Zones.FirstOrDefault().UID;

			var random = new Random();
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
					passJournal.EmployeeUID = employeeUID;
					passJournal.ZoneUID = zoneUID;
					passJournal.EnterTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, startTimeSpan.Hours, startTimeSpan.Minutes, startTimeSpan.Seconds);
					passJournal.ExitTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, endTimeSpan.Hours, endTimeSpan.Minutes, endTimeSpan.Seconds);
					Context.PassJournals.InsertOnSubmit(passJournal);
				}
			}

			Context.SubmitChanges();
		}

		public OperationResult<List<DayTimeTrack>> GetTimeTracks(Guid employeeUID, DateTime startDate, DateTime endDate)
		{
			InvalidatePassJournal();

			try
			{
				var timeTracks = new List<DayTimeTrack>();
				for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
				{
					timeTracks.Add(GetTimeTrack(employeeUID, date));
				}
				return new OperationResult<List<DayTimeTrack>> { Result = timeTracks };
			}
			catch (Exception e)
			{
				return new OperationResult<List<DayTimeTrack>>(e.Message);
			}
		}

		DayTimeTrack GetTimeTrack(Guid employeeUID, DateTime date)
		{
			var passJournals = Context.PassJournals.Where(x => x.EmployeeUID == employeeUID && x.EnterTime != null && x.EnterTime.Date == date.Date).ToList();
			if (passJournals == null)
				passJournals = new List<DataAccess.PassJournal>();

			var employee = Context.Employees.FirstOrDefault(x => x.UID == employeeUID);
			if (employee == null)
				return new DayTimeTrack("Не найден сотрудник");
			var schedule = Context.Schedules.FirstOrDefault(x => x.UID == employee.ScheduleUID);
			if (schedule == null)
				return new DayTimeTrack("Не найден график");
			var scheduleScheme = Context.ScheduleSchemes.FirstOrDefault(x => x.UID == schedule.ScheduleSchemeUID.Value);
			if (scheduleScheme == null)
				return new DayTimeTrack("Не найдена схема работы");
			var scheduleSchemeType = (FiresecAPI.EmployeeTimeIntervals.ScheduleSchemeType)scheduleScheme.Type;

			var days = Context.Days.Where(x => x.ScheduleSchemeUID == scheduleScheme.UID);
			if (days == null || days.Count() == 0)
				return new DayTimeTrack();
			int dayNo = -1;

			switch (scheduleSchemeType)
			{
				case FiresecAPI.EmployeeTimeIntervals.ScheduleSchemeType.Week:
					dayNo = (int)date.DayOfWeek - 1;
					if (dayNo == -1)
						dayNo = 6;
					break;
				case FiresecAPI.EmployeeTimeIntervals.ScheduleSchemeType.SlideDay:
					var daysCount = days.Count();
					var period = new TimeSpan(date.Ticks - employee.ScheduleStartDate.Ticks);
					dayNo = (int)Math.IEEERemainder((int)period.TotalDays, daysCount);
					break;
				case FiresecAPI.EmployeeTimeIntervals.ScheduleSchemeType.Month:
					dayNo = (int)date.Day;
					break;
			}
			var day = days.FirstOrDefault(x => x.Number == dayNo);
			if (day == null)
				return new DayTimeTrack("Не найден день");

			List<DataAccess.Interval> intervals = new List<DataAccess.Interval>();
			if (day.NamedIntervalUID != null)
			{
				var namedInterval = Context.NamedIntervals.FirstOrDefault(x => x.UID == day.NamedIntervalUID);
				if (namedInterval == null)
					return new DayTimeTrack("Не найден дневной интервал");
				intervals = Context.Intervals.Where(x => x.NamedIntervalUID == namedInterval.UID).ToList();
			}

			TimeTrackPart nightTimeTrackPart = null;
			{
				SKDDriver.DataAccess.Day previousDay = null;
				if (dayNo > 0)
					previousDay = days.FirstOrDefault(x => x.Number == dayNo - 1);
				else
					previousDay = days.FirstOrDefault(x => x.Number == days.Count() - 1);
				if (previousDay != null)
				{
					if (previousDay.NamedIntervalUID != null)
					{
						var namedInterval = Context.NamedIntervals.FirstOrDefault(x => x.UID == previousDay.NamedIntervalUID);
						if (namedInterval != null)
						{
							var previousIntervals = Context.Intervals.Where(x => x.NamedIntervalUID == namedInterval.UID).ToList();
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

			var dayTimeTrack = new DayTimeTrack();
			dayTimeTrack.EmployeeUID = employeeUID;
			dayTimeTrack.Date = date;
			dayTimeTrack.IsIgnoreHoliday = schedule.IsIgnoreHoliday;
			dayTimeTrack.IsOnlyFirstEnter = schedule.IsOnlyFirstEnter;
			dayTimeTrack.AllowedLate = TimeSpan.FromSeconds(schedule.AllowedLate);
			dayTimeTrack.AllowedEarlyLeave = TimeSpan.FromSeconds(schedule.AllowedEarlyLeave);

			if (!schedule.IsIgnoreHoliday)
			{
				var holiday = Context.Holidays.FirstOrDefault(x => x.Date == date && x.Type == (int)FiresecAPI.EmployeeTimeIntervals.HolidayType.Holiday && x.OrganisationUID == employee.OrganisationUID);
				if (holiday != null)
				{
					dayTimeTrack.IsHoliday = true;
				}
				holiday = Context.Holidays.FirstOrDefault(x => x.Date == date && x.Type == (int)FiresecAPI.EmployeeTimeIntervals.HolidayType.BeforeHoliday && x.OrganisationUID == employee.OrganisationUID);
				if (holiday != null)
				{
					dayTimeTrack.HolidayReduction = holiday.Reduction;
				}
				holiday = Context.Holidays.FirstOrDefault(x => x.TransferDate == date && x.Type == (int)FiresecAPI.EmployeeTimeIntervals.HolidayType.WorkingHoliday && x.OrganisationUID == employee.OrganisationUID);
				if (holiday != null)
				{
					dayTimeTrack.IsHoliday = true;
				}
			}

			if (!dayTimeTrack.IsHoliday)
			{
				foreach (var tableInterval in intervals)
				{
					var timeTrackPart = new TimeTrackPart();
					timeTrackPart.StartTime = new TimeSpan(Math.BigMul(tableInterval.BeginTime, 10000000));
					timeTrackPart.EndTime = new TimeSpan(Math.BigMul(Math.Min(tableInterval.EndTime, 60 * 60 * 24 - 1), 10000000));
					dayTimeTrack.PlannedTimeTrackParts.Add(timeTrackPart);
				}
				if (nightTimeTrackPart != null)
				{
					dayTimeTrack.PlannedTimeTrackParts.Add(nightTimeTrackPart);
				}
				dayTimeTrack.PlannedTimeTrackParts = dayTimeTrack.PlannedTimeTrackParts.OrderBy(x => x.StartTime.Ticks).ToList();
			}
			if (dayTimeTrack.HolidayReduction > 0)
			{
				var reductionTimeSpan = new TimeSpan(Math.BigMul(dayTimeTrack.HolidayReduction, 10000000));
				var lastTimeTrack = dayTimeTrack.PlannedTimeTrackParts.LastOrDefault();
				if (lastTimeTrack != null)
				{
					if (lastTimeTrack.Delta.TotalHours > reductionTimeSpan.TotalHours)
					{
						lastTimeTrack.EndTime = lastTimeTrack.EndTime.Subtract(reductionTimeSpan);
					}
					else
					{
						dayTimeTrack.PlannedTimeTrackParts.Remove(lastTimeTrack);
					}
				}
			}

			var scheduleZones = Context.ScheduleZones.Where(x => x.ScheduleUID == schedule.UID).ToList();
			foreach (var passJournal in passJournals)
			{
				var scheduleZone = scheduleZones.FirstOrDefault(x => x.ZoneUID == passJournal.ZoneUID);
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

			var operationResult = SKDDatabaseService.TimeTrackDocumentTranslator.Get(date, employeeUID);
			if (!operationResult.HasError)
			{
				dayTimeTrack.TimeTrackDocument = operationResult.Result;
			}

			return dayTimeTrack;
		}
	}
}