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

		public SelectDepartmentViewModel(Employee employee)
		{
			Title = "Отдел";
			Employee = employee;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			Departments = new List<SelectationDepartmentViewModel>();
			var departments = DepartmentHelper.GetByOrganisation(Employee.OrganisationUID);
			if (departments == null || departments.Count() == 0)
			{
				MessageBoxService.Show("Для данной организации не указано не одного отдела");
				return;
			}
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
			SelectationDepartmentViewModel selectedDepartment;
			if (employee.Department == null)
				selectedDepartment = Departments.FirstOrDefault();
			else
			{
				selectedDepartment = Departments.FirstOrDefault(x => x.Department.UID == Employee.Department.UID);
				if (selectedDepartment == null)
					selectedDepartment = Departments.FirstOrDefault();
			if (selectedDepartment != null)
			{
				selectedDepartment.IsChecked = true;
				selectedDepartment.ExpandToThis();
			}
			HighlightedDepartment = selectedDepartment;
			OnPropertyChanged(() => SelectedDepartment);
			OnPropertyChanged(() => HighlightedDepartment);
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

		SelectationDepartmentViewModel _highlightedDepartment;
		public SelectationDepartmentViewModel HighlightedDepartment
		{
			get { return _highlightedDepartment; }
			set
			{
				_highlightedDepartment = value;
				OnPropertyChanged(() => HighlightedDepartment);
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
			if (HighlightedDepartment.Parent != null)
				parentDepartmentUID = HighlightedDepartment.Parent.Department.UID;
			var departmentDetailsViewModel = new DepartmentDetailsViewModel(Employee.OrganisationUID, null, parentDepartmentUID);
			if (DialogService.ShowModalWindow(departmentDetailsViewModel))
			{
				var departmentViewModel = new SelectationDepartmentViewModel(departmentDetailsViewModel.ShortDepartment, this);
				HighlightedDepartment.AddChild(departmentViewModel);
				Departments.Add(departmentViewModel);
				departmentViewModel.SelectCommand.Execute();
				HighlightedDepartment.ExpandToThis();
			}
		}
		bool CanAdd()
		{
			return HighlightedDepartment != null;
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
