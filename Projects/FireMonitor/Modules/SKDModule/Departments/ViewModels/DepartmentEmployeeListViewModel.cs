using System;
using System.Collections.Generic;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class DepartmentEmployeeListViewModel : EmployeeListBaseViewModel
	{
		public DepartmentEmployeeListViewModel(Guid parentUID, Guid organisationUID, HRViewModel hrViewModel) : base(parentUID, organisationUID, hrViewModel) { }

		protected override bool AddToParent(Guid uid)
		{
			return EmployeeHelper.SetDepartment(uid, _parentUID);
		}

		protected override bool RemoveFromParent(Guid uid)
		{
			return EmployeeHelper.SetDepartment(uid, Guid.Empty);
		}

		public override bool CanEditDepartment { get { return false; } }
		public override bool CanEditPosition { get { return true; } }

		protected override EmployeeFilter Filter
		{
			get { return new EmployeeFilter { DepartmentUIDs = new List<Guid> { _parentUID } }; }
		}

		protected override EmployeeFilter EmptyFilter
		{
			get { return new EmployeeFilter { DepartmentUIDs = new List<Guid> { Guid.Empty } }; }
		}

		protected override Guid GetParentUID(Employee employee)
		{
			return employee.Department != null ? employee.Department.UID : Guid.Empty;
		}
	}
}
