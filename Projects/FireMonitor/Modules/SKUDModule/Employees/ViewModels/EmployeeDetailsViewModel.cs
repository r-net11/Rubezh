using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

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
			
			var positions = PositionHelper.GetByOrganization(Employee.OrganizationUID);
			if (positions == null)
				Positions = new List<Position>();
			else
				Positions = positions.ToList();
			
			SelectedPosition = Positions.FirstOrDefault(x => x.UID == Employee.PositionUID);
			if(SelectedPosition == null)
				SelectedPosition = positions.FirstOrDefault();

			CopyProperties();
		}

		public void CopyProperties()
		{
			FirstName = Employee.FirstName;
			SecondName = Employee.SecondName;
			LastName = Employee.LastName;
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
					OnPropertyChanged(()=>FirstName);
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
					OnPropertyChanged(()=>SecondName);
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
					OnPropertyChanged(()=>LastName);
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
			Employee.PositionUID = SelectedPosition.UID;
			return true;
		}
	}
}