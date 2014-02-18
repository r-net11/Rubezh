using System;
using SKDDriver.DataAccess;

namespace SKDDriver
{
	static partial class Translator
	{
		public static Position TranslateBack(FiresecAPI.Position item)
		{
			if (item == null)
				return null;
			var result = new Position
			{
				Name = item.Name,
				Description = item.Description
			};
			TranslateBackOrganizationElement(item, result);
			return result;
		}

		public static Department TranslateBack(FiresecAPI.Department item)
		{
			if (item == null)
				return null;
			var result = new Department
			{
				Name = item.Name,
				Description = item.Description,
				ParentDepartmentUid = item.ParentDepartmentUid,
				ContactEmployeeUid = item.ContactEmployeeUid,
				AttendantUid = item.AttendantEmployeeUId,
			};
			TranslateBackOrganizationElement(item, result);
			return result;
		}

		public static Employee TranslateBack(FiresecAPI.Employee item)
		{
			if (item == null)
				return null;
			var result = new Employee
			{
				FirstName = item.FirstName,
				SecondName = item.SecondName,
				LastName = item.LastName,
				Appointed = CheckDate(item.Appointed),
				Dismissed = CheckDate(item.Dismissed),
				PositionUid = item.PositionUid,
				DepartmentUid = item.DepartmentUid,
				ScheduleUid = item.ScheduleUid,
				Type = (int)item.Type
			};
			TranslateBackOrganizationElement(item, result);
			return result;
		}

		public static Journal TranslateBack(FiresecAPI.SKDJournalItem item)
		{
			if (item == null)
				return null;
			var result = new Journal
			{
				CardNo = item.CardNo,
				Description = item.Description,
				DeviceDate = item.DeviceDateTime,
				DeviceNo = item.DeviceJournalRecordNo,
				IpPort = item.IpAddress,
				Name = item.Name,
				SysemDate = CheckDate(item.SystemDateTime)
			};
			TranslateBackBase(item, result);
			return result;
		}

		public static Frame TranslateBack(FiresecAPI.Frame item)
		{
			if (item == null)
				return null;
			var result = new Frame
			{
				CameraUid = item.CameraUid,
				DateTime = CheckDate(item.DateTime),
				FrameData = item.FrameData,
				JournalItemUid = item.JournalItemUid
			};
			TranslateBackBase(item, result);
			return result;
		}

		public static Card TranslateBack(FiresecAPI.SKDCard item)
		{
			if (item == null)
				return null;
			var validFrom = CheckDate(item.ValidFrom);
			var validTo = CheckDate(item.ValidTo);
			var result = new Card
			{
				EmployeeUid = item.HolderUid,
				Number = item.Number,
				Series = item.Series,
				ValidFrom = validFrom,
				ValidTo = validTo,
				IsAntipass = item.IsAntipass,
				IsInStopList = item.IsInStopList,
				StopReason = item.StopReason
			};
			TranslateBackBase(item, result);
			return result;
		}

		public static CardZoneLink TranslateBack(FiresecAPI.CardZoneLink item)
		{
			if (item == null)
				return null;
			var result = new CardZoneLink
			{
				IntervalType = (int?)item.IntervalType,
				IntervalUid = item.IntervalUid,
				IsWithEscort = item.IsWithEscort,
				ZoneUid = item.ZoneUid,
				CardUid = item.CardUid
			};
			TranslateBackBase(item, result);
			return result;
		}

		public static Organization TranslateBack(FiresecAPI.Organization item)
		{
			if (item == null)
				return null;
			var result = new Organization
			{
				Name = item.Name,
				Description = item.Description
			};
			TranslateBackBase(item, result);
			return result;
		}

		public static Document TranslateBack(FiresecAPI.Document item)
		{
			if (item == null)
				return null;
			var result = new Document
			{
				Name = item.Name,
				Description = item.Description,
				IssueDate = item.IssueDate,
				LaunchDate = item.LaunchDate,
				No = item.No
			};
			TranslateBackBase(item, result);
			return result;
		}

		static void TranslateBackBase<T>(FiresecAPI.SKDModelBase apiItem, T item)
			where T : IDatabaseElement
		{
			item.Uid = apiItem.UID;
			item.IsDeleted = apiItem.IsDeleted;
			item.RemovalDate = CheckDate(apiItem.RemovalDate);
		}

		static void TranslateBackOrganizationElement<T>(FiresecAPI.OrganizationElementBase apiItem, T item)
			where T : IOrganizationDatabaseElement
		{
			TranslateBackBase(apiItem, item);
			item.OrganizationUid = apiItem.OrganizationUid;
		}

		static DateTime? CheckDate(DateTime? dateTime)
		{
			if (dateTime == null)
				return null;
			if (dateTime.Value.Year < 1754)
				return null;
			if (dateTime.Value.Year > 9998)
				return null;
			return dateTime;
		}
	}
}
