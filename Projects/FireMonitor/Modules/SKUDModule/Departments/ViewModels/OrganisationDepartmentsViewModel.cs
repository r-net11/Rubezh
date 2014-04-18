using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationDepartmentsViewModel : BaseViewModel
	{
		public Organisation Organization { get; private set; }

		public void Initialize(Organisation organization, IEnumerable<ShortDepartment> departments)
		{
			Organization = organization;

			Departments = new ObservableCollection<DepartmentViewModel>();
			if (departments != null)
				foreach (var department in departments)
				{
					var departmentViewModel = new DepartmentViewModel(this, department);
					Departments.Add(departmentViewModel);
				}
			RootDepartments = Departments.Where(x => x.Department.ParentDepartmentUID == null).ToArray();
			if (RootDepartments.IsNotNullOrEmpty())
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
			foreach (var root in RootDepartments)
			{
				AddChildren(root);
			}
		}

		void AddChildren(DepartmentViewModel departmentViewModel)
		{
			if (departmentViewModel.Department.ChildDepartmentUIDs != null && departmentViewModel.Department.ChildDepartmentUIDs.Count > 0)
			{
				var children = Departments.Where(x => departmentViewModel.Department.ChildDepartmentUIDs.Any(y => y == x.Department.UID));
				foreach (var child in children)
				{
					departmentViewModel.AddChild(child);
					AddChildren(child);
				}
			}
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

		DepartmentViewModel[] rootDepartments;
		public DepartmentViewModel[] RootDepartments
		{
			get { return rootDepartments; }
			set
			{
				rootDepartments = value;
				OnPropertyChanged(() => RootDepartments);
			}
		}
		#endregion
	}
}