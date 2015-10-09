using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace RubezhDAL.DataClasses
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
				return Context.ImitatorSchedules.Include(x => x.ImitatorSheduleIntervals).OrderBy(x => x.No).ToList();
			}
			catch
			{
				return null;
			}
		}

		public ImitatorSchedule GetByNo(int no)
		{
			try
			{
				return Context.ImitatorSchedules.Include(x => x.ImitatorSheduleIntervals).Where(x => x.No == no).FirstOrDefault();
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
				var existingImitatorSchedule = Context.ImitatorSchedules.Include(x => x.ImitatorSheduleIntervals).FirstOrDefault(x => x.No == imitatorSchedule.No);
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
			catch { }
		}
	}
}