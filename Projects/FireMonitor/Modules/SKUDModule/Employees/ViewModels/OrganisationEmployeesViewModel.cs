using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using SKDModule.ViewModels;
using FiresecAPI;
using Infrastructure.Common.Windows;
using FiresecClient.SKDHelpers;
using System.Windows;

namespace SKDModule.ViewModels
{
	public class OrganisationEmployeesViewModel : BaseViewModel
	{
		public OrganisationEmployeesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			AddCardCommand = new RelayCommand(OnAddCard, CanAddCard);
		}

		public void Initialize(Organization organization, List<Employee> employees)
		{
			Organization = organization;
			Name = Organization.Name;

			Employees = new ObservableCollection<EmployeeViewModel>();
			foreach (var employee in employees)
			{
				var employeeViewModel = new EmployeeViewModel(this, employee);
				Employees.Add(employeeViewModel);
			}
			SelectedEmployee = Employees.FirstOrDefault();

			InitializeAdditionalColumns();
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		public void InitializeAdditionalColumns()
		{
			AdditionalColumnNames = new ObservableCollection<string>();

			var additionalColumnTypeFilter = new AdditionalColumnTypeFilter();
			additionalColumnTypeFilter.OrganizationUIDs.Add(Organization.UID);
			var additionalColumnTypes = AdditionalColumnTypeHelper.Get(additionalColumnTypeFilter);
			if (additionalColumnTypes != null)
			{
				foreach (var additionalColumnType in additionalColumnTypes)
				{
					AdditionalColumnNames.Add(additionalColumnType.Name);
				}
				foreach (var employee in Employees)
				{
					employee.AdditionalColumnValues = new ObservableCollection<string>();
					foreach (var additionalColumnType in additionalColumnTypes)
					{
						employee.AdditionalColumnValues.Add("Test " + additionalColumnType.Name);
					}
				}
			}
		}

		public ObservableCollection<string> AdditionalColumnNames { get; private set; }

		public Organization Organization { get; private set; } 

		ObservableCollection<EmployeeViewModel> _employee;
		public ObservableCollection<EmployeeViewModel> Employees
		{
			get { return _employee; }
			set
			{
				_employee = value;
				OnPropertyChanged("Employees");
			}
		}

		EmployeeViewModel _selectedEmployee;
		public EmployeeViewModel SelectedEmployee
		{
			get { return _selectedEmployee; }
			set
			{
				_selectedEmployee = value;
				OnPropertyChanged("SelectedEmployee");

				RealSelectedEmployee = value;
			}
		}

		public bool DoNotSelectEmployee = false;

		EmployeeViewModel _realSelectedEmployee;
		public EmployeeViewModel RealSelectedEmployee
		{
			get { return _realSelectedEmployee; }
			set
			{
				if (DoNotSelectEmployee)
				{
					value = null;
					DoNotSelectEmployee = false;
				}

				_realSelectedEmployee = value;
				OnPropertyChanged("RealSelectedEmployee");

				IsCard = value == null;

				if (value != null)
				{
					SelectedCard = null;
				}
			}
		}

		EmployeeCardViewModel selectedCard;
		public EmployeeCardViewModel SelectedCard
		{
			get { return selectedCard; }
			set
			{
				selectedCard = value;
				OnPropertyChanged("SelectedCard");
				IsCard = value != null;

				foreach (var user in Employees)
				{
					foreach (var card in user.Cards)
					{
						card.IsCardBold = false;
						card.IsPassBold = false;
					}
				}
				if (value != null)
				{
					if (IsCard)
						value.IsCardBold = true;
					else
						value.IsPassBold = true;
				}
			}
		}

		bool _isCard;
		public bool IsCard
		{
			get { return _isCard; }
			set
			{
				_isCard = value;
				OnPropertyChanged("IsCard");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var employeeDetailsViewModel = new EmployeeDetailsViewModel(this);
			if (DialogService.ShowModalWindow(employeeDetailsViewModel))
			{
				var employee = employeeDetailsViewModel.Employee;
				var saveResult = EmployeeHelper.Save(employee);
				if (!saveResult)
					return;
				var employeeViewModel = new EmployeeViewModel(this, employee);
				Employees.Add(employeeViewModel);
				SelectedEmployee = employeeViewModel;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var employee = SelectedEmployee.Employee;
			var removeResult = EmployeeHelper.MarkDeleted(employee);
			if (!removeResult)
				return;

			var index = Employees.IndexOf(SelectedEmployee);
			Employees.Remove(SelectedEmployee);
			index = Math.Min(index, Employees.Count - 1);
			if (index > -1)
				SelectedEmployee = Employees[index];
		}
		bool CanRemove()
		{
			return SelectedEmployee != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var employeeDetailsViewModel = new EmployeeDetailsViewModel(this, SelectedEmployee.Employee);
			if (DialogService.ShowModalWindow(employeeDetailsViewModel))
			{
				var employee = employeeDetailsViewModel.Employee;
				var saveResult = EmployeeHelper.Save(employee);
				if (!saveResult)
					return;

				SelectedEmployee.Update(employee);
			}
		}
		bool CanEdit()
		{
			return SelectedEmployee != null;
		}

		public RelayCommand AddCardCommand { get; private set; }
		void OnAddCard()
		{
			if (SelectedEmployee.CanAddCard())
				SelectedEmployee.AddCardCommand.Execute();
		}
		bool CanAddCard()
		{
			return SelectedEmployee != null;
		}
	}
}