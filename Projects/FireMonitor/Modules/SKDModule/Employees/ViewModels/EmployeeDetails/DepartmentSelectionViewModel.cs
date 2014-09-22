using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DepartmentSelectionViewModel : SaveCancelDialogViewModel
	{
		Guid OrganisationUID;
		HRViewModel HRViewModel;

		public DepartmentSelectionViewModel(Employee employee, ShortDepartment shortDepartment, HRViewModel hrViewModel)
		{
			Title = "Выбор отдела";
			OrganisationUID = employee.OrganisationUID;
			HRViewModel = hrViewModel;
			AddCommand = new RelayCommand(OnAdd);

			AllDepartments = new List<DepartmentSelectionItemViewModel>();
			var departments = DepartmentHelper.GetByOrganisation(OrganisationUID);
			if (departments != null)
			{
				foreach (var department in departments)
				{
					AllDepartments.Add(new DepartmentSelectionItemViewModel(department));
				}
			}

			RootDepartments = new ObservableCollection<DepartmentSelectionItemViewModel>();
			foreach (var department in AllDepartments)
			{
				if (department.Department.ParentDepartmentUID == null)
				{
					RootDepartments.Add(department);
					SetChildren(department);
				}
			}

			if (shortDepartment != null)
			{
				SelectedDepartment = AllDepartments.FirstOrDefault(x => x.Department.UID == shortDepartment.UID);
				if (SelectedDepartment != null)
				{
					SelectedDepartment.ExpandToThis();
				}
			}
		}

		void SetChildren(DepartmentSelectionItemViewModel department)
		{
			var children = AllDepartments.Where(x => department.Department.ChildDepartmentUIDs.Any(y => y == x.Department.UID));
			foreach (var child in children)
			{
				department.AddChild(child);
				SetChildren(child);
			}
		}

		List<DepartmentSelectionItemViewModel> AllDepartments { get; set; }

		public ObservableCollection<DepartmentSelectionItemViewModel> RootDepartments { get; private set; }

		DepartmentSelectionItemViewModel _selectedDepartment;
		public DepartmentSelectionItemViewModel SelectedDepartment
		{
			get { return _selectedDepartment; }
			set
			{
				_selectedDepartment = value;
				OnPropertyChanged(() => SelectedDepartment);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			Guid? parentDepartmentUID = null;
			var hasParentDepartment = SelectedDepartment != null;
			if (hasParentDepartment)
				parentDepartmentUID = SelectedDepartment.Department.UID;
			var departmentDetailsViewModel = new DepartmentDetailsViewModel();
			departmentDetailsViewModel.Initialize(OrganisationUID, parentDepartmentUID);
			if (DialogService.ShowModalWindow(departmentDetailsViewModel))
			{
				var departmentViewModel = new DepartmentSelectionItemViewModel(departmentDetailsViewModel.Model);
				if (hasParentDepartment)
				{
					SelectedDepartment.AddChild(departmentViewModel);
				}
				else
				{
					RootDepartments.Add(departmentViewModel);
					OnPropertyChanged(() => RootDepartments);
				}
				departmentViewModel.ExpandToThis();
				SelectedDepartment = departmentViewModel;
				if(HRViewModel != null)
					HRViewModel.UpdateDepartments();
			}
		}

		protected override bool Save()
		{
			return true;
		}
	}
}