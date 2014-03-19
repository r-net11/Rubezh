using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class EmployeeDetailsViewModel : SaveCancelDialogViewModel
	{
		EmployeesViewModel EmployeesViewModel;
		public Employee Employee { get; private set; }

		public EmployeeDetailsViewModel(EmployeesViewModel employeesViewModel, Employee employee = null)
		{
			EmployeesViewModel = employeesViewModel;
			if (employee == null)
			{
				Title = "Создание сотрудника";
				employee = new Employee()
				{
					FirstName = "Новый сотрудник",
				};
			}
			else
			{
				Title = string.Format("Свойства сотрудника: {0}", employee.FirstName);
			}
			Employee = employee;
			CopyProperties();
		}

		public void CopyProperties()
		{
			FirstName = Employee.FirstName;
			LastName = Employee.LastName;
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
					OnPropertyChanged("FirstName");
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
					OnPropertyChanged("LastName");
				}
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(FirstName);
		}

		protected override bool Save()
		{
			//if (EmployeesViewModel.Employees.Any(x => x.Employee.FirstName == FirstName && x.Employee.LastName == LastName && x.Employee.UID != Employee.UID))
			//{
			//    MessageBoxService.ShowWarning("Имя и фамилия сотрудника совпадает с введеннымы ранее");
			//    return false;
			//}

			Employee.FirstName = FirstName;
			Employee.LastName = LastName;
			Employee.OrganizationUID = EmployeesViewModel.Organization.UID;
			return true;
		}
	}
}