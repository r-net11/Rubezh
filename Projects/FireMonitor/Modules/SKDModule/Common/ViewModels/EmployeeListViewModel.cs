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
	public abstract class EmployeeListBaseViewModel<TItem> : BaseViewModel
		where TItem:EmployeeListItemViewModel, new()
	{
		protected abstract EmployeeFilter Filter { get; }
		protected abstract EmployeeFilter EmptyFilter { get; }
		public virtual bool CanEditDepartment { get { return false; } }
		public virtual bool CanEditPosition { get { return false; } }
		protected IOrganisationElementViewModel _parent;
		public ObservableCollection<TItem> Employees { get; private set; }

		public EmployeeListBaseViewModel(IOrganisationElementViewModel parent)
		{
			_parent = parent;
			var employeeModels = EmployeeHelper.Get(Filter);
			if (employeeModels == null)
				return;
			Employees = new ObservableCollection<TItem>();
			foreach (var employee in employeeModels)
			{
				var viewModel = new TItem();
				viewModel.Initialize(employee);
				Employees.Add(viewModel);
			}
			SelectedEmployee = Employees.FirstOrDefault();
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			ServiceFactory.Events.GetEvent<EditEmployeePositionDepartmentEvent>().Unsubscribe(OnEditEmployeePositionDepartment);
			ServiceFactory.Events.GetEvent<EditEmployeePositionDepartmentEvent>().Subscribe(OnEditEmployeePositionDepartment);
			ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Unsubscribe(OnEditEmployee);
			ServiceFactory.Events.GetEvent<EditEmployeeEvent>().Subscribe(OnEditEmployee);
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
			var employeeSelectionViewModel = new EmployeeSelectionViewModel(EmptyFilter);
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
			return !_parent.IsDeleted;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (MessageBoxService.ShowQuestion(string.Format("Вы уверены?")))
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
			return SelectedEmployee != null;
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
			return SelectedEmployee != null;
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
			else if (employeeListItemViewModel != null && GetParentUID(employee) != _parent.UID)
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
		public bool IsDepartmentDeleted { get { return Employee.IsDepartmentDeleted; } }
		public bool IsPositionDeleted { get { return Employee.IsPositionDeleted; } }
		
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
		}
	}
}
