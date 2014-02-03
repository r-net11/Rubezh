using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;

namespace SKDDriver
{
	static class FilterHelper
	{
		public static bool IsInFilter(DataAccess.Employee employee, EmployeeFilter filter)
		{
			if (filter == null)
				return true;

			bool isInUids = IsInUidList(employee.Uid, filter.Uids);
			bool isInDepartments = IsInUidList(employee.DepartmentUid, filter.DepartmentUids);
			bool isInPositions = IsInUidList(employee.PositionUid, filter.PositionUids);
			bool isInAppointed = IsInDateTimePeriod(employee.Appointed, filter.Appointed);
			bool isInDismissed = IsInDateTimePeriod(employee.Dismissed, filter.Dismissed);
			
			return isInUids && isInDepartments && isInPositions && isInAppointed && isInDepartments;
		}

		public static bool IsInFilter(DataAccess.Journal item, SKDJournalFilter filter)
		{
			if (filter == null)
				return true;

			bool isInUids = IsInUidList(item.Uid, filter.Uids);
			bool isInSystemDate = IsInDateTimePeriod(item.SysemDate, filter.SystemDateTime);
			bool isInDeviceDate = IsInDateTimePeriod(item.DeviceDate, filter.DeviceDateTime);
			
			return isInUids && isInSystemDate && isInDeviceDate;
		}

		public static bool IsInFilter(DataAccess.Department item, DepartmentFilter filter)
		{
			if (filter == null)
				return true;

			bool isInUids = IsInUidList(item.Uid, filter.Uids);

			return isInUids;
		}

		public static bool IsInFilter(DataAccess.Position item, PositionFilter filter)
		{
			if (filter == null)
				return true;

			bool isInUids = IsInUidList(item.Uid, filter.Uids);

			return isInUids;
		}

		public static bool IsInFilter(DataAccess.Frame item, FrameFilter filter)
		{
			if (filter == null)
				return true;

			bool IsInUids = IsInUidList(item.Uid, filter.Uids);
			bool IsInCameras = IsInUidList(item.CameraUid, filter.CameraUid);
			bool IsInJournalItems = IsInUidList(item.JournalItemUid, filter.JournalItemUid);
			bool IsInDateTime = IsInDateTimePeriod(item.DateTime, filter.DateTime);

			return IsInUids && IsInCameras && IsInJournalItems && IsInDateTime;
		}

		static bool IsInDateTimePeriod(DateTime? dateTime, DateTimePeriod dateTimePeriod)
		{
			if (dateTimePeriod == null)
				return true;
			return dateTime >= dateTimePeriod.StartDate && dateTime <= dateTimePeriod.EndDate;
		}

		static bool IsInUidList(Guid? uid, List<Guid> uidList)
		{
			if (uidList == null || uidList.Count == 0)
				return true;
			return uidList.Any(x => x == uid);
		}
	}
}
