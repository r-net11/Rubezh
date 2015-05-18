using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
namespace SKDDriver
{
	public class GKScheduleTranslator
	{
		protected SKDDatabaseService DatabaseService;
		protected DataAccess.SKDDataContext Context;

		public GKScheduleTranslator(SKDDatabaseService databaseService)
		{
			DatabaseService = databaseService;
			Context = databaseService.Context;
		}

		T TranslateModelBase<T, TTableItem>(TTableItem tableItem)
			where T : ModelBase, new()
			where TTableItem : DataAccess.IGKModelBase
		{
			return new T
			{
				UID = tableItem.UID,
				No = tableItem.No,
				Name = tableItem.Name,
				Description = tableItem.Description
			};
		}

		void TranslateModelBaseBack<T, TTableItem>(T item, TTableItem tableItem)
			where T : ModelBase, new()
			where TTableItem : DataAccess.IGKModelBase
		{
			tableItem.UID = item.UID;
			tableItem.No = item.No;
			tableItem.Name = item.Name;
			tableItem.Description = item.Description;
		}

		GKSchedule TranslateGKSchdule(DataAccess.GKSchedule tableItem, IEnumerable<DataAccess.GKScheduleDay> scheduleDays, IEnumerable<Guid> dayScheduleUIDs)
		{
			var result = TranslateModelBase<GKSchedule, DataAccess.GKSchedule>(tableItem);
			result.Calendar = new Calendar
			{
				SelectedDays = scheduleDays.Select(x => x.DateTime).ToList(),
				Year = tableItem.Year
			};
			result.ScheduleType = (GKScheduleType)tableItem.Type;
			result.SchedulePeriodType = (GKSchedulePeriodType)tableItem.PeriodType;
			result.StartDateTime = tableItem.StartDateTime;
			result.HoursPeriod = tableItem.HoursPeriod;
			result.HolidayScheduleNo = tableItem.HolidayScheduleNo;
			result.WorkHolidayScheduleNo = tableItem.WorkingHolidayScheduleNo;
			result.DayScheduleUIDs = dayScheduleUIDs.ToList();
			return result;
		}

		GKDaySchedule TranslateGKDaySchedule(DataAccess.GKDaySchedule daySchedule, IEnumerable<DataAccess.GKDaySchedulePart> dayScheduleParts)
		{
			var result = TranslateModelBase<GKDaySchedule, DataAccess.GKDaySchedule>(daySchedule);
			result.DayScheduleParts = new List<GKDaySchedulePart>();
			foreach (var tableDaySchedulePart in dayScheduleParts)
			{
				var daySchedulePart = TranslateModelBase<GKDaySchedulePart, DataAccess.GKDaySchedulePart>(tableDaySchedulePart);
				daySchedulePart.StartMilliseconds = tableDaySchedulePart.StartMilliseconds;
				daySchedulePart.EndMilliseconds = tableDaySchedulePart.EndMilliseconds;
				result.DayScheduleParts.Add(daySchedulePart);
			}
			return result;
		}

		public OperationResult<List<GKDaySchedule>> GetDaySchedules()
		{
			try
			{
				var result = new List<GKDaySchedule>();
				var tableItems = Context.GKDaySchedules.GroupJoin(Context.GKDayScheduleParts,
					daySchedule => daySchedule.UID,
					daySchedulePart => daySchedulePart.DayScheduleUID,
					(daySchedule, dayScheduleParts) => new { daySchedule, dayScheduleParts });
				foreach (var item in tableItems)
				{
					result.Add(TranslateGKDaySchedule(item.daySchedule, item.dayScheduleParts));
				}
				return new OperationResult<List<GKDaySchedule>>(result);
			}
			catch (Exception e)
			{
				return OperationResult<List<GKDaySchedule>>.FromError(e.Message);
			}
		}

		public OperationResult<List<GKSchedule>> GetSchedules()
		{
			try
			{
				var result = new List<GKSchedule>();
				var tableItems = Context.GKSchedules.GroupJoin(Context.GKScheduleDays,
					schedule => schedule.UID,
					scheduleDay => scheduleDay.ScheduleUID,
					(schedule, scheduleDays) => new { schedule, scheduleDays }).
					GroupJoin(Context.ScheduleGKDaySchedules,
					schedule => schedule.schedule.UID,
					daySchedule => daySchedule.ScheduleUID,
					(scheduleWithDays, daySchedules) => new { scheduleWithDays, daySchedules });
				foreach (var item in tableItems)
				{
					result.Add(TranslateGKSchdule(item.scheduleWithDays.schedule, item.scheduleWithDays.scheduleDays, item.daySchedules.Select(x => x.DayScheduleUID)));
				}
				return new OperationResult<List<GKSchedule>>(result);
			}
			catch (Exception e)
			{
				return OperationResult<List<GKSchedule>>.FromError(e.Message);
			}
		}

