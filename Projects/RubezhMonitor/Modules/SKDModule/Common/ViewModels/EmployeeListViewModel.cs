using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public abstract class EmployeeListBaseViewModel<TItem> : BaseViewModel
		where TItem:EmployeeListItemViewModel, new()
	{
		protected abstract EmployeeFilter Filter { get; }
		protected abstract EmployeeFilter EmptyFilter { get; }
		public virtual bool CanEditDepartment { get { return false; } }
		public virtual bool CanEditPosition { get { return false; } }
		protected IEmployeeListParent _parent;
		protected bool _isWithDeleted;
		protected bool _isOrganisationDeleted;
		
		public bool IsDeleted { get { return _parent.IsDeleted; } }
		
		public EmployeeListBaseViewModel(IEmployeeListParent parent)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			ServiceFactory.Events.GetEvent<EditEmployeePositionDepartmentEvent>().Unsubscribe(OnEditEmployeePositionDepartment);
			ServiceFactory.Events.GetEvent<EditEmployeePositionDepartmentEvent>().Subscribe(OnEditEmployeePositionDepartment);
			ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Unsubscribe(OnEditEmployee);
			ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Subscribe(OnEditEmployee);
			ServiceFactory.Events.GetEvent<EditEmployee2Event>().Unsubscribe(OnEditEmployee);
			ServiceFactory.Events.GetEvent<EditEmployee2Event>().Subscribe(OnEditEmployee);
			Initialize(parent);
		}

		void Initialize(IEmployeeListParent parent)
		{
			_parent = parent;
			_isWithDeleted = parent.IsWithDeleted;
			_isOrganisationDeleted = _parent.IsOrganisationDeleted;
			var employeeModels = EmployeeHelper.Get(Filter);
			if (employeeModels == null)
				return;
			Employees = new ObservableCollection<TItem>();
			foreach (var employee in employeeModels)
			{
				var viewModel = new TItem();
				viewModel.Initialize(employee);
				viewModel.IsOrganisationDeleted = _isOrganisationDeleted;
				Employees.Add(viewModel);
			}
			SelectedEmployee = Employees.FirstOrDefault();
            AfterInitialize();
		}

        protected virtual void AfterInitialize() { }

		ObservableCollection<TItem> _Employees;
		public ObservableCollection<TItem> Employees 
		{
			get { return _Employees; }
			private set
			{
				_Employees = value;
				OnPropertyChanged(() => Employees);
			}
		}
		
		TItem _selectedEmployee;
		public TItem SelectedEmployee
		{
			get { return _selectedEmployee; }
			set
			{
				_selectedEmployee = value;
				OnPropertyChanged(() => SelectedEmployee);
				Update();
			}
		}

		protected virtual void Update() { }

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var employeeSelectionViewModel = new EmployeeSelectionDialogViewModel(EmptyFilter);
			if (DialogService.ShowModalWindow(employeeSelectionViewModel))
			{
				var viewModel = new TItem();
				viewModel.Initialize(employeeSelectionViewModel.SelectedEmployee);
				var result = AddToParent(viewModel.Employee);
				if (!result)
					return;
				Employees.Add(viewModel);
				SelectedEmployee = viewModel;
				ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Publish(SelectedEmployee.Employee.UID);
			}
		}
		bool CanAdd()
		{
			return !_parent.IsDeleted && ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_Employees_Edit) && IsConnected;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (MessageBoxService.ShowQuestion(string.Format("Вы действительно хотите открепить сотрудника?")))
			{
				var result = RemoveFromParent(SelectedEmployee.Employee);
				if (!result)
					return;
				ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Publish(SelectedEmployee.Employee.UID);
				Employees.Remove(SelectedEmployee);
				SelectedEmployee = Employees.FirstOrDefault();
			}
		}
		bool CanRemove()
		{
			return !_parent.IsDeleted && SelectedEmployee != null && !SelectedEmployee.IsDeleted && ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_Employees_Edit) && IsConnected;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var employeeDetailsViewModel = new EmployeeDetailsViewModel();
			if (employeeDetailsViewModel.Initialize(_parent.OrganisationUID, SelectedEmployee.Employee, PersonType.Employee, CanEditDepartment, CanEditPosition) &&
				DialogService.ShowModalWindow(employeeDetailsViewModel))
			{
				SelectedEmployee.Update(employeeDetailsViewModel.Model);
				ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Publish(SelectedEmployee.Employee.UID);
			}
		}
		bool CanEdit()
		{
			return !_parent.IsDeleted && SelectedEmployee != null && !SelectedEmployee.IsDeleted && ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_Employees_Edit) && IsConnected;
		}

		protected abstract bool AddToParent(ShortEmployee employee);
		protected abstract bool RemoveFromParent(ShortEmployee employee);
		protected abstract Guid GetParentUID(Employee employee);

		void OnEditEmployeePositionDepartment(Employee employee)
		{
			var employeeListItemViewModel = Employees.FirstOrDefault(x => x.Employee.UID == employee.UID);
			if (employeeListItemViewModel == null && GetParentUID(employee) == _parent.UID)
			{
				var shortEmployee = EmployeeHelper.GetSingleShort(employee.UID);
				if (shortEmployee != null)
				{
					var viewModel = new TItem();
					viewModel.Initialize(shortEmployee);
					Employees.Add(viewModel);
				}
			}
			else if (employeeListItemViewModel != null && (GetParentUID(employee) != _parent.UID || GetParentUID(employee) == Guid.Empty)) 
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
					if (shortEmployee.IsDeleted && !_parent.IsWithDeleted)
						Employees.Remove(employeeListItemViewModel);
					else
						employeeListItemViewModel.Update(shortEmployee);
				}
			}
		}

		public bool IsConnected
		{
			get { return ((SafeRubezhService)ClientManager.RubezhService).IsConnected; }
		}
	}

	public class EmployeeListItemViewModel : BaseViewModel
	{
		public ShortEmployee Employee { get; private set; }
		public bool IsDepartmentDeleted { get { return Employee.IsDepartmentDeleted || IsOrganisationDeleted; } }
		public bool IsPositionDeleted { get { return Employee.IsPositionDeleted || IsOrganisationDeleted; } }
		public bool IsDeleted { get { return Employee.IsDeleted || IsOrganisationDeleted; } }
		bool _isOrganisationDeleted;
		public bool IsOrganisationDeleted
		{
			get { return _isOrganisationDeleted; }
			set
			{
				_isOrganisationDeleted = value;
				OnPropertyChanged(() => IsOrganisationDeleted);
				OnPropertyChanged(() => IsDepartmentDeleted);
				OnPropertyChanged(() => IsPositionDeleted);
				OnPropertyChanged(() => IsDeleted);
			}
		}
		
		public EmployeeListItemViewModel() { }

		public void Initialize(ShortEmployee employee)
		{
			Employee = employee;
		}

		public void Update(ShortEmployee employee)
		{
			Employee = employee;
			OnPropertyChanged(() => Employee);
			OnPropertyChanged(() => IsDepartmentDeleted);
			OnPropertyChanged(() => IsPositionDeleted);
			OnPropertyChanged(() => IsDeleted);
		}
}
}
