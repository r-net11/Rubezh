using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		#region Get
		public static IEnumerable<Employee> GetEmployees(EmployeeFilter filter)
		{
			return FiresecService.GetEmployees(filter);
		}
		public static IEnumerable<Department> GetDepartments(DepartmentFilter filter)
		{
			return FiresecService.GetDepartments(filter);
		}
		public static IEnumerable<SKDJournalItem> GetSKDJournalItems(SKDJournalFilter filter)
		{
			return FiresecService.GetSKDJournalItems(filter);
		}
		public static IEnumerable<Frame> GetFrames(FrameFilter filter)
		{
			return FiresecService.GetFrames(filter);
		}
		public static IEnumerable<Organization> GetOrganizations(OrganizationFilter filter)
		{
			return FiresecService.GetOrganizations(filter);
		}
		#endregion

		#region Save
		public static void SaveEmployees(IEnumerable<Employee> Employees)
		{
			FiresecService.SaveEmployees(Employees);
		}
		public static void SaveDepartments(IEnumerable<Department> Departments)
		{
			FiresecService.SaveDepartments(Departments);
		}
		public static void SaveJournalItems(IEnumerable<SKDJournalItem> journalItems)
		{
			FiresecService.SaveSKDJournalItems(journalItems);
		}
		public static void SaveFrames(IEnumerable<Frame> frames)
		{
			FiresecService.SaveFrames(frames);
		}
		public static void SaveOrganizations(IEnumerable<Organization> items)
		{
			FiresecService.SaveOrganizations(items);
		}
		#endregion

		#region MarkDeleted
		public static void MarkDeletedEmployees(IEnumerable<Employee> Employees)
		{
			FiresecService.MarkDeletedEmployees(Employees);
		}
		public static void MarkDeletedDepartments(IEnumerable<Department> Departments)
		{
			FiresecService.MarkDeletedDepartments(Departments);
		}
		public static void MarkDeletedJournalItems(IEnumerable<SKDJournalItem> journalItems)
		{
			FiresecService.MarkDeletedSKDJournalItems(journalItems);
		}
		public static void MarkDeletedFrames(IEnumerable<Frame> frames)
		{
			FiresecService.MarkDeletedFrames(frames);
		}
		public static void MarkDeletedOrganizations(IEnumerable<Organization> items)
		{
			FiresecService.MarkDeletedOrganizations(items);
		}
		#endregion

		#region Get(Uid)
		public static Employee GetEmployee(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new EmployeeFilter();
			filter.Uids.Add((Guid)uid);
			return FiresecService.GetEmployees(filter).ToList().FirstOrDefault();
		}
		public static Department GetDepartment(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new DepartmentFilter();
			filter.Uids.Add((Guid)uid);
			return FiresecService.GetDepartments(filter).ToList().FirstOrDefault();
		}
		public static SKDJournalItem GetJournalItem(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new SKDJournalFilter();
			filter.Uids.Add((Guid)uid);
			return FiresecService.GetSKDJournalItems(filter).ToList().FirstOrDefault();
		}
		public static Frame GetFrame(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new FrameFilter();
			filter.Uids.Add((Guid)uid);
			return FiresecService.GetFrames(filter).ToList().FirstOrDefault();
		}
		public static Organization GetOrganization(Guid? uid)
		{
			if (uid == null)
				return null;
			var filter = new OrganizationFilter();
			filter.Uids.Add((Guid)uid);
			return FiresecService.GetOrganizations(filter).ToList().FirstOrDefault();
		}
		#endregion 

	}
}