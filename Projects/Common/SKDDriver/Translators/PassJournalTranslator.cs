using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;
using OperationResult = FiresecAPI.OperationResult;
using System.Configuration;

namespace SKDDriver.Translators
{
	public class PassJournalTranslator : IDisposable
	{
		public static string ConnectionString { get; set; }
		DataAccess.PassJournalDataContext Context;

		public PassJournalTranslator()
		{
			Context = new DataAccess.PassJournalDataContext(ConnectionString);
		}

		public void Dispose()
		{
			Context.Dispose();
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

		public void InvalidatePassJournal()
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
			IEnumerable<ShortEmployee> employees = null;
			using (var skdDatabaseService = new SKDDatabaseService())
			{
				employees = skdDatabaseService.EmployeeTranslator.GetList(new EmployeeFilter()).Result;
			}
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

		public DayTimeTrack GetRealTimeTrack(DataAccess.Employee employee, DataAccess.Schedule schedule, DataAccess.ScheduleScheme scheduleScheme, IEnumerable<DataAccess.ScheduleZone> scheduleZones, DateTime date)
		{
			var dayTimeTrack = new DayTimeTrack();
			dayTimeTrack.Date = date;

			var passJournals = Context.PassJournals.Where(x => x.EmployeeUID == employee.UID && x.EnterTime != null && x.EnterTime.Date == date.Date).ToList();
			if (passJournals != null)
			{
				foreach (var passJournal in passJournals)
				{
					var scheduleZone = scheduleZones.FirstOrDefault(x => x.ZoneUID == passJournal.ZoneUID);
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
	}
}