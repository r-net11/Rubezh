using Common;
using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using SKDDriver.DataAccess;
using EmployeeDay = FiresecAPI.SKD.EmployeeDay;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class PassJournalTranslator : IDisposable
	{
		public static string ConnectionString { get; set; }

		public DataAccess.PassJournalDataContext Context { get; private set; }

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
					var tmpDateTime = DateTime.Now;
					exitPassJournal.ExitTime = tmpDateTime;
					exitPassJournal.ExitTimeOriginal = tmpDateTime;
					exitPassJournal.IsOpen = default(bool);
				}
				if (zoneUID != Guid.Empty)
				{
					var tmpDateTime = DateTime.Now;

					var enterPassJournal = new DataAccess.PassJournal //TODO:
					{
						UID = Guid.NewGuid(),
						EmployeeUID = employeeUID,
						ZoneUID = zoneUID,
						EnterTime = tmpDateTime,
						ExitTime = null,
						IsNeedAdjustment = exitPassJournal != null && exitPassJournal.ZoneUID == zoneUID, //TODO: check for right
						EnterTimeOriginal = tmpDateTime,
						IsOpen = true
					};
					Context.PassJournals.InsertOnSubmit(enterPassJournal);
				}
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult<Dictionary<DayTimeTrackPart, List<DayTimeTrackPart>>> FindConflictIntervals(List<DayTimeTrackPart> dayTimeTrackParts, Guid employeeGuid, DateTime currentDate)
		{
			var minIntervalsDate = dayTimeTrackParts.Where(x => x.EnterTimeOriginal.HasValue).DefaultIfEmpty().Min(x => x.EnterTimeOriginal.Value.Date); //if min return false
			var maxIntervalDate = dayTimeTrackParts.Where(x => x.ExitTimeOriginal.HasValue).DefaultIfEmpty().Max(x => x.ExitTimeOriginal.Value.Date);

			if(minIntervalsDate.Date == currentDate.Date && maxIntervalDate.Date == currentDate.Date)
				return new OperationResult<Dictionary<DayTimeTrackPart, List<DayTimeTrackPart>>>();

			List<PassJournal> linkedIntervals = Context.PassJournals.Where(x => x.EnterTimeOriginal.HasValue && x.ExitTimeOriginal.HasValue)
														.Where(
														x => x.EmployeeUID == employeeGuid &&
														x.EnterTimeOriginal.Value.Date >= minIntervalsDate.Date &&
														x.ExitTimeOriginal.Value.Date <= maxIntervalDate).ToList();
			var conflictedIntervals = new Dictionary<DayTimeTrackPart, List<DayTimeTrackPart>>();
			var tempCollection = new List<DayTimeTrackPart>();
			foreach (var el in dayTimeTrackParts)
			{
				var tmp = el;
				List<PassJournal> tmpCollection = linkedIntervals
					.Where(x => (x.UID != el.UID) && (tmp.EnterTimeOriginal.HasValue && tmp.ExitTimeOriginal.HasValue))
					.Where(x => (tmp.ExitTimeOriginal.Value.Date >= x.ExitTime.Value.Date && tmp.EnterTimeOriginal.Value.Date <= x.EnterTime.Date) &&
								(tmp.ExitTimeOriginal > x.EnterTime || tmp.EnterTimeOriginal < x.ExitTime))
					.Where(x => x.ExitTime >= tmp.EnterTimeOriginal)
					.ToList();
				List<PassJournal> tmpCollection2 = linkedIntervals
					.Where(x => (x.UID != el.UID) && (tmp.EnterTimeOriginal.HasValue && tmp.ExitTimeOriginal.HasValue))
					.Where(
						x => (tmp.EnterTimeOriginal.Value.Date >= x.EnterTime.Date && tmp.ExitTimeOriginal <= x.ExitTime.Value.Date) &&
						     (tmp.ExitTimeOriginal <= x.EnterTime || tmp.EnterTimeOriginal >= x.ExitTime))
					.Where(x => x.ExitTime <= tmp.EnterTimeOriginal)
					.ToList();

				tmpCollection = tmpCollection.Union(tmpCollection2).ToList();

				if (tmpCollection.Any())
				{
					foreach (var b in tmpCollection)
					{
						tempCollection.Add(new DayTimeTrackPart
						{
							UID = b.UID,
							EnterDateTime = b.EnterTime,
							EnterTime = b.EnterTime.TimeOfDay,
							EnterTimeOriginal = b.EnterTimeOriginal,
							ExitDateTime = b.ExitTime,
							ExitTime = b.ExitTime.GetValueOrDefault().TimeOfDay,
							ExitTimeOriginal = b.ExitTimeOriginal,
							TimeTrackZone = new TimeTrackZone
							{
								UID = b.ZoneUID
							}
						});
					}
					conflictedIntervals.Add(el, tempCollection);
				}
			}
			return new OperationResult<Dictionary<DayTimeTrackPart, List<DayTimeTrackPart>>>(conflictedIntervals);
		}

		public OperationResult EditPassJournal(DayTimeTrackPart dayTimeTrackPart, ShortEmployee employee, out bool? setAdjustmentFlag, out bool setBordersChangedFlag)
		{
			setAdjustmentFlag = null;
			setBordersChangedFlag = default(bool);

			try
			{
				if (dayTimeTrackPart.IsRemoveAllIntersections)
				{
					foreach (var intersectionElement in Context.PassJournals.Where(x => x.EmployeeUID == employee.UID)
																			.Where(x => dayTimeTrackPart.EnterDateTime <= x.ExitTime && dayTimeTrackPart.ExitDateTime >= x.EnterTime)
																			.Where(x => x.UID != dayTimeTrackPart.UID))

					{
						Context.PassJournals.DeleteOnSubmit(intersectionElement);
					}
					Context.SubmitChanges();
				}

				var passJournalItem = Context.PassJournals.FirstOrDefault(x => x.UID == dayTimeTrackPart.UID);

				if (passJournalItem != null)
				{
					if (passJournalItem.NotTakeInCalculations && !dayTimeTrackPart.NotTakeInCalculations)
						setAdjustmentFlag = false;
					else if (!passJournalItem.NotTakeInCalculations && dayTimeTrackPart.NotTakeInCalculations)
						setAdjustmentFlag = true;
					if (passJournalItem.EnterTime != dayTimeTrackPart.EnterDateTime ||
					    passJournalItem.ExitTime != dayTimeTrackPart.ExitDateTime)
						setBordersChangedFlag = true;

					passJournalItem.ZoneUID = dayTimeTrackPart.TimeTrackZone.UID;
					passJournalItem.EnterTime = dayTimeTrackPart.EnterDateTime.GetValueOrDefault();
					passJournalItem.ExitTime = dayTimeTrackPart.ExitDateTime.GetValueOrDefault();
					passJournalItem.IsNeedAdjustment = dayTimeTrackPart.IsNeedAdjustment;
					passJournalItem.AdjustmentDate = dayTimeTrackPart.AdjustmentDate;
					passJournalItem.CorrectedByUID = dayTimeTrackPart.CorrectedByUID;
					passJournalItem.NotTakeInCalculations = dayTimeTrackPart.NotTakeInCalculations;
					passJournalItem.IsAddedManually = dayTimeTrackPart.IsManuallyAdded;

					if (passJournalItem.IsOpen)
					{
						passJournalItem.ExitTimeOriginal = passJournalItem.ExitTime;
						passJournalItem.IsOpen = default(bool);
						passJournalItem.IsForceClosed = true;
					}
				}

				Context.SubmitChanges();

				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult AddCustomPassJournal(DayTimeTrackPart dayTimeTrackPart, ShortEmployee employee)
		{
			if(dayTimeTrackPart == null) return new OperationResult("ERROR");

			try
			{
				var passJournalItem = new PassJournal
				{
					UID = dayTimeTrackPart.UID,
					EmployeeUID = employee.UID,
					ZoneUID = dayTimeTrackPart.TimeTrackZone.UID,
					EnterTime = dayTimeTrackPart.EnterDateTime.GetValueOrDefault(),
					ExitTime = dayTimeTrackPart.ExitDateTime,
					AdjustmentDate = dayTimeTrackPart.AdjustmentDate,
					CorrectedByUID = dayTimeTrackPart.CorrectedByUID,
					NotTakeInCalculations = dayTimeTrackPart.NotTakeInCalculations,
					IsAddedManually = dayTimeTrackPart.IsManuallyAdded,
					EnterTimeOriginal = dayTimeTrackPart.EnterTimeOriginal,
					ExitTimeOriginal = dayTimeTrackPart.ExitTimeOriginal
				};

				Context.PassJournals.InsertOnSubmit(passJournalItem);
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
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

		public OperationResult DeleteAllPassJournalItems(DayTimeTrackPart dayTimeTrackPart)
		{
			try
			{
				var passJournalItem = Context.PassJournals.FirstOrDefault(x => x.UID == dayTimeTrackPart.UID);
				if (passJournalItem != null)
				{
					var items = Context.PassJournals.Where(x => x.EmployeeUID == passJournalItem.EmployeeUID && x.ZoneUID == passJournalItem.ZoneUID &&
						x.EnterTime >= dayTimeTrackPart.EnterDateTime && x.ExitTime != null && x.ExitTime.Value <= dayTimeTrackPart.ExitDateTime);
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
			//try
			//{
			//	var hasChanges = false;
			//	var emptyExitPassJournals = Context.PassJournals.Where(x => x.ExitTime == null);
			//	foreach (var emptyExitPassJournal in emptyExitPassJournals)
			//	{
			//		var enterTime = emptyExitPassJournal.EnterTime;
			//		var nowTime = DateTime.Now;
			//		if (nowTime.Date > enterTime.Date)
			//		{
			//			emptyExitPassJournal.EnterTime = new DateTime(enterTime.Year, enterTime.Month, enterTime.Day, 23, 59, 59);
			//			hasChanges = true;
			//		}
			//	}
			//	if (hasChanges)
			//	{
			//		Context.SubmitChanges();
			//	}
			//}
			//catch { }
		}

		public void InsertPassJournalTestData(string zoneUID)
		{
			IEnumerable<ShortEmployee> employees;
			using (var skdDatabaseService = new SKDDatabaseService())
			{
				employees = skdDatabaseService.EmployeeTranslator.GetList(new EmployeeFilter()).Result;
			}

			//Guid zoneUID;
			//using (var skdDatabaseService = new SKDDatabaseService())
			//{
			//	zoneUID = skdDatabaseService.ScheduleZoneTranslator.Result.FirstOrDefault().ZoneUID;
			//}


			//foreach (var passJournal in Context.PassJournals)
			//{
			//	Context.PassJournals.DeleteOnSubmit(passJournal);
			//}

			foreach (var employee in employees)
			{
				for (int day = 0; day < 30; day++)
				{
					var dateTime = DateTime.Now.AddDays(-day);

					//var seconds = new List<int>();
					var count = 2;
					//for (int i = 0; i < count * 2; i++)
					//{
					////	var totalSeconds = random.Next(0, 24 * 60 * 60);
					//	seconds.Add(7200);
					//}
					//seconds.Sort();
					int hours = 4;
					int pauseHours = 2;
					for (var i = 0; i < count * 2; i ++)
					{
						//var startTimeSpan = TimeSpan.FromSeconds(seconds[i]);
						//var endTimeSpan = TimeSpan.FromSeconds(seconds[i + 1]);

						var passJournal = new DataAccess.PassJournal
						{
							UID = Guid.NewGuid(),
							EmployeeUID = employee.UID,
				//			ZoneUID = zoneUID,
							ZoneUID = Guid.Parse(zoneUID),
							EnterTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hours, 5, 5),
							ExitTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, (hours + 2), 5, 5)
						};
						Context.PassJournals.InsertOnSubmit(passJournal);

						hours += pauseHours;
					}
				}
			}

			Context.SubmitChanges();
		}

		public DayTimeTrack GetRealTimeTrack(DataAccess.Employee employee, DataAccess.Schedule schedule, DataAccess.ScheduleScheme scheduleScheme, IEnumerable<DataAccess.ScheduleZone> scheduleZones, DateTime date)
		{
			return GetRealTimeTrack(employee, schedule, scheduleScheme, date, Context.PassJournals);
		}

		public DayTimeTrack GetRealTimeTrack(DataAccess.Employee employee, DataAccess.Schedule schedule, DataAccess.ScheduleScheme scheduleScheme, DateTime date, IEnumerable<DataAccess.PassJournal> passJournals)
		{
			var dayTimeTrack = new DayTimeTrack { Date = date };

			foreach (var passJournal in passJournals.Where(x => x.EmployeeUID == employee.UID && x.EnterTime.Date == date.Date).ToList())
			{
				var timeTrackPart = new TimeTrackPart
				{
					EnterDateTime = passJournal.EnterTime,
					ExitDateTime =  passJournal.ExitTime,
					ZoneUID = passJournal.ZoneUID,
					PassJournalUID = passJournal.UID,
					IsManuallyAdded = passJournal.IsAddedManually,
					NotTakeInCalculations = passJournal.NotTakeInCalculations,
					IsNeedAdjustment = passJournal.IsNeedAdjustment,
					AdjustmentDate = passJournal.AdjustmentDate,
					CorrectedByUID = passJournal.CorrectedByUID,
					EnterTimeOriginal = passJournal.EnterTimeOriginal,
					ExitTimeOriginal = passJournal.ExitTimeOriginal,
					IsOpen = passJournal.IsOpen,
					IsForceClosed = passJournal.IsForceClosed
				};

				dayTimeTrack.RealTimeTrackParts.Add(timeTrackPart);
			}

			dayTimeTrack.RealTimeTrackParts = dayTimeTrack.RealTimeTrackParts.OrderBy(x => x.EnterDateTime.Ticks).ToList();

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
				return lastPassJournal;
			}
			catch
			{
				return null;
			}
		}

		public IEnumerable<DataAccess.PassJournal> GetEmployeesLastEnterPassJournal(IEnumerable<Guid> employeeUIDs, IEnumerable<Guid> zoneUIDs, DateTime? dateTime)
		{
			try
			{
				var filter = PredicateBuilder.True<DataAccess.PassJournal>();

				var isManyEmployees = GetIsManyEmployees(employeeUIDs);

				if (!employeeUIDs.IsEmpty() && !isManyEmployees)
					filter = filter.And(e => employeeUIDs.Contains(e.EmployeeUID));
				if (!zoneUIDs.IsEmpty())
					filter = filter.And(e => zoneUIDs.Contains(e.ZoneUID));
				if (dateTime.HasValue)
				{
					filter =
						filter.And(
							e =>
								e.EnterTime.Day == dateTime.Value.Day && e.EnterTime.Hour >= dateTime.Value.Hour &&
								e.EnterTime.Minute >= dateTime.Value.Minute);
					//		filter =
					//			filter.And(
					//				e =>
					//					e.EnterTime.Day == dateTime.Value.Day && e.EnterTime.Hour >= dateTime.Value.Hour &&
					//					e.EnterTime.Minute >= dateTime.Value.Minute && !e.ExitTime.HasValue);
					// !e.ExitTime.HasValue || e.ExitTime > dateTime);
				}
				else
					filter = filter.And(e => !e.ExitTime.HasValue);

				var result = Context.PassJournals.Where(filter).ToList();

				if (isManyEmployees)
					result = result.Where(x => employeeUIDs.Contains(x.EmployeeUID)).ToList();

				return result.GroupBy(item => item.EmployeeUID).Select(gr => gr.OrderByDescending(item => item.EnterTime).First());
			}
			catch
			{
				return null;
			}
		}

		public IEnumerable<DataAccess.PassJournal> GetEmployeesLastExitPassJournal(IEnumerable<Guid> employeeUIDs, DateTime? dateTime)
		{
			try
			{
				var filter = PredicateBuilder.True<DataAccess.PassJournal>();

				var isManyEmployees = GetIsManyEmployees(employeeUIDs);

				if (!employeeUIDs.IsEmpty() && !isManyEmployees)
					filter = filter.And(e => employeeUIDs.Contains(e.EmployeeUID));
				filter = filter.And(e => e.ExitTime.HasValue);
				if (dateTime.HasValue)
					filter = filter.And(e => e.ExitTime < dateTime);

				var result = Context.PassJournals.Where(filter).ToList();

				if (isManyEmployees)
					result = result.Where(x => employeeUIDs.Contains(x.EmployeeUID)).ToList();

				return result.GroupBy(item => item.EmployeeUID).Select(gr => gr.OrderByDescending(item => item.ExitTime).First());
			}
			catch
			{
				return null;
			}
		}

		public IEnumerable<DataAccess.PassJournal> GetEmployeesRoot(IEnumerable<Guid> employeeUIDs, IEnumerable<Guid> zoneUIDs, DateTime startDateTime, DateTime endDateTime)
		{
			try
			{
				var filter = PredicateBuilder.True<DataAccess.PassJournal>();

				var isManyEmployees = GetIsManyEmployees(employeeUIDs);

				if (!employeeUIDs.IsEmpty() && !isManyEmployees)
					filter = filter.And(e => employeeUIDs.Contains(e.EmployeeUID));
				if (!zoneUIDs.IsEmpty())
					filter = filter.And(e => zoneUIDs.Contains(e.ZoneUID));
				filter = filter.And(e =>
					(e.EnterTime >= startDateTime && e.EnterTime <= endDateTime)
					|| (e.ExitTime >= startDateTime && e.ExitTime <= endDateTime || !e.ExitTime.HasValue));
				var result = Context.PassJournals.Where(filter).ToList();

				if (isManyEmployees)
					result = result.Where(x => employeeUIDs.Contains(x.EmployeeUID)).ToList();

				return result;
			}
			catch
			{
				return null;
			}
		}

		private bool GetIsManyEmployees(IEnumerable<Guid> employeeUIDs)
		{
			return employeeUIDs.Count() >= 2100;
		}

		public OperationResult<DateTime> GetMinDate()
		{
			try
			{
				if (Context.PassJournals.IsEmpty())
					return new OperationResult<DateTime>(new DateTime());
				var result = Context.PassJournals.Min(x => x.EnterTime);
				return new OperationResult<DateTime>(result);
			}
			catch (Exception e)
			{
				return OperationResult<DateTime>.FromError(e.Message);
			}
		}

	}
}