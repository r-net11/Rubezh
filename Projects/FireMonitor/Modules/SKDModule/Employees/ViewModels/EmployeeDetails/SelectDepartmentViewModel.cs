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
		HRViewModel _hrViewModel;

		public SelectDepartmentViewModel(Employee employee, ShortDepartment department, HRViewModel hrViewModel)
		{
			Title = "Выбор отдела";
			Employee = employee;
			_hrViewModel = hrViewModel;
			AddCommand = new RelayCommand(OnAdd);
			Departments = new List<SelectationDepartmentViewModel>();
			var departments = DepartmentHelper.GetByOrganisation(Employee.OrganisationUID);
			var organisation = OrganisationHelper.GetSingle(employee.OrganisationUID);
			if (departments == null || departments.Count() == 0)
			{
				MessageBoxService.Show("Для данной организации не указано не одного отдела");
				return;
			}
			foreach (var item in departments)
			{
				Departments.Add(new SelectationDepartmentViewModel(item, this));
			}
			_organisation = new SelectationDepartmentViewModel(organisation, this);
			OnPropertyChanged(() => RootDepartments);
			var rootDepartments = Departments.Where(x => x.Department.ParentDepartmentUID == null).ToArray();
			if (rootDepartments.IsNotNullOrEmpty())
			{
				foreach (var rootDepartment in rootDepartments)
				{	
					_organisation.AddChild(rootDepartment);
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

		SelectationDepartmentViewModel _organisation;
		
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
		
		public SelectationDepartmentViewModel[] RootDepartments
		{
			get { return new SelectationDepartmentViewModel[] { _organisation }; }
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
			var hasParentDepartment = HighlightedDepartment != null && HighlightedDepartment.IsDepartment;
			if (hasParentDepartment)
				parentDepartmentUID = HighlightedDepartment.Department.UID;
			var departmentDetailsViewModel = new DepartmentDetailsViewModel(Employee.OrganisationUID, null, parentDepartmentUID);
			if (DialogService.ShowModalWindow(departmentDetailsViewModel))
			{
				var departmentViewModel = new SelectationDepartmentViewModel(departmentDetailsViewModel.ShortDepartment, this);
				Departments.Add(departmentViewModel);
				if (hasParentDepartment)
				{
					HighlightedDepartment.AddChild(departmentViewModel);
					departmentViewModel.ExpandToThis();
					departmentViewModel.SelectCommand.Execute();
				}
				else
				{
					_organisation.AddChild(departmentViewModel);
					departmentViewModel.SelectCommand.Execute();
				}
				_hrViewModel.UpdateDepartments();
			}
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
