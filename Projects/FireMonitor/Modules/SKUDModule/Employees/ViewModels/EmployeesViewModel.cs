using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;
using SKDModule.PassCard.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeesViewModel : ViewPartViewModel
	{
		public static EmployeesViewModel Current { get; private set; }
		public PassCardViewModel PassCardViewModel { get; private set; }
		EmployeeFilter Filter;
		PersonType PersonType;
		public Organization Organization { get; private set; }
		public List<AdditionalColumnType> AdditionalColumnTypes { get; private set; }

		public EmployeesViewModel()
		{
			ShowFilterCommand = new RelayCommand(OnShowFilter);
			RefreshCommand = new RelayCommand(OnRefresh);
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			AddCardCommand = new RelayCommand(OnAddCard, CanAddCard);

			Filter = new EmployeeFilter();
			if (FiresecManager.CurrentUser.IsGuestsAllowed)
				Filter.PersonType = PersonType.Guest;
			else
				Filter.PersonType = PersonType.Employee;
			var organizationUID = FiresecManager.CurrentUser.OrganisationUIDs.FirstOrDefault();
			if (organizationUID != Guid.Empty)
				Filter.OrganizationUIDs = new List<Guid> { organizationUID };
			Initialize();
		}

		void Initialize()
		{
			PersonType = Filter.PersonType;
			PassCardViewModel = new PassCardViewModel(this);
			Organization = OrganizationHelper.GetSingle(Filter.OrganizationUIDs.FirstOrDefault());
			var employees = EmployeeHelper.Get(Filter);
			Employees = new ObservableCollection<EmployeeViewModel>();
			foreach (var employee in employees)
			{
				var employeeViewModel = new EmployeeViewModel(this, employee);
				Employees.Add(employeeViewModel);
			}
			SelectedEmployee = Employees.FirstOrDefault();

			InitializeAdditionalColumns();
			ServiceFactory.Events.GetEvent<UpdateAdditionalColumns>().Publish(null);
		}

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
				PassCardViewModel.CreatePassCard();
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
			EmployeeDetails employeeDetails = null;
			if (PersonType == PersonType.Employee)
			{
				var employeeDetailsViewModel = new EmployeeDetailsViewModel(this);
				if (DialogService.ShowModalWindow(employeeDetailsViewModel))
					employeeDetails = employeeDetailsViewModel.EmployeeDetails;
			}
			else if (PersonType == PersonType.Guest)
			{
				var guestDetailsViewModel = new GuestDetailsViewModel(this);
				if (DialogService.ShowModalWindow(guestDetailsViewModel))
					employeeDetails = guestDetailsViewModel.EmployeeDetails;
			}

			if (employeeDetails == null)
				return;

			var employee = employeeDetails.GetEmployee();
			var saveResult = EmployeeHelper.Save(employee);
			if (!saveResult)
				return;
			
			var employeeViewModel = new EmployeeViewModel(this, employee);
			Employees.Add(employeeViewModel);
			SelectedEmployee = employeeViewModel;
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
			EmployeeDetails employeeDetails = null;
			if (PersonType == PersonType.Employee)
			{
				var employeeDetailsViewModel = new EmployeeDetailsViewModel(this, SelectedEmployee.Employee);
				if (DialogService.ShowModalWindow(employeeDetailsViewModel))
					employeeDetails = employeeDetailsViewModel.EmployeeDetails;
			}
			else if (PersonType == PersonType.Guest)
			{
				var guestDetailsViewModel = new GuestDetailsViewModel(this, SelectedEmployee.Employee);
				if (DialogService.ShowModalWindow(guestDetailsViewModel))
					employeeDetails = guestDetailsViewModel.EmployeeDetails;
			}

			if (employeeDetails == null)
				return;

			var employee = employeeDetails.GetEmployee();
			var saveResult = EmployeeHelper.Save(employee);
			if (!saveResult)
				return;
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

		public RelayCommand ShowFilterCommand { get; private set; }
		void OnShowFilter()
		{
			var employeeFilterViewModel = new EmployeeFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(employeeFilterViewModel))
			{
				Filter = employeeFilterViewModel.Filter;
				Initialize();
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}

		public void InitializeAdditionalColumns()
		{
			AdditionalColumnNames = new ObservableCollection<string>();
			var additionalColumnTypeFilter = new AdditionalColumnTypeFilter();
			if (Organization != null)
				additionalColumnTypeFilter.OrganizationUIDs.Add(Organization.UID);
			var columnTypes = AdditionalColumnTypeHelper.Get(additionalColumnTypeFilter);
			if (columnTypes == null)
				return;
			AdditionalColumnTypes = columnTypes.ToList();
			foreach (var additionalColumnType in AdditionalColumnTypes)
			{
				if (additionalColumnType.DataType == AdditionalColumnDataType.Text)
					AdditionalColumnNames.Add(additionalColumnType.Name);
			}
			foreach (var employee in Employees)
			{
				employee.AdditionalColumnValues = new ObservableCollection<string>();
				foreach (var additionalColumnType in AdditionalColumnTypes)
				{
					if (additionalColumnType.DataType == AdditionalColumnDataType.Text)
						employee.AdditionalColumnValues.Add("Test " + additionalColumnType.Name);
				}
			}
		}

		public ObservableCollection<string> AdditionalColumnNames { get; private set; }
	}
}