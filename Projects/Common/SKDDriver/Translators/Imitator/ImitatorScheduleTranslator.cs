using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace SKDDriver.DataClasses
{
	public class ImitatorScheduleTranslator
	{
		public DbService DbService { get; private set; }
		public DatabaseContext Context { get { return DbService.Context; } }

		public ImitatorScheduleTranslator(DbService dbService)
		{
			DbService = dbService;
		}

		public List<ImitatorSchedule> Get()
		{
			try
			{
				var schedules = Context.ImitatorSchedules.Include(x => x.ImitatorSheduleIntervals).ToList();
				return schedules;
			}
			catch
			{
				return null;
			}
		}

		public void AddOrEdit(ImitatorSchedule imitatorSchedule)
		{
			try
			{
				var existingImitatorSchedule = Context.ImitatorSchedules.FirstOrDefault(x => x.No == imitatorSchedule.No);
				if (existingImitatorSchedule != null)
				{
					Context.ImitatorSheduleIntervals.RemoveRange(existingImitatorSchedule.ImitatorSheduleIntervals);
					existingImitatorSchedule.Name = imitatorSchedule.Name;
					existingImitatorSchedule.HolidayScheduleNo = imitatorSchedule.HolidayScheduleNo;
					existingImitatorSchedule.PartsCount = imitatorSchedule.PartsCount;
					existingImitatorSchedule.TotalSeconds = imitatorSchedule.TotalSeconds;
					existingImitatorSchedule.WorkHolidayScheduleNo = imitatorSchedule.WorkHolidayScheduleNo;
					existingImitatorSchedule.ImitatorSheduleIntervals = imitatorSchedule.ImitatorSheduleIntervals;
				}
				else
				{
					Context.ImitatorSchedules.Add(imitatorSchedule);
				}
				Context.SaveChanges();
			}
			catch
			{
			}
		}
	}
}