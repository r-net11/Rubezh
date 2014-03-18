using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.TreeList;
using FiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient.SKDHelpers;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class DepartmentViewModel : TreeNodeViewModel<DepartmentViewModel>
	{
		OrganisationDepartmentsViewModel OrganisationDepartmentsViewModel;
		public Department Department { get; private set; }

		public DepartmentViewModel(OrganisationDepartmentsViewModel organisationDepartmentsViewModel, Department department)
		{
			OrganisationDepartmentsViewModel = organisationDepartmentsViewModel;
			Department = department;

			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
		}

		public void Update()
		{
			OnPropertyChanged(()=>Department);
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var departmentDetailsViewModel = new DepartmentDetailsViewModel(OrganisationDepartmentsViewModel);
			if (DialogService.ShowModalWindow(departmentDetailsViewModel))
			{
				var department = departmentDetailsViewModel.Department;
				DepartmentHelper.LinkToParent(department, Department);
				var saveResult = DepartmentHelper.Save(department);
				if (!saveResult)
					return;
				var departmentViewModel = new DepartmentViewModel(OrganisationDepartmentsViewModel, department);
				this.Department.ChildDepartmentUIDs.Add(department.UID);
				this.AddChild(departmentViewModel);
				this.Update();
				OrganisationDepartmentsViewModel.Departments.Add(departmentViewModel);
			}
		}

		public RelayCommand AddToParentCommand { get; private set; }
		void OnAddToParent()
		{
			Parent.AddCommand.Execute();
		}
		public bool CanAddToParent()
		{
			return Parent != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var parent = Parent;
			if (parent != null)
			{
				var removeResult = DepartmentHelper.MarkDeleted(Department);
				if (!removeResult)
					return;
				
				var index = ZonesViewModel.Current.SelectedZone.VisualIndex;
				parent.Nodes.Remove(this);
				parent.Update();

				index = Math.Min(index, parent.ChildrenCount - 1);
				OrganisationDepartmentsViewModel.Departments.Remove(this);
				var children = OrganisationDepartmentsViewModel.GetAllChildrenModels(this);
				OrganisationDepartmentsViewModel.SelectedDepartment = index >= 0 ? parent.GetChildByVisualIndex(index) : parent;
			}
		}
		bool CanRemove()
		{
			return Parent != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var departmentDetailsViewModel = new DepartmentDetailsViewModel(OrganisationDepartmentsViewModel, this.Department);
			if (DialogService.ShowModalWindow(departmentDetailsViewModel))
			{
				var department = departmentDetailsViewModel.Department;
				var saveResult = DepartmentHelper.Save(department);
				if (!saveResult)
					return;
				this.Department = department;
				Update();
			}
		}
		public bool CanEdit()
		{
			return true;
		}
	}
}