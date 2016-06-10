using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;
using StrazhAPI.SKD;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SKDModule.ViewModels
{
	public abstract class EmployeeListBaseViewModel<TItem> : BaseViewModel
		where TItem:EmployeeListItemViewModel, new()
	{
		protected abstract EmployeeFilter Filter { get; }
		protected abstract EmployeeFilter EmptyFilter { get; }
		public virtual bool CanEditDepartment { get { return false; } }
		public virtual bool CanEditPosition { get { return false; } }
		protected IOrganisationElementViewModel Parent;
		protected bool IsWithDeleted;
		protected bool IsOrganisationDeleted;

		public bool IsDeleted { get { return Parent.IsDeleted; } }

		protected EmployeeListBaseViewModel(IOrganisationElementViewModel parent, bool isWithDeleted)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			ServiceFactoryBase.Events.GetEvent<EditEmployeePositionDepartmentEvent>().Unsubscribe(OnEditEmployeePositionDepartment);
			ServiceFactoryBase.Events.GetEvent<EditEmployeePositionDepartmentEvent>().Subscribe(OnEditEmployeePositionDepartment);
			ServiceFactoryBase.Events.GetEvent<EditEmployeeEvent>().Unsubscribe(OnEditEmployee);
			ServiceFactoryBase.Events.GetEvent<EditEmployeeEvent>().Subscribe(OnEditEmployee);
			ServiceFactoryBase.Events.GetEvent<EditEmployee2Event>().Unsubscribe(OnEditEmployee);
			ServiceFactoryBase.Events.GetEvent<EditEmployee2Event>().Subscribe(OnEditEmployee);
			Initialize(parent, isWithDeleted);
		}

		public void Initialize(IOrganisationElementViewModel parent, bool isWithDeleted)
		{
			Parent = parent;
			IsWithDeleted = isWithDeleted;
			IsOrganisationDeleted = Parent.IsOrganisationDeleted;
			var employeeModels = EmployeeHelper.Get(Filter);
			if (employeeModels == null)
				return;
			Employees = new ObservableCollection<TItem>();
			foreach (var employee in employeeModels)
			{
				var viewModel = new TItem();
				viewModel.Initialize(employee);
				viewModel.IsOrganisationDeleted = IsOrganisationDeleted;
				Employees.Add(viewModel);
			}
			SelectedEmployee = Employees.FirstOrDefault();
		}

		private ObservableCollection<TItem> _employees;
		public ObservableCollection<TItem> Employees
		{
			get { return _employees; }
			private set
			{
				_employees = value;
				OnPropertyChanged(() => Employees);
			}
		}

		private TItem _selectedEmployee;
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
		private void OnAdd()
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
				ServiceFactoryBase.Events.GetEvent<EditEmployeeEvent>().Publish(SelectedEmployee.Employee.UID);
			}
		}
		bool CanAdd()
		{
			return !Parent.IsDeleted && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_Employees_Edit);
		}

		public RelayCommand RemoveCommand { get; private set; }
		private void OnRemove()
		{
			if (!MessageBoxService.ShowQuestion(string.Format("Вы действительно хотите открепить сотрудника?"))) return;

			var result = RemoveFromParent(SelectedEmployee.Employee);

			if (!result) return;

			ServiceFactoryBase.Events.GetEvent<EditEmployeeEvent>().Publish(SelectedEmployee.Employee.UID);
			Employees.Remove(SelectedEmployee);
			SelectedEmployee = Employees.FirstOrDefault();
		}
		private bool CanRemove()
		{
			return !Parent.IsDeleted && SelectedEmployee != null && !SelectedEmployee.IsDeleted && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_Employees_Edit);
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			var employeeDetailsViewModel = new EmployeeDetailsViewModel();
			if (employeeDetailsViewModel.Initialize(Parent.OrganisationUID, SelectedEmployee.Employee, PersonType.Employee, CanEditDepartment, CanEditPosition) &&
				DialogService.ShowModalWindow(employeeDetailsViewModel))
			{
				SelectedEmployee.Update(employeeDetailsViewModel.Model);
				ServiceFactoryBase.Events.GetEvent<EditEmployeeEvent>().Publish(SelectedEmployee.Employee.UID);
			}
		}
		bool CanEdit()
		{
			return !Parent.IsDeleted && SelectedEmployee != null && !SelectedEmployee.IsDeleted && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_Employees_Edit);
		}

		protected abstract bool AddToParent(ShortEmployee employee);
		protected abstract bool RemoveFromParent(ShortEmployee employee);
		protected abstract Guid GetParentUID(Employee employee);

		private void OnEditEmployeePositionDepartment(Employee employee)
		{
			var employeeListItemViewModel = Employees.FirstOrDefault(x => x.Employee.UID == employee.UID);
			if (employeeListItemViewModel == null && GetParentUID(employee) == Parent.UID)
			{
				var shortEmployee = EmployeeHelper.GetSingleShort(employee.UID);

				if (shortEmployee == null) return;

				var viewModel = new TItem();
				viewModel.Initialize(shortEmployee);
				Employees.Add(viewModel);
			}
			else if (employeeListItemViewModel != null &&
			         (GetParentUID(employee) != Parent.UID || GetParentUID(employee) == Guid.Empty))
				Employees.Remove(employeeListItemViewModel);
		}

		void OnEditEmployee(Guid employeeUID)
		{
			var employeeListItemViewModel = Employees.FirstOrDefault(x => x.Employee.UID == employeeUID);
			if (employeeListItemViewModel != null)
			{
				var shortEmployee = EmployeeHelper.GetSingleShort(employeeUID);
				if (shortEmployee != null)
				{
					if (shortEmployee.IsDeleted && !Parent.IsWithDeleted)
						Employees.Remove(employeeListItemViewModel);
					else
						employeeListItemViewModel.Update(shortEmployee);
				}
			}
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
