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
		SKDDbContext _context;
		
		public GKScheduleTranslator(SKDDbContext context)
		{
			_context = context;
		}

		public OperationResult<List<FiresecAPI.GK.GKSchedule>> Get()
		{
			try
			{
				var result = _context.GKSchedules.Include(x => x.ScheduleDays).Include(x => x.ScheduleGKDaySchedules)
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
				var tableItem = _context.GKSchedules.Find(item.UID);
				if (tableItem == null)
				{
					isNew = true;
					tableItem = new GKSchedule { UID = item.UID };
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
					_context.GKSchedules.Add(tableItem);
				_context.SaveChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult Delete(GKSchedule item)
		{
			try
			{
				var tableItem = _context.GKSchedules.FirstOrDefault(x => x.UID == item.UID);
				if (tableItem != null)
				{
					_context.GKSchedules.Remove(tableItem);
				}
				_context.SaveChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}
	}

	public class SKDDbContext : DbContext
	{
		ContextType _contextType;

		public SKDDbContext(string connectionStringName, ContextType contextType)
			: base(connectionStringName)
		{
			_contextType = contextType;
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			string schemaStr;
			switch (_contextType)
			{
				case ContextType.MSSQL:
					schemaStr = "dbo";
					break;
				case ContextType.PostgreSQL:
					schemaStr = "public";
					break;
				default:
					schemaStr = "";
					break;
			}
			modelBuilder.Entity<GKSchedule>().ToTable("GKSchedule", schemaStr);
			modelBuilder.Entity<GKScheduleDay>().ToTable("GKScheduleDay", schemaStr);
			modelBuilder.Entity<GKDaySchedule>().ToTable("GKDaySchedule", schemaStr);
			modelBuilder.Entity<GKDaySchedulePart>().ToTable("GKDaySchedulePart", schemaStr);
			modelBuilder.Entity<ScheduleGKDaySchedule>().ToTable("ScheduleGKDaySchedule", schemaStr);
		}

		public DbSet<GKSchedule> GKSchedules { get; set; }
		public DbSet<GKScheduleDay> GKScheduleDays { get; set; }
		public DbSet<GKDaySchedule> GKDaySchedules { get; set; }
		public DbSet<GKDaySchedulePart> GKDayScheduleParts { get; set; }
		public DbSet<ScheduleGKDaySchedule> ScheduleGKDaySchedules { get; set; }
	}

	public enum ContextType
	{
		MSSQL,
		PostgreSQL
	}
}
