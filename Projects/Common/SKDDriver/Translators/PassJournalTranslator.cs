using System.Data.SqlTypes;
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
					exitPassJournal.IsNeedAdjustment = exitPassJournal.ZoneUID == zoneUID;
					exitPassJournal.IsNeedAdjustmentOriginal = exitPassJournal.IsNeedAdjustment;
					exitPassJournal.IsOpen = default(bool);
				}
				if (zoneUID != Guid.Empty)
				{
					var tmpDateTime = DateTime.Now;

					var enterPassJournal = new PassJournal //TODO:
					{
						UID = Guid.NewGuid(),
						EmployeeUID = employeeUID,
						ZoneUID = zoneUID,
						EnterTime = tmpDateTime,
						ExitTime = null,
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

		public OperationResult<bool> CheckForCanForseCloseInterval(Guid openedIntervalGuid)
		{
			var currentInterval = Context.PassJournals.FirstOrDefault(x => x.UID == openedIntervalGuid);

			if (currentInterval != null && currentInterval.IsOpen) return new OperationResult<bool>(true);

			return new OperationResult<bool>(false);
		}

		public OperationResult<List<DayTimeTrackPart>> GetMissedIntervals(DateTime currentDate, ShortEmployee currentEmployee)
		{
			var result = Context.PassJournals
				.Where(x => x.EmployeeUID == currentEmployee.UID)
				.ToList()
				.Where(x => x.EnterTime.Date != currentDate.Date && x.ExitTime.GetValueOrDefault().Date != currentDate.Date)
				.Where(
					x =>
						(x.EnterTimeOriginal.HasValue && x.EnterTimeOriginal.Value.Date == currentDate.Date) ||
						(x.ExitTimeOriginal.HasValue && x.ExitTimeOriginal.Value.Date == currentDate.Date));

			var resultCollection = new List<DayTimeTrackPart>();
			foreach (var passjournalItem in result)
			{
				var timeTrackZone = SKDManager.Zones.FirstOrDefault(x => x.UID == passjournalItem.ZoneUID);
				resultCollection.Add(new DayTimeTrackPart
				{
					AdjustmentDate = passjournalItem.AdjustmentDate,
					CorrectedByUID = passjournalItem.CorrectedByUID,
					EnterDateTime = passjournalItem.EnterTime,
					EnterTime = passjournalItem.EnterTime.TimeOfDay,
					EnterTimeOriginal = passjournalItem.EnterTimeOriginal,
					ExitDateTime = passjournalItem.ExitTime,
					ExitTime = passjournalItem.ExitTime.GetValueOrDefault().TimeOfDay,
					ExitTimeOriginal = passjournalItem.ExitTimeOriginal,
					IsForceClosed = passjournalItem.IsForceClosed,
					IsManuallyAdded = passjournalItem.IsAddedManually,
					IsNeedAdjustment = passjournalItem.IsNeedAdjustment,
					IsNeedAdjustmentOriginal = passjournalItem.IsNeedAdjustmentOriginal,
					IsOpen = passjournalItem.IsOpen,
					NotTakeInCalculations = passjournalItem.NotTakeInCalculations,
					UID = passjournalItem.UID,
					TimeTrackZone = new TimeTrackZone
					{
						UID = timeTrackZone != null ? timeTrackZone.UID : Guid.Empty,
						Description = timeTrackZone != null ? timeTrackZone.Description : string.Empty,
						Name = timeTrackZone != null ? timeTrackZone.Name : string.Empty,
						SKDZone = timeTrackZone,
						No = timeTrackZone != null ? timeTrackZone.No : default(int)
					}

				});
			}

			return new OperationResult<List<DayTimeTrackPart>>(resultCollection);
		}

		private List<DayTimeTrackPart> TrySetOriginalValuesToTimeTracks(List<DayTimeTrackPart> dayTimeTimeTrackParts)
		{
			var resultCollection = new List<DayTimeTrackPart>();
			foreach (var dayTimeTrack in dayTimeTimeTrackParts)
			{
				dayTimeTrack.EnterDateTime = dayTimeTrack.EnterTimeOriginal;
				dayTimeTrack.ExitDateTime = dayTimeTrack.ExitTimeOriginal;
				resultCollection.Add(dayTimeTrack);
			}
			return resultCollection;
		}

		private Dictionary<DayTimeTrackPart, List<DayTimeTrackPart>> GetIntervalsWithConflicts(
			List<DayTimeTrackPart> originalCollection, List<PassJournal> linkedIntervalsCollection)
		{
			var resultDictionary = new Dictionary<DayTimeTrackPart, List<DayTimeTrackPart>>();
			var tmpCollection = new List<DayTimeTrackPart>();
			foreach (var el in originalCollection)
			{
				var conflictedCollection = linkedIntervalsCollection
					.Where(x => (x.UID != el.UID) && (el.EnterTimeOriginal.HasValue && el.ExitTimeOriginal.HasValue))
					.Where(x => el.ExitTimeOriginal >= x.ExitTime && el.EnterTimeOriginal <= x.EnterTime).ToList();

				if (conflictedCollection.Any())
				{
					tmpCollection.AddRange(conflictedCollection.Select(item => new DayTimeTrackPart
					{
						UID = item.UID,
						EnterDateTime = item.EnterTime,
						EnterTime = item.EnterTime.TimeOfDay,
						EnterTimeOriginal = item.EnterTimeOriginal,
						ExitDateTime = item.ExitTime,
						IsManuallyAdded = item.IsAddedManually,
						ExitTime = item.ExitTime.GetValueOrDefault().TimeOfDay,
						ExitTimeOriginal = item.ExitTimeOriginal,
						IsNeedAdjustmentOriginal = item.IsNeedAdjustmentOriginal,
						TimeTrackZone = new TimeTrackZone
						{
							UID = item.ZoneUID, Name = SKDManager.Zones.FirstOrDefault(x => x.UID == item.ZoneUID) != null ? SKDManager.Zones.FirstOrDefault(x => x.UID == item.ZoneUID).Name : string.Empty
						}
					}));
					resultDictionary.Add(el, tmpCollection);
				}
			}

			return resultDictionary;
		}

		public OperationResult<Dictionary<DayTimeTrackPart, List<DayTimeTrackPart>>> FindConflictIntervals(List<DayTimeTrackPart> dayTimeTrackParts, Guid employeeGuid, DateTime currentDate)
		{
			var minIntervalsDate = dayTimeTrackParts.Where(x => x.EnterTimeOriginal.HasValue).DefaultIfEmpty().Min(x => x.EnterTimeOriginal != null ? x.EnterTimeOriginal.Value.Date : new DateTime()); //if min return false
			var maxIntervalDate = dayTimeTrackParts.Where(x => x.ExitTimeOriginal.HasValue).DefaultIfEmpty().Max(x => x.ExitTimeOriginal != null ? x.ExitTimeOriginal.Value.Date : new DateTime());

			if(minIntervalsDate.Date == currentDate.Date && maxIntervalDate.Date == currentDate.Date)
				return new OperationResult<Dictionary<DayTimeTrackPart, List<DayTimeTrackPart>>>();

			//var originalIntervalsCollection = TrySetOriginalValuesToTimeTracks(new List<DayTimeTrackPart>(dayTimeTrackParts));

			List<PassJournal> linkedIntervals = Context.PassJournals.Where(x => x.EnterTimeOriginal.HasValue && x.ExitTimeOriginal.HasValue)
														.Where(
														x => x.EmployeeUID == employeeGuid &&
														x.EnterTimeOriginal.Value.Date >= minIntervalsDate.Date &&
														x.ExitTimeOriginal.Value.Date <= maxIntervalDate).ToList();
			var conflictedIntervals = GetIntervalsWithConflicts(dayTimeTrackParts, linkedIntervals);

			return new OperationResult<Dictionary<DayTimeTrackPart, List<DayTimeTrackPart>>>(conflictedIntervals);
		}

		public OperationResult<IEnumerable<DayTimeTrackPart>> GetIntersectionIntervals(DayTimeTrackPart currentDayTimeTrackPart,
			ShortEmployee currentEmployee)
		{
			var linkedIntervals = Context.PassJournals.Where(x => x.EmployeeUID == currentEmployee.UID && x.UID != currentDayTimeTrackPart.UID)
				.Where(
					x =>
						currentDayTimeTrackPart.ExitDateTime >= x.EnterTime
						&& currentDayTimeTrackPart.EnterDateTime <= x.ExitTime)
				.Union(Context.PassJournals.Where(x => x.EmployeeUID == currentEmployee.UID && x.IsOpen)).ToList();

			var resultCollection = new List<DayTimeTrackPart>();
			foreach (var linkedInterval in linkedIntervals)
			{
				var skdZone = SKDManager.Zones.FirstOrDefault(x => x.UID == linkedInterval.ZoneUID);

				if (linkedInterval.IsOpen)
				{
					if (currentDayTimeTrackPart.ExitDateTime > linkedInterval.EnterTime)
					{


						resultCollection.Add(new DayTimeTrackPart
						{
							UID = linkedInterval.UID,
							EnterDateTime = linkedInterval.EnterTime,
							ExitDateTime = linkedInterval.ExitTime,
							TimeTrackZone = new TimeTrackZone
							{
								Name = skdZone != null ? skdZone.Name : string.Empty
							},
						});
					}
				}
				else if (linkedInterval.EnterTime < currentDayTimeTrackPart.ExitDateTime &&
				          linkedInterval.ExitTime > currentDayTimeTrackPart.EnterDateTime)
				{
					resultCollection.Add(new DayTimeTrackPart
					{
						UID = linkedInterval.UID,
						EnterDateTime = linkedInterval.EnterTime,
						ExitDateTime = linkedInterval.ExitTime,
						TimeTrackZone = new TimeTrackZone { Name = skdZone != null ? skdZone.Name : string.Empty }
					});
				}
			}

			return new OperationResult<IEnumerable<DayTimeTrackPart>>(resultCollection);
		}

		public OperationResult EditPassJournal(DayTimeTrackPart dayTimeTrackPart, ShortEmployee employee, out bool? setAdjustmentFlag, out bool setBordersChangedFlag, out bool setForceClosedFlag)
		{
			setAdjustmentFlag = null;
			setBordersChangedFlag = default(bool);
			setForceClosedFlag = default(bool);

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
					passJournalItem.ExitTime = dayTimeTrackPart.ExitDateTime;
					passJournalItem.IsNeedAdjustment = dayTimeTrackPart.IsNeedAdjustment;
					passJournalItem.AdjustmentDate = dayTimeTrackPart.AdjustmentDate;
					passJournalItem.CorrectedByUID = dayTimeTrackPart.CorrectedByUID;
					passJournalItem.NotTakeInCalculations = dayTimeTrackPart.NotTakeInCalculations;
					passJournalItem.IsAddedManually = dayTimeTrackPart.IsManuallyAdded;
					if (dayTimeTrackPart.IsForceClosed)
					{
						passJournalItem.ExitTimeOriginal = passJournalItem.ExitTime;
						passJournalItem.IsOpen = default(bool);
						passJournalItem.IsForceClosed = true;
						setForceClosedFlag = true;
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

			var linkedIntervals = Context.PassJournals.Where(x => x.EmployeeUID == employee.UID && x.UID != dayTimeTrackPart.UID)
				.Where(
					x =>
						dayTimeTrackPart.ExitDateTime > x.EnterTime
						&& dayTimeTrackPart.EnterDateTime < x.ExitTime).ToList();

			if(linkedIntervals.Any()) return new OperationResult("Данный интервал является пересекающимся");

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

		public OperationResult RemoveSelectedIntervals(IEnumerable<DayTimeTrackPart> removedDayTimeTrackParts)
		{
			try
			{
				foreach (var removedDayTimeTrackPart in removedDayTimeTrackParts)
				{
					DeleteAllPassJournalItems(removedDayTimeTrackPart);
				}
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

			var intervalsCollection = new List<PassJournal>();

			var firstdayOfCurrentMonth = new DateTime(date.Year, date.Month, 1);
			if(date.Date == firstdayOfCurrentMonth)
				intervalsCollection = passJournals
					.Where(x => x.EmployeeUID == employee.UID)
					.Where(x => (x.EnterTime < date.Date && !x.ExitTime.HasValue) || (x.EnterTime < date.Date && x.ExitTime.HasValue && x.ExitTime.Value.Date >= date.Date))
					.ToList();

			intervalsCollection.AddRange(passJournals.Where(x => x.EmployeeUID == employee.UID && x.EnterTime.Date == date.Date));

			foreach (var passJournal in intervalsCollection)
			{
				var timeTrackPart = new TimeTrackPart
				{
					EnterDateTime = passJournal.EnterTime,
					ExitDateTime =  passJournal.ExitTime,
					ZoneUID = passJournal.ZoneUID,
					IsForURVZone = schedule.ScheduleZones.Any(x => x.ZoneUID == passJournal.ZoneUID),
					PassJournalUID = passJournal.UID,
					IsManuallyAdded = passJournal.IsAddedManually,
					NotTakeInCalculations = passJournal.NotTakeInCalculations,
					IsNeedAdjustment = passJournal.IsNeedAdjustment,
					AdjustmentDate = passJournal.AdjustmentDate,
					CorrectedByUID = passJournal.CorrectedByUID,
					EnterTimeOriginal = passJournal.EnterTimeOriginal,
					ExitTimeOriginal = passJournal.ExitTimeOriginal,
					IsOpen = passJournal.IsOpen,
					IsForceClosed = passJournal.IsForceClosed,
					IsNeedAdjustmentOriginal = passJournal.IsNeedAdjustmentOriginal
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