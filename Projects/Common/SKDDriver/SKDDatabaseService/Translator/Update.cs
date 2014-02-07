using System;
using SKDDriver.DataAccess;

namespace SKDDriver
{
	static partial class Translator
	{
		public static void Update(Position position, FiresecAPI.Position apiPosition)
		{
			if (apiPosition == null)
				return;
			position.Name = apiPosition.Name;
			position.Description = apiPosition.Description;
			return;
		}
		
		public static void Update(Department department, FiresecAPI.Department apiDepartment)
		{
			if (apiDepartment == null)
				return;
			department.Name = apiDepartment.Name;
			department.Description = apiDepartment.Description;
			department.ParentDepartmentUid = apiDepartment.ParentDepartmentUid;
			department.ContactEmployeeUid = apiDepartment.ContactEmployeeUid;
			department.AttendantUid = apiDepartment.AttendantEmployeeUId;
		}


		public static void Update(Employee employee, FiresecAPI.Employee apiEmployee)
		{
			if (apiEmployee == null)
				return;
			employee.FirstName = apiEmployee.FirstName;
			employee.SecondName = apiEmployee.SecondName;
			employee.LastName = apiEmployee.LastName;
			employee.Appointed = apiEmployee.Appointed;
			employee.Dismissed = apiEmployee.Dismissed;
			employee.PositionUid = apiEmployee.PositionUid;
			employee.DepartmentUid = apiEmployee.DepartmentUid;
			employee.ScheduleUid = apiEmployee.ScheduleUid;
			return;
		}

		public static void Update(Journal journalItem, FiresecAPI.SKDJournalItem apiJournalItem)
		{
			if (apiJournalItem == null)
				return;
			journalItem.CardNo = apiJournalItem.CardNo;
			journalItem.Description = apiJournalItem.Description;
			journalItem.DeviceDate = apiJournalItem.DeviceDateTime;
			journalItem.DeviceNo = apiJournalItem.DeviceJournalRecordNo;
			journalItem.IpPort = apiJournalItem.IpAddress;
			journalItem.Name = apiJournalItem.Name;
			journalItem.SysemDate = CheckDate(apiJournalItem.SystemDateTime);
		}

		public static void Update(Frame frame, FiresecAPI.Frame apiFrame)
		{
			if (apiFrame == null)
				return;
			frame.CameraUid = apiFrame.CameraUid;
			frame.DateTime = CheckDate(apiFrame.DateTime);
			frame.FrameData = apiFrame.FrameData;
			frame.JournalItemUid = apiFrame.JournalItemUid;
		}

		public static void Update(Card card, FiresecAPI.SKDCard apiCard)
		{
			if (apiCard == null)
				return;
			card.Number = apiCard.Number;
			card.Series = apiCard.Series;
			card.EmployeeUid = apiCard.EmployeeUid;
			card.ValidFrom = CheckDate(apiCard.ValidFrom);
			card.ValidTo = CheckDate(apiCard.ValidTo);
			card.IsAntipass = apiCard.IsAntipass;
			card.IsInStopList = apiCard.IsInStopList;
			card.StopReason = apiCard.StopReason;
		}

		public static void Update(CardZoneLink cardZoneLink, FiresecAPI.CardZoneLink apiCardZoneLink)
		{
			if (apiCardZoneLink == null)
				return;
			cardZoneLink.IntervalType = apiCardZoneLink.IntervalType.ToString();
			cardZoneLink.IntervalUid = apiCardZoneLink.IntervalUid;
			cardZoneLink.IsWithEscort = apiCardZoneLink.IsWithEscort;
			cardZoneLink.ZoneUid = apiCardZoneLink.ZoneUid;
			cardZoneLink.CardUid = apiCardZoneLink.CardUid;
		}
	}
}
