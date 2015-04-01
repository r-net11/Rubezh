using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class PassJournalTranslator : IDisposable
	{
		public static string ConnectionString { get; set; }
		DataAccess.PassJournalDataContext Context;

		public PassJournalTranslator()
		{
			Context = new DataAccess.PassJournalDataContext(ConnectionString);
			Synchroniser = new PassJounalSynchroniser(Context.PassJournals);
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

		public OperationResult AddCustomPassJournal(Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			try
			{
				var passJournalItem = new DataAccess.PassJournal();
				passJournalItem.UID = uid;
				passJournalItem.EmployeeUID = employeeUID;
				passJournalItem.ZoneUID = zoneUID;
				passJournalItem.EnterTime = enterTime;
				passJournalItem.ExitTime = exitTime;
				if (IsIntersection(passJournalItem))
				{
					return new OperationResult("Невозможно добавить пересекающийся интервал");
				}
				Context.PassJournals.InsertOnSubmit(passJournalItem);
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult EditPassJournal(Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			try
			{
				var passJournalItem = Context.PassJournals.FirstOrDefault(x => x.UID == uid);
				if (passJournalItem != null)
				{
					passJournalItem.ZoneUID = zoneUID;
					passJournalItem.EnterTime = enterTime;
					passJournalItem.ExitTime = exitTime;
				}
				if (IsIntersection(passJournalItem))
				{
					return new OperationResult("Невозможно добавить пересекающийся интервал");
				}
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		bool IsIntersection(DataAccess.PassJournal passJournalItem)
		{
			return Context.PassJournals.Any(x => x.UID != passJournalItem.UID &&
						(x.EnterTime <= passJournalItem.EnterTime && x.ExitTime >= passJournalItem.EnterTime ||
							x.EnterTime <= passJournalItem.ExitTime && x.ExitTime >= passJournalItem.ExitTime));
		}

		public OperationResult DeletePassJournal(Guid uid)
		{
			try
			{
				var passJournalItem = Context.PassJournals.FirstOrDefault(x => x.UID == uid);
				if (passJournalItem != null)
				{
					Context.PassJournals.DeleteOnSubmit(passJournalItem);
				}
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult DeleteAllPassJournalItems(Guid uid, DateTime enterTime, DateTime exitTime)
		{
			try
			{
				var passJournalItem = Context.PassJournals.FirstOrDefault(x => x.UID == uid);
				if (passJournalItem != null)
				{
					var items = Context.PassJournals.Where(x => x.EmployeeUID == passJournalItem.EmployeeUID && x.ZoneUID == passJournalItem.ZoneUID &&
						x.EnterTime >= enterTime && x.ExitTime != null && x.ExitTime.Value <= exitTime);
					Context.PassJournals.DeleteAllOnSubmit(items);
				}
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
								ZoneUID = passJournal.ZoneUID,
								PassJournalUID = passJournal.UID
							};
							dayTimeTrack.RealTimeTrackParts.Add(timeTrackPart);
						}
					}
				}
			}
			dayTimeTrack.RealTimeTrackParts = dayTimeTrack.RealTimeTrackParts.OrderBy(x => x.StartTime.Ticks).ToList();

			return dayTimeTrack;
		}

		public OperationResult SaveEmployeeDays(List<EmployeeDay> employeeDays)
		{
			try
			{
				foreach (var employeeDay in employeeDays)
				{
					var tableEmployeeDay = new DataAccess.EmployeeDay
					{
						UID = employeeDay.UID,
						AllowedEarlyLeave = employeeDay.AllowedEarlyLeave,
						AllowedLate = employeeDay.AllowedLate,
						Date = employeeDay.Date,
						DayIntervalsString = employeeDay.DayIntervalsString,
						EmployeeUID = employeeDay.EmployeeUID,
						IsIgnoreHoliday = employeeDay.IsIgnoreHoliday,
						IsOnlyFirstEnter = employeeDay.IsOnlyFirstEnter
					};
					Context.EmployeeDays.InsertOnSubmit(tableEmployeeDay);
				}
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public PassJounalSynchroniser Synchroniser;

		public DataAccess.PassJournal GetEmployeeLastPassJournal(Guid employeeUID)
		{
			try
			{
				var lastPassJournal = Context.PassJournals.FirstOrDefault(x => x.EmployeeUID == employeeUID && x.ExitTime == null);
				if (lastPassJournal != null)
				{
					return lastPassJournal;
				}
				else
				{
					lastPassJournal = Context.PassJournals.Where(x => x.EmployeeUID == employeeUID).OrderByDescending(x => x.ExitTime).FirstOrDefault();
					if (lastPassJournal != null)
					{
						return lastPassJournal;
					}
				}
				return null;
			}
			catch (Exception e)
			{
				return null;
			}
		}

		public IEnumerable<DataAccess.PassJournal> GetEmployeeRoot(Guid employeeUID, List<Guid> zoneUIDs, DateTime startDateTime, DateTime endDateTime)
		{
			try
			{
				var lastPassJournal = Context.PassJournals.Where(x => x.EmployeeUID == employeeUID && zoneUIDs.Contains(x.ZoneUID) && x.EnterTime >= startDateTime && x.ExitTime <= endDateTime);
				if (lastPassJournal != null)
				{
					return lastPassJournal;
				}
				return null;
			}
			catch (Exception e)
			{
				return null;
			}
		}

		public IEnumerable<DataAccess.PassJournal> GetEmployeesLastEnterPassJournal(IEnumerable<Guid> employeeUIDs, IEnumerable<Guid> zoneUIDs, DateTime? dateTime)
		{
			try
			{
				var filter = PredicateBuilder.True<DataAccess.PassJournal>();
				if (!employeeUIDs.IsEmpty())
					filter = filter.And(e => employeeUIDs.Contains(e.EmployeeUID));
				if (!zoneUIDs.IsEmpty())
					filter = filter.And(e => zoneUIDs.Contains(e.ZoneUID));
				if (dateTime.HasValue)
					filter = filter.And(e => e.EnterTime < dateTime && (!e.ExitTime.HasValue || e.ExitTime > dateTime));
				else
					filter = filter.And(e => !e.ExitTime.HasValue);
				return Context.PassJournals.Where(filter).GroupBy(item => item.EmployeeUID).Select(gr => gr.OrderByDescending(item => item.EnterTime).First());
			}
			catch(Exception e)
			{
				return null;
			}
		}
		public IEnumerable<DataAccess.PassJournal> GetEmployeesLastExitPassJournal(IEnumerable<Guid> employeeUIDs, DateTime? dateTime)
		{
			try
			{
				var filter = PredicateBuilder.True<DataAccess.PassJournal>();
				if (!employeeUIDs.IsEmpty())
					filter = filter.And(e => employeeUIDs.Contains(e.EmployeeUID));
				filter = filter.And(e => e.ExitTime.HasValue);
				if (dateTime.HasValue)
					filter = filter.And(e => e.ExitTime < dateTime);
				return Context.PassJournals.Where(filter).GroupBy(item => item.EmployeeUID).Select(gr => gr.OrderByDescending(item => item.ExitTime).First());
			}
			catch(Exception e)
			{
				return null;
			}

		}
		public IEnumerable<DataAccess.PassJournal> GetEmployeesRoot(IEnumerable<Guid> employeeUIDs, IEnumerable<Guid> zoneUIDs, DateTime startDateTime, DateTime endDateTime)
		{
			try
			{
				var filter = PredicateBuilder.True<DataAccess.PassJournal>();
				if (!employeeUIDs.IsEmpty())
					filter = filter.And(e => employeeUIDs.Contains(e.EmployeeUID));
				if (!zoneUIDs.IsEmpty())
					filter = filter.And(e => zoneUIDs.Contains(e.ZoneUID));
				filter = filter.And(e => (e.EnterTime >= startDateTime && e.EnterTime <= endDateTime) || (e.ExitTime >= startDateTime && e.ExitTime <= endDateTime));
				return Context.PassJournals.Where(filter);
			}
			catch (Exception e)
			{
				return null;
			}
		}

		public OperationResult<DateTime> GetMinDate()
		{
			try
			{
				if(Context.PassJournals.IsEmpty())
					return new OperationResult<DateTime> { Result = new DateTime() };
				var result = Context.PassJournals.Min(x => x.EnterTime);
				return new OperationResult<DateTime> { Result = result };
			}
			catch(Exception e)
			{
				return new OperationResult<DateTime>(e.Message);
			}
		}
	}
}