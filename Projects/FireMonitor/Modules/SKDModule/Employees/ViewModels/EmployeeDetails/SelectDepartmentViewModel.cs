using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SelectDepartmentViewModel : SaveCancelDialogViewModel
	{
		Employee Employee;
		HRViewModel HrViewModel;

		public SelectDepartmentViewModel(Employee employee, ShortDepartment department, HRViewModel hrViewModel)
		{
			Title = "Выбор отдела";
			Employee = employee;
			HrViewModel = hrViewModel;
			AddCommand = new RelayCommand(OnAdd);

			Departments = new List<SelectationDepartmentViewModel>();
			var departments = DepartmentHelper.GetByOrganisation(Employee.OrganisationUID);
			if (departments != null)
			{
				foreach (var item in departments)
				{
					Departments.Add(new SelectationDepartmentViewModel(item, this));
				}
			}

			RootDepartments = Departments.Where(x => x.Department.ParentDepartmentUID == null).ToList();
			if (RootDepartments.IsNotNullOrEmpty())
			{
				foreach (var rootDepartment in RootDepartments)
				{
					SetChildren(rootDepartment);
				}
			}
			SelectationDepartmentViewModel selectedDepartment;
			if (department == null)
				selectedDepartment = Departments.FirstOrDefault();
			else
			{
				selectedDepartment = Departments.FirstOrDefault(x => x.Department.UID == department.UID);
				if (selectedDepartment == null)
					selectedDepartment = Departments.FirstOrDefault();
			}
			if (selectedDepartment != null)
			{
				selectedDepartment.IsChecked = true;
				selectedDepartment.ExpandToThis();
			}
			SelectedDepartment = selectedDepartment;
			OnPropertyChanged(() => SelectedDepartment);
		}

		void SetChildren(SelectationDepartmentViewModel department)
		{
			if (department.Department.ChildDepartmentUIDs.Count == 0)
				return;
			var children = Departments.Where(x => department.Department.ChildDepartmentUIDs.Any(y => y == x.Department.UID));
			foreach (var child in children)
			{
				department.AddChild(child);
				SetChildren(child);
			}
		}

		SelectationDepartmentViewModel _selectedDepartment;
		public SelectationDepartmentViewModel SelectedDepartment
		{
			get { return _selectedDepartment; }
			set
			{
				_selectedDepartment = value;
				OnPropertyChanged(() => SelectedDepartment);
			}
		}

		List<SelectationDepartmentViewModel> departments;
		public List<SelectationDepartmentViewModel> Departments
		{
			get { return departments; }
			private set
			{
				departments = value;
				OnPropertyChanged(() => Departments);
			}
		}

		List<SelectationDepartmentViewModel> rootDepartments;
		public List<SelectationDepartmentViewModel> RootDepartments
		{
			get { return rootDepartments; }
			private set
			{
				rootDepartments = value;
				OnPropertyChanged(() => RootDepartments);
				OnPropertyChanged(() => RootDepartmentsArray);
			}
		}

		public SelectationDepartmentViewModel[] RootDepartmentsArray
		{
			get { return RootDepartments.ToArray(); }
		}

		List<SelectationDepartmentViewModel> GetAllChildren(SelectationDepartmentViewModel department)
		{
			var result = new List<SelectationDepartmentViewModel>();
			if (department.Department.ChildDepartmentUIDs.Count == 0)
				return result;
			Departments.ForEach(x =>
			{
				if (department.Department.ChildDepartmentUIDs.Contains(x.Department.UID))
				{
					result.Add(x);
					result.AddRange(GetAllChildren(x));
				}
			});
			return result;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			Guid? parentDepartmentUID = null;
			var hasParentDepartment = SelectedDepartment != null && SelectedDepartment.IsDepartment;
			if (hasParentDepartment)
				parentDepartmentUID = SelectedDepartment.Department.UID;
			var departmentDetailsViewModel = new DepartmentDetailsViewModel();
            departmentDetailsViewModel.Initialize(Employee.OrganisationUID ,parentDepartmentUID);
			if (DialogService.ShowModalWindow(departmentDetailsViewModel))
			{
				var departmentViewModel = new SelectationDepartmentViewModel(departmentDetailsViewModel.Model, this);
				Departments.Add(departmentViewModel);
				if (hasParentDepartment)
				{
					SelectedDepartment.AddChild(departmentViewModel);
				}
				else
				{
					RootDepartments.Add(departmentViewModel);
				}
				departmentViewModel.ExpandToThis();
				departmentViewModel.SelectCommand.Execute();
				HrViewModel.UpdateDepartments();
			}
		}

		protected override bool Save()
		{
			if (SelectedDepartment == null)
			{
				MessageBoxService.ShowWarning("Выборите отдел");
				return false;
			}
			return true;
		}
	}
}