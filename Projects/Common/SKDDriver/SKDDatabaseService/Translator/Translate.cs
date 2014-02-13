using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SKDDriver.DataAccess;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Data.Linq;

namespace SKDDriver
{
    static partial class Translator
	{
		public static FiresecAPI.Position Translate(Position position)
		{
			if (position == null)
				return null;
			return new FiresecAPI.Position
			{
				UID = position.Uid,
				Name = position.Name,
				Description = position.Description
			};
		}

		public static FiresecAPI.Department Translate(Department department)
		{
			if (department == null)
				return null;
			var phoneUids = new List<Guid>();
			department.Phone.ToList().ForEach(x => phoneUids.Add(x.Uid));
			var childDepartmentUids = new List<Guid>();
			department.Department2.ToList().ForEach(x => childDepartmentUids.Add(x.Uid));
			var resultDepartment = new FiresecAPI.Department
			{
				Uid = department.Uid,
				Name = department.Name,
				Description = department.Description,
				ParentDepartmentUid = department.ParentDepartmentUid,
				ChildDepartmentUids = childDepartmentUids,
				ContactEmployeeUid = department.ContactEmployeeUid,
				AttendantEmployeeUId = department.AttendantUid,
				PhoneUids = phoneUids
			};
			return resultDepartment;
		}



		public static FiresecAPI.Employee Translate(Employee employee)
		{
			if (employee == null)
				return null;
			var additionalColumnUids = new List<Guid>();
			employee.AdditionalColumn.ToList().ForEach(x => additionalColumnUids.Add(x.Uid));
			Guid? replacementUid = null;
			if (employee.EmployeeReplacement != null)
				replacementUid = employee.EmployeeReplacement.Uid;
			var cardUids = new List<Guid>();
			foreach (var item in employee.Card)
			{
				cardUids.Add(item.Uid);
			}

			var resultEmployee = new FiresecAPI.Employee
			{
				UID = employee.Uid,
				FirstName = employee.FirstName,
				SecondName = employee.SecondName,
				LastName = employee.LastName,
				Appointed = employee.Appointed,
				Dismissed = employee.Dismissed,
				PositionUid = employee.PositionUid,
				ReplacementUid = replacementUid,
				DepartmentUid = employee.DepartmentUid,
				ScheduleUid = employee.ScheduleUid,
				AdditionalColumnUids = additionalColumnUids,
				CardUids = cardUids
			};
			return resultEmployee;
		}

		public static FiresecAPI.Holiday Translate(Holiday holiday)
		{
			if (holiday == null)
				return null;
			FiresecAPI.HolidayType type;
			switch (holiday.Type)
			{
				case "Holiday":
					type = FiresecAPI.HolidayType.Holiday;
					break;
				case "Reduced":
					type = FiresecAPI.HolidayType.Reduced;
					break;
				case "Transferred":
					type = FiresecAPI.HolidayType.Transferred;
					break;
				default:
					type = FiresecAPI.HolidayType.Working;
					break;
			}
			return new FiresecAPI.Holiday
			{
				Uid = holiday.Uid,
				Name = holiday.Name,
				Date = holiday.Date,
				TransferDate = holiday.TransferDate,
				Reduction = holiday.Reduction,
				Type = type
			};
		}

		public static FiresecAPI.Document Translate(Document document)
		{
			if (document == null)
				return null;
			return new FiresecAPI.Document
			{
				UID = document.Uid,
				Name = document.Name,
				Description = document.Description,
				IssueDate = document.IssueDate,
				LaunchDate = document.LaunchDate
			};
		}

		public static FiresecAPI.SKDJournalItem Translate(Journal journal)
		{
			if (journal == null)
				return null;
			return new FiresecAPI.SKDJournalItem
			{
				Uid = journal.Uid,
				Name = journal.Name,
				Description = journal.Description,
				SystemDateTime = journal.SysemDate,
				DeviceDateTime = journal.DeviceDate,
				CardNo = journal.CardNo,
				CardSeries = journal.CardSeries,
				CardUid = journal.CardUid,
				DeviceJournalRecordNo = journal.DeviceNo,
				IpAddress = journal.IpPort
			};
		}

