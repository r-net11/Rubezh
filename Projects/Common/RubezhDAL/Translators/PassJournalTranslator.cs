using Common;
using RubezhAPI;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace RubezhDAL.DataClasses
{
	public class PassJournalTranslator
	{
		DbService DbService;
		DatabaseContext Context;
		public PassJounalSynchroniser Synchroniser { get; private set; }

		public PassJournalTranslator(DbService dbService)
		{
			DbService = dbService;
			Context = DbService.Context;
			Synchroniser = new PassJounalSynchroniser(dbService);
		}

		public OperationResult<bool> AddPassJournal(Guid employeeUID, Guid zoneUID)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var exitPassJournal = Context.PassJournals.FirstOrDefault(x => x.EmployeeUID == employeeUID && x.ExitTime == null);
				if (exitPassJournal != null)
				{
					exitPassJournal.ExitTime = DateTime.Now;
				}
				if (zoneUID != Guid.Empty)
				{
					var enterPassJournal = new PassJournal();
					enterPassJournal.UID = Guid.NewGuid();
					enterPassJournal.EmployeeUID = employeeUID;
					enterPassJournal.ZoneUID = zoneUID;
					enterPassJournal.EnterTime = DateTime.Now;
					enterPassJournal.ExitTime = null;
					Context.PassJournals.Add(enterPassJournal);
				}
				Context.SaveChanges();
				return true;
			});
		}

		public OperationResult<bool> AddCustomPassJournal(Guid uid, Guid employeeUID, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var passJournalItem = new PassJournal();
				passJournalItem.UID = uid;
				passJournalItem.EmployeeUID = employeeUID.EmptyToNull();
				passJournalItem.ZoneUID = zoneUID;
				passJournalItem.EnterTime = enterTime.CheckDate();
				passJournalItem.ExitTime = exitTime.CheckDate();
				if (IsIntersection(passJournalItem))
					throw new Exception("Невозможно добавить пересекающийся интервал");
				Context.PassJournals.Add(passJournalItem);
				Context.SaveChanges();
				return true;
			});
		}

		public OperationResult<bool> EditPassJournal(Guid uid, Guid zoneUID, DateTime enterTime, DateTime exitTime)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var passJournalItem = Context.PassJournals.FirstOrDefault(x => x.UID == uid);
				if (passJournalItem != null)
				{
					passJournalItem.ZoneUID = zoneUID;
					passJournalItem.EnterTime = enterTime.CheckDate();
					passJournalItem.ExitTime = exitTime.CheckDate();
				}
				if (IsIntersection(passJournalItem))
					throw new Exception("Невозможно добавить пересекающийся интервал");
				Context.SaveChanges();
				return true;
			});
		}

		public OperationResult<bool> DeletePassJournal(Guid uid)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var passJournalItem = Context.PassJournals.FirstOrDefault(x => x.UID == uid);
				if (passJournalItem != null)
				{
					Context.PassJournals.Remove(passJournalItem);
					Context.SaveChanges();
				}
				return true;
			});
		}

		public OperationResult<bool> DeleteAllPassJournalItems(Guid uid, DateTime enterTime, DateTime exitTime)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var passJournalItem = Context.PassJournals.FirstOrDefault(x => x.UID == uid);
				if (passJournalItem != null)
				{
					var items = Context.PassJournals.Where(x =>
						x.EmployeeUID == passJournalItem.EmployeeUID &&
						x.ZoneUID == passJournalItem.ZoneUID &&
						x.EnterTime >= enterTime &&
						x.ExitTime != null &&
						x.ExitTime.Value <= exitTime);
					Context.PassJournals.RemoveRange(items);
					Context.SaveChanges();
				}
				return true;
			});
		}

		public void InsertPassJournalTestData()
		{
			IEnumerable<ShortEmployee> employees = null;
			employees = DbService.EmployeeTranslator.ShortTranslator.Get(new EmployeeFilter()).Result;
			var zoneUID = GKManager.Zones.FirstOrDefault().UID;

			Context.PassJournals.RemoveRange(Context.PassJournals);

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

						var passJournal = new PassJournal();
						passJournal.UID = Guid.NewGuid();
						passJournal.EmployeeUID = employee.UID;
						passJournal.ZoneUID = zoneUID;
						passJournal.EnterTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, startTimeSpan.Hours, startTimeSpan.Minutes, startTimeSpan.Seconds);
						passJournal.ExitTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, endTimeSpan.Hours, endTimeSpan.Minutes, endTimeSpan.Seconds);
						Context.PassJournals.Add(passJournal);
					}
				}
			}
			Context.SaveChanges();
		}

		public DayTimeTrack GetRealTimeTrack(Guid employeeUID, IEnumerable<ScheduleZone> scheduleZones, DateTime date)
		{
			return GetRealTimeTrack(employeeUID, scheduleZones, date, Context.PassJournals);
		}

		public DayTimeTrack GetRealTimeTrack(Guid employeeUID, IEnumerable<ScheduleZone> scheduleZones, DateTime date, IEnumerable<PassJournal> passJournals)
		{
			var dayTimeTrack = new DayTimeTrack();
			dayTimeTrack.Date = date;

			foreach (var passJournal in passJournals.Where(x => x.EmployeeUID == employeeUID
				&& x.EnterTime != null && x.EnterTime.Date <= date.Date
				&& x.ExitTime != null && x.ExitTime.Value.Date >= date.Date
				&& scheduleZones.Select(scheduleZone => scheduleZone.ZoneUID).Contains(x.ZoneUID)).ToList())
			{
				var startTime = new TimeSpan();
				var endTime = new TimeSpan();
				var startsInPreviousDay = false;
				var endsInNextDay = false;
				var enterTime = passJournal.EnterTime;
				var exitTime = passJournal.ExitTime.Value;
				if (enterTime.Date == date && exitTime.Date >= date)
				{
					startTime = enterTime.TimeOfDay;
					if (exitTime.Date == date)
						endTime = exitTime.TimeOfDay;
					else if (exitTime.Date > date)
						endTime = TimeSpan.FromDays(1);
					endsInNextDay = exitTime.Date > date;
				}
				else if (enterTime.Date < date && exitTime.Date == date)
				{
					startTime = TimeSpan.Zero; ;
					endTime = exitTime.TimeOfDay;
					startsInPreviousDay = true;
				}
				else if (enterTime.Date < date && exitTime.Date > date)
				{
					startTime = TimeSpan.Zero; ;
					endTime = TimeSpan.FromDays(1);
					startsInPreviousDay = true;
					endsInNextDay = true;
				}

				if (passJournal.ExitTime.HasValue)
				{
					var timeTrackPart = new TimeTrackPart()
					{
						StartTime = startTime,
						EndTime = endTime,
						ZoneUID = passJournal.ZoneUID,
						PassJournalUID = passJournal.UID,
						StartsInPreviousDay = startsInPreviousDay,
						EndsInNextDay = endsInNextDay
					};
					dayTimeTrack.RealTimeTrackParts.Add(timeTrackPart);
				}
			}
			dayTimeTrack.RealTimeTrackParts = dayTimeTrack.RealTimeTrackParts.OrderBy(x => x.StartTime.Ticks).ToList();
			return dayTimeTrack;
		}

		public OperationResult<bool> SaveEmployeeDays(List<RubezhAPI.SKD.EmployeeDay> employeeDays)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				foreach (var employeeDay in employeeDays)
				{
					var tableEmployeeDay = new EmployeeDay
					{
						UID = employeeDay.UID,
						AllowedEarlyLeaveTimeSpan = TimeSpan.FromSeconds(employeeDay.AllowedEarlyLeave),
						AllowedLateTimeSpan = TimeSpan.FromSeconds(employeeDay.AllowedLate),
						Date = employeeDay.Date,
						DayIntervalsString = employeeDay.DayIntervalsString,
						EmployeeUID = employeeDay.EmployeeUID,
						IsIgnoreHoliday = employeeDay.IsIgnoreHoliday,
						IsOnlyFirstEnter = employeeDay.IsOnlyFirstEnter
					};
					Context.EmployeeDays.Add(tableEmployeeDay);
				}
				Context.SaveChanges();
				return true;
			});
		}

		public PassJournal GetEmployeeLastPassJournal(Guid employeeUID)
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
					lastPassJournal = Context.PassJournals.Where(x => x.EmployeeUID == employeeUID)
						.OrderByDescending(x => x.ExitTime).FirstOrDefault();
					if (lastPassJournal != null)
					{
						return lastPassJournal;
					}
				}
				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public IEnumerable<PassJournal> GetEmployeeRoot(Guid employeeUID, List<Guid> zoneUIDs, DateTime startDateTime, DateTime endDateTime)
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
			catch (Exception)
			{
				return null;
			}
		}

		public IEnumerable<PassJournal> GetEmployeesLastEnterPassJournal(IEnumerable<Guid> employeeUIDs, IEnumerable<Guid> zoneUIDs, DateTime? dateTime)
		{
			try
			{
				var isManyEmployees = employeeUIDs.Count() >= 2100;
				IQueryable<PassJournal> items = Context.PassJournals;
				items = items.Where(e => e.EmployeeUID != null && employeeUIDs.Contains(e.EmployeeUID.Value));
				if (zoneUIDs != null && zoneUIDs.Count() > 0 && employeeUIDs.Count() > 0)
					items = items.Where(x => zoneUIDs.Contains(x.ZoneUID));
				if (dateTime.HasValue)
					items = items.Where(e => e.EnterTime < dateTime && (!e.ExitTime.HasValue || e.ExitTime > dateTime));
				else
					items.Where(e => !e.ExitTime.HasValue);
				//if (isManyEmployees)
				//	items = items.Where(e => e.EmployeeUID.HasValue && employeeUIDs.Contains(e.EmployeeUID.Value)).ToList();
				return items.GroupBy(item => item.EmployeeUID).Select(gr => gr.OrderByDescending(item => item.EnterTime).FirstOrDefault()).ToList();
			}
			catch (Exception)
			{
				return null;
			}
		}
		public IEnumerable<PassJournal> GetEmployeesLastExitPassJournal(IEnumerable<Guid> employeeUIDs, DateTime? dateTime)
		{
			try
			{
				var isManyEmployees = employeeUIDs.Count() >= 2100;
				IQueryable<PassJournal> items = Context.PassJournals;
				if (employeeUIDs.Count() == 0 || isManyEmployees)
					items = items.Where(e => e.EmployeeUID != null && employeeUIDs.Contains(e.EmployeeUID.Value));
				if (dateTime.HasValue)
					items = items.Where(x => x.ExitTime.HasValue && x.ExitTime < dateTime);
				//if (isManyEmployees)
				//	items = items.Where(e => e.EmployeeUID.HasValue && employeeUIDs.Contains(e.EmployeeUID.Value)).ToList();
				return items.GroupBy(item => item.EmployeeUID).Select(gr => gr.OrderByDescending(item => item.ExitTime).First());
			}
			catch (Exception)
			{
				return null;
			}

		}
		public IEnumerable<PassJournal> GetEmployeesRoot(IEnumerable<Guid> employeeUIDs, IEnumerable<Guid> zoneUIDs, DateTime startDateTime, DateTime endDateTime)
		{
			try
			{
				var isManyEmployees = employeeUIDs.Count() >= 2100;
				IQueryable<PassJournal> items = Context.PassJournals;
				if (employeeUIDs.Count() == 0 || isManyEmployees)
					items = items.Where(e => e.EmployeeUID != null && employeeUIDs.Contains(e.EmployeeUID.Value));
				if (zoneUIDs != null && zoneUIDs.Count() > 0)
					items = items.Where(x => zoneUIDs.Contains(x.ZoneUID));
				items = items.Where(e => (e.EnterTime >= startDateTime && e.EnterTime <= endDateTime) || (e.ExitTime >= startDateTime && e.ExitTime <= endDateTime));
				//if (isManyEmployees)
				//	items = items.Where(e => e.EmployeeUID != null && employeeUIDs.Contains(e.EmployeeUID.Value));
				return items.ToList();
			}
			catch (Exception)
			{
				return null;
			}
		}

		public OperationResult<DateTime> GetMinDate()
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				if (Context.PassJournals.IsEmpty())
					return new DateTime();
				return Context.PassJournals.Min(x => x.EnterTime);
			});
		}

		public List<PassJournal> GetAllPassJournals()
		{
			return Context.PassJournals.ToList();
		}

		bool IsIntersection(PassJournal passJournalItem)
		{
			return Context.PassJournals.Any(x => x.UID != passJournalItem.UID &&
				x.EmployeeUID == passJournalItem.EmployeeUID &&
				(x.EnterTime < passJournalItem.EnterTime && x.ExitTime > passJournalItem.EnterTime ||
					x.EnterTime < passJournalItem.ExitTime && x.ExitTime > passJournalItem.ExitTime));
		}
	}

	public class PassJounalSynchroniser
	{
		DatabaseContext _context;
		string Name { get { return "PassJournal"; } }
		public string NameXml { get { return Name + ".xml"; } }

		public PassJounalSynchroniser(DbService dbService)
		{
			_context = dbService.Context;
		}

		public OperationResult<bool> Export(JournalExportFilter filter)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				if (!Directory.Exists(filter.Path))
					throw new Exception("Папка не существует");
				var minDate = filter.MinDate.CheckDate();
				var maxDate = filter.MaxDate.CheckDate();
				var tableItems = _context.PassJournals.Where(x => x.EnterTime >= minDate && x.EnterTime <= maxDate).ToList();
				var employeeUIDs = tableItems.Select(x => x.EmployeeUID).ToList();
				var employees = _context.Employees.Where(x => employeeUIDs.Any(y => y == x.UID)).ToList();
				var items = tableItems.Select(x => Translate(x, employees)).ToList();
				var serializer = new XmlSerializer(typeof(List<ExportPassJournalItem>));
				using (var fileStream = File.Open(NameXml, FileMode.Create))
				{
					serializer.Serialize(fileStream, items);
				}
				if (filter.Path != null)
				{
					var newPath = Path.Combine(filter.Path, NameXml);
					if (File.Exists(newPath))
						File.Delete(newPath);
					File.Move(NameXml, newPath);
				}
				return true;
			});
		}

		ExportPassJournalItem Translate(PassJournal tableItem, List<Employee> employees)
		{
			var employee = employees.FirstOrDefault(x => x.UID == tableItem.EmployeeUID);
			var zone = GKManager.SKDZones.FirstOrDefault(x => x.UID == tableItem.ZoneUID);
			return new ExportPassJournalItem
			{
				UID = tableItem.UID,
				EmployeeUID = tableItem.EmployeeUID != null ? tableItem.EmployeeUID.Value : Guid.Empty,
				EmployeeFIO = employee != null ? employee.LastName + " " + employee.FirstName + " " + employee.SecondName : "",
				EnterDateTime = tableItem.EnterTime,
				ExitDateTime = tableItem.ExitTime != null ? tableItem.ExitTime.Value : new DateTime(),
				ZoneUID = tableItem.ZoneUID,
				ZoneNo = zone != null ? zone.No : -1
			};
		}
	}

	public class ExportPassJournalItem
	{
		public Guid UID { get; set; }
		public Guid EmployeeUID { get; set; }
		public string EmployeeFIO { get; set; }
		public Guid ZoneUID { get; set; }
		public int ZoneNo { get; set; }
		public DateTime EnterDateTime { get; set; }
		public DateTime ExitDateTime { get; set; }
	}
}