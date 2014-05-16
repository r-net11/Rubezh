using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SelectDepartmentViewModel : SaveCancelDialogViewModel
	{
		Employee Employee;

		public SelectDepartmentViewModel(Employee employee)
		{
			Title = "Отдел";
			Employee = employee;
			Departments = new List<SelectationDepartmentViewModel>();
			var departments = DepartmentHelper.GetByOrganisation(Employee.OrganisationUID);
			if (departments != null)
			{
				foreach (var department in departments)
				{
					Departments.Add(new SelectationDepartmentViewModel(department, this));
				}
				RootDepartments = Departments.Where(x => x.Department.ParentDepartmentUID == null).ToArray();
				if (RootDepartments.IsNotNullOrEmpty())
				{
					foreach (var rootDepartment in RootDepartments)
					{
						SetChildren(rootDepartment);
					}
				}
			}
			var selectedDepartment = Departments.FirstOrDefault(x => x.Department.UID == Employee.DepartmentUID);
			if (selectedDepartment == null)
				selectedDepartment = Departments.FirstOrDefault();
			selectedDepartment.IsChecked = true;
			selectedDepartment.ExpandToThis();
			OnPropertyChanged(() => SelectedDepartment);
			AddCommand = new RelayCommand(OnAdd);
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

		public SelectationDepartmentViewModel SelectedDepartment
		{
			get { return Departments.FirstOrDefault(x => x.IsChecked); }
		}

		SelectationDepartmentViewModel _selectedDepartment2;
		public SelectationDepartmentViewModel SelectedDepartment2
		{
			get { return _selectedDepartment2; }
			set
			{
				_selectedDepartment2 = value;
				OnPropertyChanged(() => SelectedDepartment2);
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

		SelectationDepartmentViewModel[] rootDepartments;
		public SelectationDepartmentViewModel[] RootDepartments
		{
			get { return rootDepartments; }
			set
			{
				rootDepartments = value;
				OnPropertyChanged(() => RootDepartments);
			}
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
			if (SelectedDepartment2.Parent != null)
				parentDepartmentUID = SelectedDepartment2.Parent.Department.UID;
			var departmentDetailsViewModel = new DepartmentDetailsViewModel(Employee.OrganisationUID, null, parentDepartmentUID);
			if (DialogService.ShowModalWindow(departmentDetailsViewModel))
			{
				var departmentViewModel = new SelectationDepartmentViewModel(departmentDetailsViewModel.ShortDepartment, this);
				SelectedDepartment2.AddChild(departmentViewModel);
				Departments.Add(departmentViewModel);
				departmentViewModel.SelectCommand.Execute();
				SelectedDepartment2.ExpandToThis();
			}
		}
		bool CanAdd()
		{
			return SelectedDepartment2 != null;
		}

		protected override bool Save()
		{
			if(SelectedDepartment == null)
			{
				MessageBoxService.ShowWarning("Выборите отдел");
				return false;
			}
			return true;
		}
	}
}
