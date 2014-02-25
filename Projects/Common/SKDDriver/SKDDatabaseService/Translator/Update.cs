using System;
using SKDDriver.DataAccess;

namespace SKDDriver
{
	static partial class Translator
	{
		public static void Update(Department item, FiresecAPI.Department apiItem)
		{
			if (apiItem == null)
				return;
			item.Name = apiItem.Name;
			item.Description = apiItem.Description;
			item.ParentDepartmentUid = apiItem.ParentDepartmentUid;
			item.ContactEmployeeUid = apiItem.ContactEmployeeUid;
			item.AttendantUid = apiItem.AttendantEmployeeUId;
			UpdateOrganizationElement(item, apiItem);
		}

		public static void Update(Employee item, FiresecAPI.Employee apiItem)
		{
			if (apiItem == null)
				return;
			item.FirstName = apiItem.FirstName;
			item.SecondName = apiItem.SecondName;
			item.LastName = apiItem.LastName;
			item.Appointed = apiItem.Appointed;
			item.Dismissed = apiItem.Dismissed;
			item.PositionUid = apiItem.PositionUid;
			item.DepartmentUid = apiItem.DepartmentUid;
			item.ScheduleUid = apiItem.ScheduleUid;
			item.Type = (int)apiItem.Type;
			UpdateOrganizationElement(item, apiItem);
		}

		public static void Update(Journal item, FiresecAPI.SKDJournalItem apiItem)
		{
			if (apiItem == null)
				return;
			item.CardNo = apiItem.CardNo;
			item.Description = apiItem.Description;
			item.DeviceDate = apiItem.DeviceDateTime;
			item.DeviceNo = apiItem.DeviceJournalRecordNo;
			item.IpPort = apiItem.IpAddress;
			item.Name = apiItem.Name;
			item.SysemDate = CheckDate(apiItem.SystemDateTime.GetValueOrDefault(new DateTime()));
			//UpdateBase(item, apiItem);
		}

		public static void Update(Frame item, FiresecAPI.Frame apiItem)
		{
			if (apiItem == null)
				return;
			item.CameraUid = apiItem.CameraUid;
			item.DateTime = CheckDate(apiItem.DateTime.GetValueOrDefault(new DateTime()));
			item.FrameData = apiItem.FrameData;
			item.JournalItemUid = apiItem.JournalItemUid;
			UpdateBase(item, apiItem);
		}

		public static void Update(Card item, FiresecAPI.SKDCard apiItem)
		{
			if (apiItem == null)
				return;
			item.Number = apiItem.Number;
			item.Series = apiItem.Series;
			item.EmployeeUid = apiItem.HolderUid;
			item.ValidFrom = CheckDate(apiItem.ValidFrom);
			item.ValidTo = CheckDate(apiItem.ValidTo);
			item.IsAntipass = apiItem.IsAntipass;
			item.IsInStopList = apiItem.IsInStopList;
			item.StopReason = apiItem.StopReason;
			UpdateBase(item, apiItem);
		}

		public static void Update(CardZoneLink item, FiresecAPI.CardZoneLink apiItem)
		{
			if (apiItem == null)
				return;
			item.IntervalType = (int?)apiItem.IntervalType;
			item.IntervalUid = apiItem.IntervalUid;
			item.IsWithEscort = apiItem.IsWithEscort;
			item.ZoneUid = apiItem.ZoneUid;
			item.CardUid = apiItem.CardUid;
			UpdateBase(item, apiItem);
		}

		public static void Update(Organization item, FiresecAPI.Organization apiItem)
		{
			if (apiItem == null)
				return;
			item.Name = apiItem.Name;
			item.Description = apiItem.Description;
			UpdateBase(item, apiItem);
		}

		static void UpdateBase(IDatabaseElement item, FiresecAPI.SKDModelBase apiItem)
		{
			item.IsDeleted = apiItem.IsDeleted;
			item.RemovalDate = apiItem.RemovalDate;
		}

		static void UpdateOrganizationElement(IOrganizationDatabaseElement item, FiresecAPI.OrganizationElementBase apiItem)
		{
			item.OrganizationUid = apiItem.OrganizationUid;
		}

		static readonly DateTime MinYear = new DateTime(1900, 1, 1);
		static readonly DateTime MaxYear = new DateTime(9000, 1, 1);

		static DateTime CheckDate(DateTime dateTime)
		{
			if (dateTime < MinYear)
				return MinYear;
			if (dateTime > MaxYear)
				return MaxYear;
			return dateTime;
		}
	}
}