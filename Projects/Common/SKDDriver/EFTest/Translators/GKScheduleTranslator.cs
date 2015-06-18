using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
namespace SKDDriver.DataClasses
{
	public class GKScheduleTranslator
	{
		DbService DbService; 
		SKDDbContext Context;

		public GKScheduleTranslator(DbService context)
		{
			DbService = context;
			Context = DbService.Context;
		}
		
		public OperationResult<List<FiresecAPI.GK.GKSchedule>> Get()
		{
			try
			{
				var result = Context.GKSchedules.Include(x => x.ScheduleDays).Include(x => x.ScheduleGKDaySchedules).ToList()
					.Select(x => Translate(x)).ToList();
				return new OperationResult<List<FiresecAPI.GK.GKSchedule>>(result);
			}
			catch (Exception e)
			{
				return OperationResult<List<FiresecAPI.GK.GKSchedule>>.FromError(e.Message);
			}
		}

		public FiresecAPI.GK.GKSchedule Translate(GKSchedule tableItem)
		{
			var result = new FiresecAPI.GK.GKSchedule();
			result.UID = tableItem.UID;
			result.No = tableItem.No;
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.ScheduleType = (FiresecAPI.GK.GKScheduleType)tableItem.Type;
			result.SchedulePeriodType = (FiresecAPI.GK.GKSchedulePeriodType)tableItem.PeriodType;
			result.StartDateTime = tableItem.StartDateTime;
			result.HoursPeriod = tableItem.HoursPeriod;
			result.HolidayScheduleNo = tableItem.HolidayScheduleNo;
			result.WorkHolidayScheduleNo = tableItem.WorkingHolidayScheduleNo;
			result.DayScheduleUIDs = 
				tableItem.ScheduleGKDaySchedules.Select(x => x.DayScheduleUID.GetValueOrDefault()).ToList();
			result.Calendar = new Calendar
			{
				SelectedDays = tableItem.ScheduleDays.Select(x => x.DateTime).ToList(),
				Year = tableItem.Year
			};
			return result;
		}

		public OperationResult Save(FiresecAPI.GK.GKSchedule item)
		{
			try
			{
				bool isNew = false;
				var tableItem = Context.GKSchedules.Include(x=>x.ScheduleDays).Include(x => x.ScheduleGKDaySchedules).FirstOrDefault(x => x.UID == item.UID);
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
				tableItem.Year = item.Calendar.Year;
				tableItem.Type = (int)item.ScheduleType;
				tableItem.PeriodType = (int)item.SchedulePeriodType;
				tableItem.StartDateTime = TranslatiorHelper.CheckDate(item.StartDateTime);
				tableItem.HoursPeriod = item.HoursPeriod;
				tableItem.HolidayScheduleNo = item.HolidayScheduleNo;
				tableItem.WorkingHolidayScheduleNo = item.WorkHolidayScheduleNo;
				tableItem.ScheduleDays = item.Calendar.SelectedDays.Select(x => new GKScheduleDay
				{
					UID = Guid.NewGuid(),
					ScheduleUID = item.UID,
					DateTime = x
				}).ToList();
				tableItem.ScheduleGKDaySchedules = item.DayScheduleUIDs.Select(x => new ScheduleGKDaySchedule
				{
					UID = Guid.NewGuid(),
					ScheduleUID = item.UID,
					DayScheduleUID = x
				}).ToList();
				if(isNew)
					Context.GKSchedules.Add(tableItem);
				Context.SaveChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult Delete(FiresecAPI.GK.GKSchedule item)
		{
			try
			{
				var tableItem = Context.GKSchedules.FirstOrDefault(x => x.UID == item.UID);
				if (tableItem != null)
				{
					Context.GKSchedules.Remove(tableItem);
				}
				Context.SaveChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}
	}
}
