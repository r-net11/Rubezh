using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System;

namespace SKDModule.ViewModels
{
	public class DepartmentsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		DepartmentFilter Filter;

		public DepartmentsViewModel()
		{
			EditFilterCommand = new RelayCommand(OnEditFilter);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Filter = new DepartmentFilter() { OrganizationUIDs = FiresecManager.CurrentUser.OrganisationUIDs };
			Initialize();
		}

		public void Initialize()
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
			var departments = DepartmentHelper.GetList(Filter);

			AllDepartments = new List<DepartmentViewModel>();
			Organisations = new List<DepartmentViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new DepartmentViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllDepartments.Add(organisationViewModel);
				foreach (var department in departments)
				{
					if (department.OrganisationUID == organisation.UID)
					{
						var departmentViewModel = new DepartmentViewModel(department);
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

			foreach (var organisation in Organisations)
			{
				organisation.ExpandToThis();
			}
			OnPropertyChanged("RootDepartments");
		}

		void AddChildren(DepartmentViewModel departmentViewModel)
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

		#region DepartmentSelection
		public List<DepartmentViewModel> AllDepartments;

		public void Select(Guid departmentUID)
		{
			if (departmentUID != Guid.Empty)
			{
				var departmentViewModel = AllDepartments.FirstOrDefault(x => x.Department != null && x.Department.UID == departmentUID);
				if (departmentViewModel != null)
					departmentViewModel.ExpandToThis();
				SelectedDepartment = departmentViewModel;
			}
		}
		#endregion

		DepartmentViewModel _selectedDepartment;
		public DepartmentViewModel SelectedDepartment
		{
			get { return _selectedDepartment; }
			set
			{
				_selectedDepartment = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged("SelectedDepartment");
			}
		}

		List<DepartmentViewModel> _organisations;
		public List<DepartmentViewModel> Organisations
		{
			get { return _organisations; }
			private set
			{
				_organisations = value;
				OnPropertyChanged("Organisations");
			}
		}

		public Organisation Organisation
		{
			get
			{
				DepartmentViewModel OrganisationViewModel = SelectedDepartment;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedDepartment.GetAllParents().FirstOrDefault(x=>x.IsOrganisation);

				if (OrganisationViewModel != null)
					return OrganisationViewModel.Organization;

				return null;
			}
		}

		public DepartmentViewModel[] RootDepartments
		{
			get { return Organisations.ToArray(); }
		}

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			var filterViewModel = new DepartmentFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				Filter = filterViewModel.Filter;
				Initialize();
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			Guid? parentDepartmentUID = null;
			if(SelectedDepartment.Parent != null && !SelectedDepartment.Parent.IsOrganisation)
				parentDepartmentUID = SelectedDepartment.Parent.Department.UID;
			var departmentDetailsViewModel = new DepartmentDetailsViewModel(this, Organisation, null, parentDepartmentUID);
			if (DialogService.ShowModalWindow(departmentDetailsViewModel))
			{
				var departmentViewModel = new DepartmentViewModel(departmentDetailsViewModel.ShortDepartment);

				DepartmentViewModel OrganisationViewModel = SelectedDepartment;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedDepartment.Parent;

				if (OrganisationViewModel == null || OrganisationViewModel.Organization == null)
					return;

				OrganisationViewModel.AddChild(departmentViewModel);
				SelectedDepartment = departmentViewModel;
			}
		}
		bool CanAdd()
		{
			return SelectedDepartment != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var parent = SelectedDepartment.Parent;
			if (parent != null)
			{
				var removeResult = DepartmentHelper.MarkDeleted(SelectedDepartment.Department.UID);
				if (!removeResult)
					return;

				var index = ZonesViewModel.Current.SelectedZone.VisualIndex;
				parent.Nodes.Remove(SelectedDepartment);
				parent.Update();

				index = Math.Min(index, parent.ChildrenCount - 1);
				AllDepartments.Remove(SelectedDepartment);
				//var children = GetAllChildrenModels(SelectedDepartment);
				//SelectedDepartment = index >= 0 ? parent.GetChildByVisualIndex(index) : parent;
			}
		}
		bool CanRemove()
		{
			return SelectedDepartment != null && !SelectedDepartment.IsOrganisation;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var departmentDetailsViewModel = new DepartmentDetailsViewModel(this, Organisation, SelectedDepartment.Department.UID);
			if (DialogService.ShowModalWindow(departmentDetailsViewModel))
			{
				SelectedDepartment.Update(departmentDetailsViewModel.ShortDepartment);
			}
		}
		bool CanEdit()
		{
			return SelectedDepartment != null && SelectedDepartment.Parent != null && !SelectedDepartment.IsOrganisation;
		}

			public List<ShortDepartment> GetAllChildrenModels(DepartmentViewModel departmentViewModel)
		{
			var result = new List<ShortDepartment>();
			if (departmentViewModel.ChildrenCount == 0)
				return result;
			foreach (var child in departmentViewModel.Children)
			{
				result.Add(child.Department);
				GetAllChildrenModels(child);
			}
			return (result);
		}
	}
}