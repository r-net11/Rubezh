using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class GuestDetailsViewModel : SaveCancelDialogViewModel
	{
		public EmployeesViewModel EmployeesViewModel { get; private set; }
		public Employee EmployeeDetails { get; private set; }

		public GuestDetailsViewModel(EmployeesViewModel employeesViewModel, EmployeeListItem employee = null)
		{
			EmployeesViewModel = employeesViewModel;
			if (employee == null)
			{
				Title = "Добавить посетителя";
				EmployeeDetails = new Employee();
				EmployeeDetails.OrganizationUID = EmployeesViewModel.Organization.UID;
				EmployeeDetails.FirstName = "Новый посетитель";
			}
			else
			{
				Title = string.Format("Свойства посетителя: {0}", employee.FirstName);
				EmployeeDetails = EmployeeHelper.GetDetails(employee.UID);
				if (EmployeeDetails == null)
				{
					EmployeeDetails = new Employee();
					EmployeeDetails.OrganizationUID = EmployeesViewModel.Organization.UID;
				}
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

		public bool HasAdditionalColumns
		{
			get { return AdditionalColumns.IsNotNullOrEmpty(); }
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
			return true;
		}
	}
}