		public static FiresecAPI.Frame Translate(Frame frame)
		{
			if (frame == null)
				return null;
			return new FiresecAPI.Frame
			{
				Uid = frame.Uid,
				CameraUid = frame.CameraUid,
				DateTime = frame.DateTime,
				FrameData = frame.FrameData.ToArray(),
				JournalItemUid = frame.JournalItemUid
			};
		}

		public static FiresecAPI.SKDCard Translate(Card card)
		{
			if (card == null)
				return null;
			var zoneUids = new List<Guid>();
			foreach (var cardZoneLink in card.CardZoneLink)
			{
				if(cardZoneLink.ZoneUid != null)
					zoneUids.Add(cardZoneLink.ZoneUid.Value);
			}
			return new FiresecAPI.SKDCard
			{
				EmployeeUid = card.EmployeeUid,
				Number = card.Number,
				Series = card.Series,
				UID = card.Uid,
				ValidFrom = card.ValidFrom,
				ValidTo = card.ValidTo,
				ZoneLinkUids = zoneUids,
				IsAntipass = card.IsAntipass,
				IsInStopList = card.IsInStopList,
				StopReason = card.StopReason
			};
		}

		public static FiresecAPI.CardZoneLink Translate(CardZoneLink cardControllerLink)
		{
			if (cardControllerLink == null)
				return null;
			FiresecAPI.IntervalType intervalType;
			switch (cardControllerLink.IntervalType)
			{
				case "Time":
					intervalType = FiresecAPI.IntervalType.Time;
					break;
				case "Weekly":
					intervalType = FiresecAPI.IntervalType.Weekly;
					break;
				case "SlideDay":
					intervalType = FiresecAPI.IntervalType.SlideDay;
					break;
				default:
					intervalType = FiresecAPI.IntervalType.SlideWeekly;
					break;
			}
			return new FiresecAPI.CardZoneLink
			{
				Uid = cardControllerLink.Uid,
				IntervalType = intervalType,
				IntervalUid = cardControllerLink.IntervalUid,
				IsWithEscort = cardControllerLink.IsWithEscort,
				ZoneUid = cardControllerLink.ZoneUid,
				CardUid = cardControllerLink.CardUid,
			};
		}




		//public FiresecAPI.Models.Skud.Interval Translate(Interval interval)
        //{
        //    if (interval == null)
        //        return null;
        //    FiresecAPI.Models.Skud.Transition transitition;
        //    switch(interval.Transition)
        //    {
        //        case "Day":
        //            transitition = FiresecAPI.Models.Skud.Transition.Day;
        //            break;
        //        case "Night":
        //            transitition = FiresecAPI.Models.Skud.Transition.Night;
        //            break;
        //        default:
        //            transitition = FiresecAPI.Models.Skud.Transition.DayNight;
        //            break;
        //    };
        //    return new FiresecAPI.Models.Skud.Interval
        //    {
        //        Uid = interval.Uid,
        //        BeginDate = interval.BeginDate,
        //        EndDate = interval.EndDate,
        //        Transition = transitition
        //    };
        //}

        //public FiresecAPI.Models.Skud.NamedInterval Translate(NamedInterval namedInterval)
        //{
        //    if (namedInterval == null)
        //        return null;
        //    var intervals = new List<FiresecAPI.Models.Skud.Interval>();
        //    foreach (var item in Context.Interval.ToList())
        //    {
        //        if (item.NamedIntervalUid == namedInterval.Uid)
        //            intervals.Add(Translate(item));
        //    }
        //    return new FiresecAPI.Models.Skud.NamedInterval
        //    {
        //        Uid = namedInterval.Uid,
        //        Name = namedInterval.Name,
        //        Intervals = intervals
        //    };
        //}

        //public FiresecAPI.Models.Skud.Day Translate(Day day)
        //{
        //    if (day == null)
        //        return null;
        //    var namedInterval = Translate(Context.NamedInterval.ToList().FirstOrDefault(x => x.Uid == day.NamedIntervalUid));
        //    return new FiresecAPI.Models.Skud.Day
        //    {
        //        Uid = day.Uid,
        //        Number = day.Number,
        //        NamedInterval = namedInterval
        //    };
        //}

