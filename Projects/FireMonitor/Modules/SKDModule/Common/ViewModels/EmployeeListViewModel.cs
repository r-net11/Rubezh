using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public abstract class EmployeeListBaseViewModel : BaseViewModel
	{
		protected abstract EmployeeFilter Filter { get; }
		protected abstract EmployeeFilter EmptyFilter { get; }
		public virtual bool CanEditDepartment { get { return false; } }
		public virtual bool CanEditPosition { get { return false; } }
		protected Guid _parentUID;
		protected Guid _organisationUID;
		protected HRViewModel _hrViewModel;
		public ObservableCollection<EmployeeListItemViewModel> Employees { get; private set; }

		public EmployeeListBaseViewModel(Guid parentUID, Guid organisationUID, HRViewModel hrViewModel)
		{
			_parentUID = parentUID;
			_organisationUID = organisationUID;
			_hrViewModel = hrViewModel;
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
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			ServiceFactory.Events.GetEvent<EditEmployeePositionDepartmentEvent>().Unsubscribe(OnEditEmployeePositionDepartment);
			ServiceFactory.Events.GetEvent<EditEmployeePositionDepartmentEvent>().Subscribe(OnEditEmployeePositionDepartment);
			ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Unsubscribe(OnEditEmployee);
			ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Subscribe(OnEditEmployee);
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
				var result = AddToParent(employeeListItemViewModel.Employee.UID);
				if (!result)
					return;
				Employees.Add(employeeListItemViewModel);
				SelectedEmployee = employeeListItemViewModel;
				ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Publish(SelectedEmployee.Employee.UID);
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (MessageBoxService.ShowQuestion2(string.Format("Вы уверены?")))
			{
				var result = RemoveFromParent(SelectedEmployee.Employee.UID);
				if (!result)
					return;
				ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Publish(SelectedEmployee.Employee.UID);
				Employees.Remove(SelectedEmployee);
				SelectedEmployee = Employees.FirstOrDefault();
			}
		}
		bool CanRemove()
		{
			return SelectedEmployee != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var employeeDetailsViewModel = new EmployeeDetailsViewModel();
			if (employeeDetailsViewModel.Initialize(_organisationUID, SelectedEmployee.Employee, _hrViewModel, PersonType.Employee, CanEditDepartment, CanEditPosition) &&
				DialogService.ShowModalWindow(employeeDetailsViewModel))
			{
				SelectedEmployee.Update(employeeDetailsViewModel.Model);
				ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Publish(SelectedEmployee.Employee.UID);
			}
		}
		bool CanEdit()
		{
			return SelectedEmployee != null;
		}

		protected abstract bool AddToParent(Guid uid);
		protected abstract bool RemoveFromParent(Guid uid);
		protected abstract Guid GetParentUID(Employee employee);

		void OnEditEmployeePositionDepartment(Employee employee)
		{
			var employeeListItemViewModel = Employees.FirstOrDefault(x => x.Employee.UID == employee.UID);
			if (employeeListItemViewModel == null && GetParentUID(employee) == _parentUID)
			{
				var shortEmployee = EmployeeHelper.GetSingleShort(employee.UID);
				if (shortEmployee != null)
				{
					Employees.Add(new EmployeeListItemViewModel(shortEmployee));
				}
			}
			else if (employeeListItemViewModel != null && GetParentUID(employee) != _parentUID)
			{
				Employees.Remove(employeeListItemViewModel);
			}
		}

		void OnEditEmployee(Guid employeeUID)
		{
			var employeeListItemViewModel = Employees.FirstOrDefault(x => x.Employee.UID == employeeUID);
			if (employeeListItemViewModel != null)
			{
				var shortEmployee = EmployeeHelper.GetSingleShort(employeeUID);
				if (shortEmployee != null)
				{
					employeeListItemViewModel.Update(shortEmployee);
				}
			}
		}
	}

	public class EmployeeListItemViewModel : BaseViewModel
	{
		public ShortEmployee Employee { get; private set; }
		
		public EmployeeListItemViewModel(ShortEmployee employee)
		{
			Employee = employee;
		}

		public void Update(ShortEmployee employee)
		{
			Employee = employee;
			OnPropertyChanged(() => Employee);
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
