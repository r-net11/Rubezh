using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeFilterViewModel : DialogViewModel
	{
		public EmployeeFilter Filter { get; private set; }

		public EmployeeFilterViewModel(EmployeeFilter filter)
		{
			Title = "Настройки фмльтра";

			Filter = filter;

			ClearCommand = new RelayCommand(OnClear);
			SaveCommand = new RelayCommand(OnSave);
			CancelCommand = new RelayCommand(OnCancel);

			Departments = new List<FilterDepartmentViewModel>();
			var departments = FiresecManager.GetDepartments(null).ToList();
			departments.ForEach(x => Departments.Add(new FilterDepartmentViewModel(x, this)));
			
			RootDepartment = Departments.FirstOrDefault(x => x.Department.ParentDepartmentUid == null);
			SetChildren(RootDepartment);

			Positions = new List<FilterPositionViewModel>();
			var positions = FiresecManager.GetPositions(null).ToList();
			positions.ForEach(x => Positions.Add(new FilterPositionViewModel(x)));

			//WithDeleted = filter.WithDeleted;
			
			Initialize();
		}

		void Initialize()
		{
			Departments.ForEach(x => x.IsChecked = false);
			Positions.ForEach(x => x.IsChecked = false);

			Departments.ForEach(x =>
			{
				if (Filter.DepartmentUids.Any(y => y == x.Department.Uid))
					x.IsChecked = true;
			});

			Positions.ForEach(x =>
			{
				if (Filter.PositionUids.Any(y => y == x.Position.UID))
					x.IsChecked = true;
			});
		}

		public RelayCommand ClearCommand { get; private set; }
		void OnClear()
		{
			Filter = new EmployeeFilter();
			Initialize();
		}

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			Filter = new EmployeeFilter();
			Departments.ForEach(x =>
			{
				if (x.IsChecked)
				{
					Filter.DepartmentUids.Add(x.Department.Uid);
				}
			});
			Filter.DepartmentUids.Distinct();
			Positions.ForEach(x =>
			{
				if (x.IsChecked)
					Filter.PositionUids.Add(x.Position.UID);
			});
			//Filter.WithDeleted = WithDeleted;
			//if (StartDateTime > EndDateTime)
			//{
			//    MessageBoxService.ShowWarning("Начальная дата должна быть меньше конечной");
			//    return;
			//}
			Close(true);
		}

		List<FilterDepartmentViewModel> GetAllChildren(FilterDepartmentViewModel department)
		{
			var result = new List<FilterDepartmentViewModel>();
			if (department.Department.ChildDepartmentUids.Count == 0)
				return result;
			Departments.ForEach(x =>
			{
				if (department.Department.ChildDepartmentUids.Contains(x.Department.Uid))
				{
					result.Add(x);
					result.AddRange(GetAllChildren(x));
				}
			});
			return result;
		}

		void SetChildren(FilterDepartmentViewModel department)
		{
			if (department.Department.ChildDepartmentUids.Count == 0)
				return;
			var children = Departments.Where(x => department.Department.ChildDepartmentUids.Any(y => y == x.Department.Uid));
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

		public RelayCommand CancelCommand { get; private set; }
		void OnCancel()
		{
			Close(false);
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


		List<FilterPositionViewModel> positions;
		public List<FilterPositionViewModel> Positions
		{
			get { return positions; }
			private set
			{
				positions = value;
				OnPropertyChanged(()=>Positions);
			}
		}

		bool withDeleted;
		public bool WithDeleted
		{
			get { return withDeleted; }
			set
			{
				withDeleted = value;
				OnPropertyChanged(() => WithDeleted);
			}
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

	public class FilterPositionViewModel : BaseViewModel
	{
		public FilterPositionViewModel(Position position)
		{
			Position = position;
		}

		public Position Position { get; private set; }

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
	}
}