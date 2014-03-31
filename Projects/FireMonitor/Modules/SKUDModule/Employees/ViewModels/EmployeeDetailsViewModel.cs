using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeDetailsViewModel : SaveCancelDialogViewModel
	{
		public EmployeesViewModel EmployeesViewModel { get; private set; }
		public EmployeeDetails EmployeeDetails { get; private set; }

		public EmployeeDetailsViewModel(EmployeesViewModel employeesViewModel, Employee employee = null)
		{
			EmployeesViewModel = employeesViewModel;
			if (employee == null)
			{
				Title = "Создание сотрудника";
				EmployeeDetails = new EmployeeDetails();
				EmployeeDetails.OrganizationUID = EmployeesViewModel.Organization.UID;
				EmployeeDetails.FirstName = "Новый сотрудник";
			}
			else
			{
				Title = string.Format("Свойства сотрудника: {0}", employee.FirstName);
				EmployeeDetails = EmployeeHelper.GetDetails(employee.UID);
				if (EmployeeDetails == null)
				{
					EmployeeDetails = new EmployeeDetails();
					EmployeeDetails.OrganizationUID = EmployeesViewModel.Organization.UID;
				}
			}

			var positions = PositionHelper.GetByOrganization(EmployeeDetails.OrganizationUID);
			if (positions == null)
				Positions = new List<Position>();
			else
				Positions = positions.ToList();

			if (EmployeeDetails.Position == null)
			{
				SelectedPosition = Positions.FirstOrDefault();
			}
			else
			{
				SelectedPosition = Positions.FirstOrDefault(x => x.UID == EmployeeDetails.Position.UID);
				if (SelectedPosition == null)
					SelectedPosition = Positions.FirstOrDefault();
			}
			
			AdditionalColumns = new List<AdditionalColumnViewModel>();
			if (EmployeesViewModel.AdditionalColumnTypes.IsNotNullOrEmpty())
			{
				foreach (var column in EmployeeDetails.AdditionalColumns)
				{
					AdditionalColumns.Add(new AdditionalColumnViewModel(column));
				}
				SelectedAdditionalColumn = AdditionalColumns.FirstOrDefault();
			}
			CopyProperties();
		}

		public void CopyProperties()
		{
			FirstName = EmployeeDetails.FirstName;
			SecondName = EmployeeDetails.SecondName;
			LastName = EmployeeDetails.LastName;
		}

		public bool HasAdditionalColumns
		{
			get { return AdditionalColumns.IsNotNullOrEmpty(); }
		}

		public List<Position> Positions { get; private set; }

		Position _selectedPosition;
		public Position SelectedPosition
		{
			get { return _selectedPosition; }
			set
			{
				_selectedPosition = value;
				OnPropertyChanged(() => SelectedPosition);
			}
		}

		string _firstName;
		public string FirstName
		{
			get { return _firstName; }
			set
			{
				if (_firstName != value)
				{
					_firstName = value;
					OnPropertyChanged(() => FirstName);
				}
			}
		}

		string _secondName;
		public string SecondName
		{
			get { return _secondName; }
			set
			{
				if (_secondName != value)
				{
					_secondName = value;
					OnPropertyChanged(() => SecondName);
				}
			}
		}

		string _lastName;
		public string LastName
		{
			get { return _lastName; }
			set
			{
				if (_lastName != value)
				{
					_lastName = value;
					OnPropertyChanged(() => LastName);
				}
			}
		}

		List<AdditionalColumnViewModel> additionalColumns;
		public List<AdditionalColumnViewModel> AdditionalColumns
		{
			get { return additionalColumns; }
			set
			{
				additionalColumns = value;
				OnPropertyChanged(() => AdditionalColumns);
			}
		}

		AdditionalColumnViewModel selectedAdditionalColumn;
		public AdditionalColumnViewModel SelectedAdditionalColumn
		{
			get { return selectedAdditionalColumn; }
			set
			{
				selectedAdditionalColumn = value;
				OnPropertyChanged(() => SelectedAdditionalColumn);
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(FirstName);
		}

		protected override bool Save()
		{
			if (EmployeesViewModel.Employees.Any(x => x.Employee.FirstName == FirstName && x.Employee.LastName == LastName && x.Employee.UID != EmployeeDetails.UID))
			{
				MessageBoxService.ShowWarning("Имя и фамилия сотрудника совпадает с введеннымы ранее");
				return false;
			}

			EmployeeDetails.FirstName = FirstName;
			EmployeeDetails.SecondName = SecondName;
			EmployeeDetails.LastName = LastName;
			if (SelectedPosition != null)
				EmployeeDetails.Position = SelectedPosition;
			return true;
		}
	}
}