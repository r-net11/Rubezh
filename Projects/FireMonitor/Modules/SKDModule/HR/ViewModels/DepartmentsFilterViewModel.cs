using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DepartmentsFilterViewModel : BaseViewModel
	{
		public DepartmentsFilterViewModel(EmployeeFilter filter)
		{
			var organisations = OrganisationHelper.GetByCurrentUser();
			var departments = DepartmentHelper.GetByCurrentUser();

			AllDepartments = new List<DepartmentFilterItemViewModel>();
			Organisations = new List<DepartmentFilterItemViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new DepartmentFilterItemViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllDepartments.Add(organisationViewModel);
				foreach (var department in departments)
				{
					if (department.OrganisationUID == organisation.UID)
					{
						var departmentViewModel = new DepartmentFilterItemViewModel(department);
						departmentViewModel.IsChecked = filter.DepartmentUIDs.Contains(department.UID);
						organisationViewModel.AddChild(departmentViewModel);
						AllDepartments.Add(departmentViewModel);
					}
				}
				foreach (var departmentViewModel in AllDepartments)
				{
					if (departmentViewModel.Department != null && departmentViewModel.Department.ParentDepartmentUID == null)
						AddChildren(departmentViewModel);
				}
			}
		}

		void AddChildren(DepartmentFilterItemViewModel departmentViewModel)
		{
			if (departmentViewModel.Department.ChildDepartmentUIDs != null && departmentViewModel.Department.ChildDepartmentUIDs.Count > 0)
			{
				var children = AllDepartments.Where(x => departmentViewModel.Department.ChildDepartmentUIDs.Any(y => x.Department != null && y == x.Department.UID));
				foreach (var child in children)
				{
					departmentViewModel.AddChild(child);
					AddChildren(child);
				}
			}
		}

		public List<DepartmentFilterItemViewModel> AllDepartments;
		public List<DepartmentFilterItemViewModel> Organisations { get; private set; }

		public DepartmentFilterItemViewModel[] RootDepartments
		{
			get { return Organisations.ToArray(); }
		}

		public List<Guid> UIDs
		{
			get
			{
				var result = new List<Guid>();
				foreach (var department in AllDepartments)
				{
					if (department.IsChecked && !department.IsOrganisation)
					{
						result.Add(department.UID);
					}
				}
				return result;
			}
		}
	}
}