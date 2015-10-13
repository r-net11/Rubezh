using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using RubezhAPI;
using RubezhAPI.GK;
namespace RubezhDAL.DataClasses
{
	public class GKScheduleTranslator
	{
		DbService DbService;
		DatabaseContext Context;

		public GKScheduleTranslator(DbService context)
		{
			DbService = context;
			Context = DbService.Context;
		}

		public OperationResult<List<RubezhAPI.GK.GKSchedule>> Get()
		{
			try
			{
				var result = GetTableItems().ToList().Select(x => Translate(x)).ToList();
				return new OperationResult<List<RubezhAPI.GK.GKSchedule>>(result);
			}
			catch (Exception e)
			{
				return OperationResult<List<RubezhAPI.GK.GKSchedule>>.FromError(e.Message);
			}
		}

		public RubezhAPI.GK.GKSchedule Translate(GKSchedule tableItem)
		{
			var result = new RubezhAPI.GK.GKSchedule();
			result.UID = tableItem.UID;
			result.No = tableItem.No;
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.ScheduleType = (RubezhAPI.GK.GKScheduleType)tableItem.Type;
			result.SchedulePeriodType = (RubezhAPI.GK.GKSchedulePeriodType)tableItem.PeriodType;
			result.StartDateTime = tableItem.StartDateTime;
			result.HoursPeriod = tableItem.HoursPeriod;
			result.HolidayScheduleNo = tableItem.HolidayScheduleNo;
			result.WorkHolidayScheduleNo = tableItem.WorkingHolidayScheduleNo;
			result.ScheduleParts =
				tableItem.ScheduleGKDaySchedules.Select(x => new GKSchedulePart() { DayScheduleUID = x.DayScheduleUID.GetValueOrDefault(), DayNo = x.DayNo }).ToList();
			result.SelectedDays = tableItem.ScheduleDays.Select(x => x.DateTime).ToList();
			result.Year = tableItem.Year;
			return result;
		}

		public OperationResult Save(RubezhAPI.GK.GKSchedule item)
		{
			try
			{
				bool isNew = false;
				var tableItem = GetTableItems().FirstOrDefault(x => x.UID == item.UID);
				if (tableItem == null)
				{
					isNew = true;
					tableItem = new GKSchedule { UID = item.UID };
				}
				else
				{
					Context.GKScheduleDays.RemoveRange(tableItem.ScheduleDays);
					Context.ScheduleGKDaySchedules.RemoveRange(tableItem.ScheduleGKDaySchedules);
				}
				tableItem.No = item.No;
				tableItem.Name = item.Name;
				tableItem.Description = item.Description;
				tableItem.Year = item.Year;
				tableItem.Type = (int)item.ScheduleType;
				tableItem.PeriodType = (int)item.SchedulePeriodType;
				tableItem.StartDateTime = item.StartDateTime.CheckDate();
				tableItem.HoursPeriod = item.HoursPeriod;
				tableItem.HolidayScheduleNo = item.HolidayScheduleNo;
				tableItem.WorkingHolidayScheduleNo = item.WorkHolidayScheduleNo;
				tableItem.ScheduleDays = item.SelectedDays.Select(x => new GKScheduleDay
				{
					UID = Guid.NewGuid(),
					ScheduleUID = item.UID,
					DateTime = x.CheckDate()
				}).ToList();
				tableItem.ScheduleGKDaySchedules = item.ScheduleParts.Select(x => new ScheduleGKDaySchedule
				{
					UID = Guid.NewGuid(),
					ScheduleUID = item.UID,
					DayScheduleUID = x.DayScheduleUID.EmptyToNull(),
					DayNo = x.DayNo
				}).ToList();
				if (isNew)
					Context.GKSchedules.Add(tableItem);
				Context.SaveChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult Delete(RubezhAPI.GK.GKSchedule item)
		{
			try
			{
				var tableItem = GetTableItems().FirstOrDefault(x => x.UID == item.UID);
				if (tableItem != null)
				{
					Context.GKSchedules.Remove(tableItem);
					Context.SaveChanges();
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		IQueryable<GKSchedule> GetTableItems()
		{
			return Context.GKSchedules.Include(x => x.ScheduleDays).Include(x => x.ScheduleGKDaySchedules);
		}
	}
}