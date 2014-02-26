using System;
using SKDDriver.DataAccess;

namespace SKDDriver
{
	static partial class Translator
	{
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
				Appointed = CheckDate(item.Appointed.GetValueOrDefault(new DateTime())),
				Dismissed = CheckDate(item.Dismissed.GetValueOrDefault(new DateTime())),
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
				SysemDate = CheckDate(item.SystemDateTime.GetValueOrDefault(new DateTime()))
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
				DateTime = CheckDate(item.DateTime.GetValueOrDefault(new DateTime())),
				FrameData = item.FrameData,
				JournalItemUid = item.JournalItemUid
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
	}
}