using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeListViewModel : BaseViewModel
	{
		protected virtual EmployeeFilter Filter { get; set; }
		protected virtual EmployeeFilter EmptyFilter { get; set; }
		protected Guid _parentUID;
		public ObservableCollection<EmployeeListItemViewModel> Employees { get; private set; }

		public EmployeeListViewModel(Guid parentUID)
		{
			_parentUID = parentUID;
			var employeeModels = EmployeeHelper.Get(Filter);
			if (employeeModels == null)
				return;
			Employees = new ObservableCollection<EmployeeListItemViewModel>();
			foreach (var employee in employeeModels)
			{
				Employees.Add(new EmployeeListItemViewModel(employee));
			}
			SelectedEmployee = Employees.FirstOrDefault();
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove);
			EditCommand = new RelayCommand(OnEdit);
		}

		EmployeeListItemViewModel _selectedEmployee;
		public EmployeeListItemViewModel SelectedEmployee
		{
			get { return _selectedEmployee; }
			set
			{
				_selectedEmployee = value;
				OnPropertyChanged(() => SelectedEmployee);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var employeeSelectionViewModel = new EmployeeSelectionViewModel(EmptyFilter);
			if (DialogService.ShowModalWindow(employeeSelectionViewModel))
			{
				var employeeListItemViewModel = new EmployeeListItemViewModel(employeeSelectionViewModel.SelectedEmployee);
				var result = AddToParent(employeeListItemViewModel.ShortEmployee.UID);
				if (!result)
					return;
				Employees.Add(employeeListItemViewModel);
				SelectedEmployee = employeeListItemViewModel;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (MessageBoxService.ShowQuestion2(string.Format("Вы уверены в удалении?")))
			{
				var result = RemoveFromParent(SelectedEmployee.ShortEmployee.UID);
				if (!result)
					return;
				Employees.Remove(SelectedEmployee);
				SelectedEmployee = Employees.FirstOrDefault();
			}
			
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			;
		}

		protected virtual bool AddToParent(Guid uid) { return true; }
		protected virtual bool RemoveFromParent(Guid uid) { return true; }
	}

	public class DepartmentEmployeeListViewModel : EmployeeListViewModel
	{
		public DepartmentEmployeeListViewModel(Guid parentUID) : base(parentUID) { }

		protected override bool AddToParent(Guid uid)
		{
			return EmployeeHelper.SetDepartment(uid, _parentUID);
		}

		protected override bool RemoveFromParent(Guid uid)
		{
			return EmployeeHelper.SetDepartment(uid, Guid.Empty);
		}

		protected override EmployeeFilter Filter
		{
			get { return new EmployeeFilter { DepartmentUIDs = new List<Guid> { _parentUID } }; }
			set { ; }
		}

		protected override EmployeeFilter EmptyFilter
		{
			get { return new EmployeeFilter { DepartmentUIDs = new List<Guid> { Guid.Empty } }; }
			set { ; }
		}
	}

	public class EmployeeListItemViewModel : BaseViewModel
	{
		public ShortEmployee ShortEmployee { get; private set; }

		public EmployeeListItemViewModel(ShortEmployee employee)
		{
			ShortEmployee = employee;
		}

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}
	}
}
