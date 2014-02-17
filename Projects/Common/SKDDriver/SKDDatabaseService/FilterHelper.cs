using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;

namespace SKDDriver
{
	static class FilterHelper
	{
		public static bool IsInFilter(DataAccess.Employee item, EmployeeFilter filter)
		{
			bool isInDepartments = IsInUidList(item.DepartmentUid, filter.DepartmentUids);
			bool isInPositions = IsInUidList(item.PositionUid, filter.PositionUids);
			bool isInAppointed = IsInDateTimePeriod(item.Appointed, filter.Appointed);
			bool isInDismissed = IsInDateTimePeriod(item.Dismissed, filter.Dismissed);
			return isInDepartments && isInPositions && isInAppointed && isInDepartments;// && IsInOrganizationFilterBase(item, filter);
		}

		public static bool IsInFilter(DataAccess.Journal item, SKDJournalFilter filter)
		{
			return IsInFilterBase(item, filter);
		}

		public static bool IsInFilter(DataAccess.Department item, DepartmentFilter filter)
		{
			return IsInOrganizationFilterBase(item, filter);
		}

		public static bool IsInFilter(DataAccess.Position item, PositionFilter filter)
		{
			return IsInOrganizationFilterBase(item, filter);
		}

		public static bool IsInFilter(DataAccess.Frame item, FrameFilter filter)
		{
			bool IsInCameras = IsInUidList(item.CameraUid, filter.CameraUid);
			bool IsInJournalItems = IsInUidList(item.JournalItemUid, filter.JournalItemUid);
			bool IsInDateTime = IsInDateTimePeriod(item.DateTime, filter.DateTime);
			return IsInCameras && IsInJournalItems && IsInDateTime && IsInFilterBase(item, filter);
		}

		public static bool IsInFilter(DataAccess.Card item, CardFilter filter)
		{
			bool isInEmployees = IsInUidList(item.EmployeeUid, filter.EmployeeUids);
			return isInEmployees && IsInFilterBase(item, filter);
		}

		public static bool IsInFilter(DataAccess.CardZoneLink item, CardZoneLinkFilter filter)
		{
			bool isInCardUids = IsInUidList(item.CardUid, filter.CardUids);
			bool isInZoneUids = IsInUidList(item.ZoneUid, filter.ZoneUids);
			bool isInIntervalUids = IsInUidList(item.IntervalUid, filter.IntervalUids);
			return isInCardUids && isInZoneUids && isInIntervalUids && IsInFilterBase(item, filter);
		}

		public static bool IsInFilter(DataAccess.Organization item, OrganizationFilter filter)
		{
			return IsInFilterBase(item, filter);
		}

		static bool IsInFilterBase(DataAccess.IDatabaseElement item, FilterBase filter)
		{
			if (!filter.WithDeleted && item.IsDeleted.GetValueOrDefault(false))
				return false;
			bool isInUids = IsInUidList(item.Uid, filter.Uids);
			bool isInRemovalDates = IsInDateTimePeriod(item.RemovalDate, filter.RemovalDates);
			return isInUids && isInRemovalDates; 
		}

		static bool IsInOrganizationFilterBase(DataAccess.IOrganizationDatabaseElement item, OrganizationFilterBase filter)
		{
			bool isInOrganizations = IsInUidList(item.OrganizationUid, filter.OrganizationUids);
			return IsInFilterBase(item, filter) && isInOrganizations;
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