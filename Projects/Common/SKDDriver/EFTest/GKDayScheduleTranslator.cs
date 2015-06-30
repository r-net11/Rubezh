using System;
using System.Linq;
using FiresecAPI;
namespace SKDDriver.DataClasses
{
	public class GKDayScheduleTranslator
	{
		SKDDbContext _context;

		public GKDayScheduleTranslator(SKDDbContext context)
		{
			_context = context;
		}

		//public OperationResult<List<FiresecAPI.GK.GKDaySchedule>> Get()
		//{
		//    try
		//    {
		//        var result = _context.GKDaySchedules.Include(x => x.GKDayScheduleParts).Include(x => x.ScheduleGKDaySchedules)
		//            .Select(x => Translate(x)).ToList();
		//        return new OperationResult<List<FiresecAPI.GK.GKDaySchedule>>(result);
		//    }
		//    catch (Exception e)
		//    {
		//        return OperationResult<List<FiresecAPI.GK.GKDaySchedule>>.FromError(e.Message);
		//    }
		//}

		//public FiresecAPI.GK.GKDaySchedule Translate(GKDaySchedule tableItem)
		//{
		//    var result = new FiresecAPI.GK.GKDaySchedule();
		//    result.UID = tableItem.UID;
		//    result.No = tableItem.No;
		//    result.Name = tableItem.Name;
		//    result.Description = tableItem.Description;
		//    result.ScheduleType = (FiresecAPI.GK.GKDayScheduleType)tableItem.Type;
		//    result.SchedulePeriodType = (FiresecAPI.GK.GKDaySchedulePeriodType)tableItem.PeriodType;
		//    result.StartDateTime = tableItem.StartDateTime;
		//    result.HoursPeriod = tableItem.HoursPeriod;
		//    result.HolidayScheduleNo = tableItem.HolidayScheduleNo;
		//    result.WorkHolidayScheduleNo = tableItem.WorkingHolidayScheduleNo;
		//    result.DayScheduleUIDs =
		//        tableItem.ScheduleGKDaySchedules.Select(x => x.DayScheduleUID.GetValueOrDefault()).ToList();
		//    result.Calendar = new Calendar
		//    {
		//        SelectedDays = tableItem.ScheduleDays.Select(x => x.DateTime).ToList(),
		//        Year = tableItem.Year
		//    };
		//    return result;
		//}

		//public OperationResult Save(FiresecAPI.GK.GKDaySchedule item)
		//{
		//    try
		//    {
		//        bool isNew = false;
		//        var tableItem = _context.GKDaySchedules.Find(item.UID);
		//        if (tableItem == null)
		//        {
		//            isNew = true;
		//            tableItem = new GKDaySchedule { UID = item.UID };
		//        }
		//        tableItem.No = item.No;
		//        tableItem.Name = item.Name;
		//        tableItem.Description = item.Description;
		//        tableItem.Year = item.Calendar.Year;
		//        tableItem.Type = (int)item.ScheduleType;
		//        tableItem.PeriodType = (int)item.SchedulePeriodType;
		//        tableItem.StartDateTime = TranslatiorHelper.CheckDate(item.StartDateTime);
		//        tableItem.HoursPeriod = item.HoursPeriod;
		//        tableItem.HolidayScheduleNo = item.HolidayScheduleNo;
		//        tableItem.WorkingHolidayScheduleNo = item.WorkHolidayScheduleNo;
		//        tableItem.ScheduleDays = item.Calendar.SelectedDays.Select(x => new GKDayScheduleDay
		//        {
		//            UID = Guid.NewGuid(),
		//            ScheduleUID = item.UID,
		//            DateTime = x
		//        }).ToList();
		//        tableItem.ScheduleGKDaySchedules = item.DayScheduleUIDs.Select(x => new ScheduleGKDaySchedule
		//        {
		//            UID = Guid.NewGuid(),
		//            ScheduleUID = item.UID,
		//            DayScheduleUID = x
		//        }).ToList();
		//        if (isNew)
		//            _context.GKDaySchedules.Add(tableItem);
		//        _context.SaveChanges();
		//        return new OperationResult();
		//    }
		//    catch (Exception e)
		//    {
		//        return new OperationResult(e.Message);
		//    }
		//}

		public OperationResult Delete(GKDaySchedule item)
		{ 

			try
			{
				var tableItem = _context.GKDaySchedules.FirstOrDefault(x => x.UID == item.UID);
				if (tableItem != null)
				{
					_context.GKDaySchedules.Remove(tableItem);
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
}
