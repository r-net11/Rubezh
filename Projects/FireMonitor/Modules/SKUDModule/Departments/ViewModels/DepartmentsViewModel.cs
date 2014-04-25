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
		public DepartmentsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
		}

		public void Initialize(DepartmentFilter filter)
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { UIDs = FiresecManager.CurrentUser.OrganisationUIDs });
			var departments = DepartmentHelper.GetList(filter);

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
						var departmentViewModel = new DepartmentViewModel(organisation, department);
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
			OnPropertyChanged("Organisations");
			SelectedDepartment = Organisations.FirstOrDefault();
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

		public List<DepartmentViewModel> Organisations { get; private set; }
		List<DepartmentViewModel> AllDepartments { get; set; }

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

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			Guid? parentDepartmentUID = null;
			if(SelectedDepartment.Parent != null && !SelectedDepartment.Parent.IsOrganisation)
				parentDepartmentUID = SelectedDepartment.Parent.Department.UID;
			var departmentDetailsViewModel = new DepartmentDetailsViewModel(SelectedDepartment.Organisation, null, parentDepartmentUID);
			if (DialogService.ShowModalWindow(departmentDetailsViewModel))
			{
				var departmentViewModel = new DepartmentViewModel(SelectedDepartment.Organisation, departmentDetailsViewModel.ShortDepartment);

				DepartmentViewModel OrganisationViewModel = SelectedDepartment;
				if (!OrganisationViewModel.IsOrganisation)
					OrganisationViewModel = SelectedDepartment.Parent;

				if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
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
			var departmentDetailsViewModel = new DepartmentDetailsViewModel(SelectedDepartment.Organisation, SelectedDepartment.Department.UID);
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