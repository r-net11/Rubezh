using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SelectDepartmentViewModel : SaveCancelDialogViewModel
	{
		Employee Employee;
		public SelectationDepartmentViewModel SelectedDepartment { get; private set; }

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
					Departments.Add(new SelectationDepartmentViewModel(department));
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
			SelectedDepartment = Departments.FirstOrDefault(x => x.Department.UID == Employee.DepartmentUID);
			if (SelectedDepartment != null)
			{
				SelectedDepartment.IsChecked = true;
				SelectedDepartment.ExpandToThis();
			}
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

		protected override bool Save()
		{
			SelectedDepartment = Departments.FirstOrDefault(x => x.IsChecked);
			if(SelectedDepartment == null)
			{
				MessageBoxService.ShowWarning("Выборите отдел");
				return false;
			}
			return true;
		}
	}
}