		public OperationResult SaveDaySchedule(GKDaySchedule daySchedule)
		{
			try
			{
				var tableDaySchedule = Context.GKDaySchedules.FirstOrDefault(x => x.UID == daySchedule.UID);
				var isNew = tableDaySchedule == null;
				if (!isNew)
				{
					var DayScheduleDaysToDelete = Context.GKDayScheduleParts.Where(x => x.DayScheduleUID == daySchedule.UID);
					Context.GKDayScheduleParts.DeleteAllOnSubmit(DayScheduleDaysToDelete);
				}
				else
				{
					tableDaySchedule = new DataAccess.GKDaySchedule { UID = daySchedule.UID };
				}
				TranslateModelBaseBack(daySchedule, tableDaySchedule);
				if (isNew)
					Context.GKDaySchedules.InsertOnSubmit(tableDaySchedule);
				var tableDayScheduleParts = new List<DataAccess.GKDaySchedulePart>();
				foreach (var daySchedulePart in daySchedule.DayScheduleParts)
				{
					var tableDayScheduePart = new DataAccess.GKDaySchedulePart
					{
						UID = daySchedulePart.UID,
						DayScheduleUID = tableDaySchedule.UID,
						StartMilliseconds = daySchedulePart.StartMilliseconds,
						EndMilliseconds = daySchedulePart.EndMilliseconds
					};
					TranslateModelBaseBack(daySchedulePart, tableDayScheduePart);
					tableDayScheduleParts.Add(tableDayScheduePart);
				}
				Context.GKDayScheduleParts.InsertAllOnSubmit(tableDayScheduleParts);
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult SaveSchedule(GKSchedule item)
		{
			try
			{
				var tableItem = Context.GKSchedules.FirstOrDefault(x => x.UID == item.UID);
				var isNew = tableItem == null;
				if (!isNew)
				{
					var scheduleDaysToDelete = Context.GKScheduleDays.Where(x => x.ScheduleUID == item.UID);
					Context.GKScheduleDays.DeleteAllOnSubmit(scheduleDaysToDelete);
					var daySchedulesToDelete = Context.ScheduleGKDaySchedules.Where(x => x.ScheduleUID == item.UID);
					Context.ScheduleGKDaySchedules.DeleteAllOnSubmit(daySchedulesToDelete);
				}
				else
				{
					tableItem = new DataAccess.GKSchedule { UID = item.UID };
				}
				TranslateModelBaseBack(item, tableItem);
				tableItem.Year = item.Calendar.Year;
				tableItem.Type = (int)item.ScheduleType;
				tableItem.PeriodType = (int)item.SchedulePeriodType;
				tableItem.StartDateTime = TranslatiorHelper.CheckDate(item.StartDateTime);
				tableItem.HoursPeriod = item.HoursPeriod;
				tableItem.HolidayScheduleNo = item.HolidayScheduleNo;
				tableItem.WorkingHolidayScheduleNo = item.WorkHolidayScheduleNo;
				if (isNew)
					Context.GKSchedules.InsertOnSubmit(tableItem);
				var scheduleDays = new List<DataAccess.GKScheduleDay>();
				foreach (var scheduleDayTime in item.Calendar.SelectedDays)
				{
					scheduleDays.Add(new DataAccess.GKScheduleDay { UID = Guid.NewGuid(), ScheduleUID = item.UID, DateTime = scheduleDayTime });
				}
				Context.GKScheduleDays.InsertAllOnSubmit(scheduleDays);
				var daySchedules = new List<DataAccess.ScheduleGKDaySchedule>();
				foreach (var dayScheduleUID in item.DayScheduleUIDs)
				{
					daySchedules.Add(new DataAccess.ScheduleGKDaySchedule { UID = Guid.NewGuid(), ScheduleUID = item.UID, DayScheduleUID = dayScheduleUID });
				}
				Context.ScheduleGKDaySchedules.InsertAllOnSubmit(daySchedules);
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}


		public OperationResult DeleteDaySchedule(GKDaySchedule daySchedule)
		{
			try
			{
				var tableDaySchedule = Context.GKDaySchedules.FirstOrDefault(x => x.UID == daySchedule.UID);
				if (tableDaySchedule != null)
				{
					var DayScheduleDaysToDelete = Context.GKDayScheduleParts.Where(x => x.DayScheduleUID == daySchedule.UID);
					Context.GKDayScheduleParts.DeleteAllOnSubmit(DayScheduleDaysToDelete);
					Context.GKDaySchedules.DeleteOnSubmit(tableDaySchedule);
				}
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult DeleteSchedule(GKSchedule item)
		{
			try
			{
				var tableItem = Context.GKSchedules.FirstOrDefault(x => x.UID == item.UID);
				if (tableItem != null)
				{
					var scheduleDaysToDelete = Context.GKScheduleDays.Where(x => x.ScheduleUID == item.UID);
					Context.GKScheduleDays.DeleteAllOnSubmit(scheduleDaysToDelete);
					var daySchedulesToDelete = Context.ScheduleGKDaySchedules.Where(x => x.ScheduleUID == item.UID);
					Context.ScheduleGKDaySchedules.DeleteAllOnSubmit(daySchedulesToDelete);
					Context.GKSchedules.DeleteOnSubmit(tableItem);
				}
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

	}
}