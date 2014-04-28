using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class DepartmentsFilterViewModel : BaseViewModel
	{
		public DepartmentsFilterViewModel(DepartmentFilter filter)
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { UIDs = FiresecManager.CurrentUser.OrganisationUIDs });
			var departments = DepartmentHelper.Get(new DepartmentFilter() { OrganisationUIDs = FiresecManager.CurrentUser.OrganisationUIDs });

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
						departmentViewModel.IsChecked = filter.UIDs.Contains(department.UID);
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

		public DepartmentFilter Save()
		{
			var departmentFilter = new DepartmentFilter();
			foreach (var department in AllDepartments)
			{
				if (department.IsChecked && !department.IsOrganisation)
				{
					departmentFilter.UIDs.Add(department.UID);
				}
			}
			return departmentFilter;
		}
	}
}