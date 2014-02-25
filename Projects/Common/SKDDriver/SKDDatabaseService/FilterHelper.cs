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
			bool isInDepartments = IsInList<Guid>(item.DepartmentUid, filter.DepartmentUids);
			bool isInPositions = IsInList<Guid>(item.PositionUid, filter.PositionUids);
			bool isInAppointed = IsInDateTimePeriod(item.Appointed, filter.Appointed);
			bool isInDismissed = IsInDateTimePeriod(item.Dismissed, filter.Dismissed);
			return isInDepartments && isInPositions && isInAppointed && isInDepartments && IsInOrganizationFilterBase(item, filter);
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
			bool IsInCameras = IsInList<Guid>(item.CameraUid, filter.CameraUid);
			bool IsInJournalItems = IsInList<Guid>(item.JournalItemUid, filter.JournalItemUid);
			bool IsInDateTime = IsInDateTimePeriod(item.DateTime, filter.DateTime);
			return IsInCameras && IsInJournalItems && IsInDateTime && IsInFilterBase(item, filter);
		}

		public static bool IsInFilter(DataAccess.Card item, CardFilter filter)
		{
			bool isInEmployees = IsInList<Guid>(item.EmployeeUid, filter.EmployeeUids);
			return isInEmployees && IsInFilterBase(item, filter);
		}

		public static bool IsInFilter(DataAccess.CardZoneLink item, CardZoneFilter filter)
		{
			bool isInCardUids = IsInList<Guid>(item.CardUid, filter.CardUids);
			bool isInZoneUids = IsInList<Guid>(item.ZoneUid, filter.ZoneUids);
			bool isInIntervalUids = IsInList<Guid>(item.IntervalUid, filter.IntervalUids);
			return isInCardUids && isInZoneUids && isInIntervalUids && IsInFilterBase(item, filter);
		}

		public static bool IsInFilter(DataAccess.Organization item, OrganizationFilter filter)
		{
			return IsInFilterBase(item, filter);
		}

		public static bool IsInFilter(DataAccess.Document item, DocumentFilter filter)
		{
			bool isInNos = IsInList<int>(item.No, filter.Nos);
			bool isInNames = IsInList<string>(item.Name, filter.Names);
			bool isInIsuueDates = IsInDateTimePeriod(item.IssueDate, filter.IssueDate);
			bool isInLaunchDates = IsInDateTimePeriod(item.LaunchDate, filter.LaunchDate);
			return isInNos && isInNames && isInIsuueDates && isInLaunchDates && IsInOrganizationFilterBase(item, filter);
		}

		static bool IsInFilterBase(DataAccess.IDatabaseElement item, FilterBase filter)
		{
			bool isInDeleted = IsInDeleted(item, filter);
			bool isInUids = IsInList<Guid>(item.Uid, filter.Uids);
			bool isInRemovalDates = true;
			if(filter.WithDeleted == DeletedType.Deleted)
				isInRemovalDates = IsInDateTimePeriod(item.RemovalDate, filter.RemovalDates);
			return isInDeleted && isInUids && isInRemovalDates; 
		}

		static bool IsInDeleted(DataAccess.IDatabaseElement item, FilterBase filter)
		{
			bool isDeleted = item.IsDeleted;
			switch (filter.WithDeleted)
			{
				case DeletedType.Deleted:
					return isDeleted;
				case DeletedType.Not:
					return !isDeleted;
				default:
					return true;
			}
		}

		static bool IsInOrganizationFilterBase(DataAccess.IOrganizationDatabaseElement item, OrganizationFilterBase filter)
		{
			bool isInOrganizations = IsInList<Guid>(item.OrganizationUid, filter.OrganizationUids);
			return IsInFilterBase(item, filter) && isInOrganizations;
		}

		static bool IsInDateTimePeriod(DateTime? dateTime, DateTimePeriod dateTimePeriod)
		{
			if (dateTimePeriod == null)
				return true;
			if (dateTime == null)
				return true;
			return dateTime >= dateTimePeriod.StartDate && dateTime <= dateTimePeriod.EndDate;
		}

		static bool IsInList<T>(T? item, List<T> list) 
			where T:struct
		{
			if (list == null || list.Count == 0)
				return true;
			return list.Any(x => x.Equals(item));
		}

		static bool IsInList<T>(T item, List<T> list)
		{
			if (list == null || list.Count == 0)
				return true;
			return list.Any(x => x.Equals(item));
		}
	}
}