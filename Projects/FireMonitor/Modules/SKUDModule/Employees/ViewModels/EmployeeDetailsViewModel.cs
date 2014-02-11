using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class EmployeeDetailsViewModel : SaveCancelDialogViewModel
	{
		public Employee Employee { get; private set; }

		public EmployeeDetailsViewModel(Employee employee = null)
		{
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
			return true;
		}

		protected override bool Save()
		{
			Employee = new Employee()
			{
				FirstName = FirstName,
				LastName = LastName
			};
			return true;
		}
	}
}