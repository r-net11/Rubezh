using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using System;
using Infrastructure.Common.CheckBoxList;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class EmployeeFilterViewModel : OrganizationFilterBaseViewModel<EmployeeFilter>
	{
		public EmployeeFilterViewModel(EmployeeFilter filter): base(filter)
		{
			ResetCommand = new RelayCommand(OnReset);
		}

		protected override void Initialize()
		{
			base.Initialize();
			
			Departments = new List<FilterDepartmentViewModel>();
			var departments = DepartmentHelper.Get(null);
			if (departments != null)
			{
				foreach (var department in departments)
				{
					Departments.Add(new FilterDepartmentViewModel(department, this));
				}
				RootDepartment = Departments.FirstOrDefault(x => x.Department.ParentDepartmentUID == null);
				SetChildren(RootDepartment);
			}
			
			Positions = new CheckBoxItemList<FilterPositionViewModel>();
			var positions = PositionHelper.Get(null);
			if (positions != null)
				foreach (var position in positions)
					Positions.Add(new FilterPositionViewModel(position));
		}

		protected override void Update()
		{
			base.Update();
			Departments.ForEach(x => x.IsChecked = false);
			Positions.Items.ForEach(x => x.IsChecked = false);
			Departments.ForEach(x =>
			{
				if (Filter.DepartmentUIDs.Any(y => y == x.Department.UID))
					x.IsChecked = true;
			});
			Positions.Items.ForEach(x =>
			{
				if (Filter.PositionUIDs.Any(y => y == x.Position.UID))
					x.IsChecked = true;
			});
		}

		List<FilterDepartmentViewModel> GetAllChildren(FilterDepartmentViewModel department)
		{
			var result = new List<FilterDepartmentViewModel>();
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

		void SetChildren(FilterDepartmentViewModel department)
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

		public void SetChildDepartmentsChecked(FilterDepartmentViewModel department)
		{
			GetAllChildren(department).ForEach(x => x.IsChecked = true);
		}

		
		List<FilterDepartmentViewModel> departments;
		public List<FilterDepartmentViewModel> Departments
		{
			get { return departments; }
			private set
			{
				departments = value;
				OnPropertyChanged(()=>Departments);
			}
		}

		FilterDepartmentViewModel rootDepartment;
		public FilterDepartmentViewModel RootDepartment
		{
			get { return rootDepartment; }
			private set
			{
				rootDepartment = value;
				OnPropertyChanged(() => RootDepartment);
				OnPropertyChanged(() => RootDepartments);
			}
		}

		public FilterDepartmentViewModel[] RootDepartments
		{
			get { return new FilterDepartmentViewModel[] { RootDepartment }; }
		}


		public CheckBoxItemList<FilterPositionViewModel> Positions { get; private set; }

		protected override bool Save()
		{
			base.Save();
			Filter.PositionUIDs = new List<Guid>();
			foreach (var position in Positions.Items)
			{
				if (position.IsChecked)
					Filter.PositionUIDs.Add(position.Position.UID);
			};
			Filter.DepartmentUIDs = new List<Guid>();
			foreach (var Department in Departments)
			{
				if (Department.IsChecked)
					Filter.DepartmentUIDs.Add(Department.Department.UID);
			};
			return true;
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			Filter = new EmployeeFilter();
			Update();
		}
	}

	public class FilterDepartmentViewModel : TreeNodeViewModel<FilterDepartmentViewModel>
	{
		public FilterDepartmentViewModel(Department department, EmployeeFilterViewModel employeeFilterViewModel)
		{
			Department = department;
			DepartmentCheckedCommand = new RelayCommand(OnDepartmentChecked);
			EmployeeFilterViewModel = employeeFilterViewModel;
		}

		public Department Department { get; private set; }
		EmployeeFilterViewModel EmployeeFilterViewModel;

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}

		public RelayCommand DepartmentCheckedCommand { get; private set; }
		void OnDepartmentChecked()
		{
			if (IsChecked)
				EmployeeFilterViewModel.SetChildDepartmentsChecked(this);
		}
	}

	public class FilterPositionViewModel : CheckBoxItemViewModel
	{
		public FilterPositionViewModel(Position position)
		{
			Position = position;
		}

		public Position Position { get; private set; }
	}
}