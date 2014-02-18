using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using System.Linq;
using System;
using System.Windows.Documents;
using System.Collections.Generic;
using Infrastructure.Common;

namespace SKDModule.ViewModels
{
	public class DepartmentsViewModel : ViewPartViewModel
	{
		public static DepartmentsViewModel Current { get; private set; }

		public DepartmentsViewModel()
		{
			Current = this;
			Departments = new ObservableCollection<DepartmentViewModel>();
			//var departments = FiresecManager.GetDepartments(null);
			//foreach (var department in departments)
			//{
			//    var departmentViewModel = new DepartmentViewModel(department);
			//    Departments.Add(departmentViewModel);
			//}

			Filter = new DepartmentFilter();
			//Update();
			WithDeletedCommand = new RelayCommand(OnWithDeleted);
		}

		public RelayCommand WithDeletedCommand { get; private set; }
		void OnWithDeleted()
		{
			Filter.WithDeleted = DeletedType.All;
			Update();
		}

		DepartmentFilter Filter;

		void Update()
		{
			Departments = new ObservableCollection<DepartmentViewModel>();
			RootDepartment = Departments.FirstOrDefault(x => x.Department.ParentDepartmentUid == null);
			if (RootDepartment != null)
			{
				BuildTree();
			}
		}

		ObservableCollection<DepartmentViewModel> _departments;
		public ObservableCollection<DepartmentViewModel> Departments
		{
			get { return _departments; }
			set
			{
				_departments = value;
				OnPropertyChanged("Departments");
			}
		}

		DepartmentViewModel _selectedDepartment;
		public DepartmentViewModel SelectedDepartment
		{
			get { return _selectedDepartment; }
			set
			{
				_selectedDepartment = value;
				OnPropertyChanged("SelectedDepartment");
			}
		}

		#region Tree
		void BuildTree()
		{
			AddChildren(RootDepartment);
		}

		void AddChildren(DepartmentViewModel departmentViewModel)
		{
			if (departmentViewModel.Department.ChildDepartmentUids.Count > 0)
			{
				var children = Departments.Where(x => departmentViewModel.Department.ChildDepartmentUids.Any(y => y == x.Department.UID));
				foreach (var child in children)
				{
					departmentViewModel.AddChild(child);
					AddChildren(child);
				}
			}
		}

		public List<Department> GetAllChildrenModels(DepartmentViewModel departmentViewModel)
		{
			var result = new List<Department>();
			if(departmentViewModel.ChildrenCount == 0)
				return result;
			foreach (var child in departmentViewModel.Children)
			{
				result.Add(child.Department);
				GetAllChildrenModels(child);
			}
			return (result);
		}

		DepartmentViewModel rootDepartment;
		public DepartmentViewModel RootDepartment
		{
			get { return rootDepartment; }
			private set
			{
				rootDepartment = value;
				OnPropertyChanged(() => RootDepartment);
				OnPropertyChanged(() => RootDepartments);
			}
		}

		public DepartmentViewModel[] RootDepartments
		{
			get { return new DepartmentViewModel[] { RootDepartment }; }
		}
		#endregion
	}
}