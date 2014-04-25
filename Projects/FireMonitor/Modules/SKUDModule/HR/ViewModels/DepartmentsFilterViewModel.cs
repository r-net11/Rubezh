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
			var departments = DepartmentHelper.GetList(new DepartmentFilter () { OrganisationUIDs = FiresecManager.CurrentUser.OrganisationUIDs });

			AllFilterEntities = new List<FilterDepartmentViewModel>();
			Organisations = new List<FilterDepartmentViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new FilterDepartmentViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllFilterEntities.Add(organisationViewModel);
				foreach (var department in departments)
				{
					if (department.OrganisationUID == organisation.UID)
					{
						var departmentViewModel = new FilterDepartmentViewModel(department);
						departmentViewModel.IsChecked = filter.UIDs.Contains(department.UID);
						organisationViewModel.AddChild(departmentViewModel);
						AllFilterEntities.Add(departmentViewModel);
					}
				}
				foreach (var filterEntitysViewModel in AllFilterEntities)
				{
					if (filterEntitysViewModel.Department != null && filterEntitysViewModel.Department.ParentDepartmentUID == null)
						AddChildren(filterEntitysViewModel);
				}
			}
		}

		void AddChildren(FilterDepartmentViewModel filterDepartmentViewModel)
		{
			if (filterDepartmentViewModel.Department.ChildDepartmentUIDs != null && filterDepartmentViewModel.Department.ChildDepartmentUIDs.Count > 0)
			{
				var children = AllFilterEntities.Where(x => filterDepartmentViewModel.Department.ChildDepartmentUIDs.Any(y => x.Department != null && y == x.Department.UID));
				foreach (var child in children)
				{
					filterDepartmentViewModel.AddChild(child);
					AddChildren(child);
				}
			}
		}

		public List<FilterDepartmentViewModel> AllFilterEntities;

		List<FilterDepartmentViewModel> _organisations;
		public List<FilterDepartmentViewModel> Organisations
		{
			get { return _organisations; }
			private set
			{
				_organisations = value;
				OnPropertyChanged("Organisations");
			}
		}

		public FilterDepartmentViewModel[] RootFilterEntities
		{
			get { return Organisations.ToArray(); }
		}

		public DepartmentFilter Save()
		{
			var departmentFilter = new DepartmentFilter();
			foreach (var filterEntity in AllFilterEntities)
			{
				if (filterEntity.IsChecked && !filterEntity.IsOrganisation)
				{
					departmentFilter.UIDs.Add(filterEntity.UID);
				}
			}
			return departmentFilter;
		}
	}
}