        //public FiresecAPI.Models.Skud.ScheduleScheme Translate(ScheduleScheme scheduleScheme)
        //{
        //    if (scheduleScheme == null)
        //        return null;
        //    var days = new List<FiresecAPI.Models.Skud.Day>();
        //    foreach (var item in Context.Day)
        //    {
        //       if(item.ScheduleSchemeUid == scheduleScheme.Uid)
        //           days.Add(Translate(item));
        //    }
        //    FiresecAPI.Models.Skud.ScheduleSchemeType type;
        //    switch(scheduleScheme.Type)
        //    {
        //        case "Month":
        //            type = FiresecAPI.Models.Skud.ScheduleSchemeType.Month;
        //            break;
        //        case "Shift":
        //            type = FiresecAPI.Models.Skud.ScheduleSchemeType.Shift;
        //            break;
        //        default:
        //            type = FiresecAPI.Models.Skud.ScheduleSchemeType.Week;
        //            break;
        //    }
        //    return new FiresecAPI.Models.Skud.ScheduleScheme
        //    {
        //        Uid = scheduleScheme.Uid,
        //        Name = scheduleScheme.Name,
        //        Days = days,
        //        Type = type
        //    };
        //}

        //public FiresecAPI.Models.Skud.RegisterDevice Translate(RegisterDevice registerDevice)
        //{
        //    if (registerDevice == null)
        //        return null;
        //    bool canControl = false;
        //    if(registerDevice.CanControl == true)
        //        canControl = true;
        //    return new FiresecAPI.Models.Skud.RegisterDevice
        //    {
        //        Uid = registerDevice.Uid,
        //        CanControl = canControl
        //    };
        //}

        //public FiresecAPI.Models.Skud.Schedule Translate(Schedule schedule)
        //{
        //    if (schedule == null)
        //        return null;
        //    var registerDevices = new List<FiresecAPI.Models.Skud.RegisterDevice>();
        //    schedule.RegisterDevice.ToList().ForEach(x => registerDevices.Add(Translate(x)));
        //    return new FiresecAPI.Models.Skud.Schedule
        //    {
        //        Uid = schedule.Uid,
        //        Name = schedule.Name,
        //        ScheduleScheme = Translate(schedule.ScheduleScheme),
        //        RegisterDevices = registerDevices
        //    };
        //}

		//public FiresecAPI.Models.Skud.AdditionalColumn Translate(AdditionalColumn additionalColumn)
		//{
		//    if (additionalColumn == null)
		//        return null;
		//    FiresecAPI.Models.Skud.AdditionalColumnType type;
		//    switch (additionalColumn.Type)
		//    {
		//        case "Graphics":
		//            type = FiresecAPI.Models.Skud.AdditionalColumnType.Graphics;
		//            break;
		//        case "Mixed":
		//            type = FiresecAPI.Models.Skud.AdditionalColumnType.Mixed;
		//            break;
		//        default:
		//            type = FiresecAPI.Models.Skud.AdditionalColumnType.Text;
		//            break;
		//    }
		//    return new FiresecAPI.Models.Skud.AdditionalColumn
		//    {
		//        Uid = additionalColumn.Uid,
		//        Name = additionalColumn.Name,
		//        Description = additionalColumn.Description,
		//        TextData = additionalColumn.TextData,
		//        GraphicsData = additionalColumn.GraphicsData.ToArray(),
		//        Type = type
		//    };
		//}

		//public FiresecAPI.Models.Skud.Phone Translate(Phone phone)
		//{
		//    if (phone == null)
		//        return null;
		//    return new FiresecAPI.Models.Skud.Phone
		//    {
		//        Uid = phone.Uid,
		//        Name = phone.Name,
		//        NumberString = phone.NumberString
		//    };
		//}

		//public FiresecAPI.Models.Skud.EmployeeReplacement Translate(EmployeeReplacement employeeReplacement)
		//{
		//    if (employeeReplacement == null)
		//        return null;
		//    return new FiresecAPI.Models.Skud.EmployeeReplacement
		//    {
		//        Uid = employeeReplacement.Uid,
		//        BeginDate = employeeReplacement.BeginDate,
		//        EndDate = employeeReplacement.EndDate,
		//        Department = Translate(employeeReplacement.Department),
		//        Schedule = Translate(employeeReplacement.Schedule)
		//    };
		//}
		
	}
}
