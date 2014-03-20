﻿using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeDetailsViewModel : SaveCancelDialogViewModel
	{
		public EmployeesViewModel EmployeesViewModel { get; private set; }
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

			AdditionalColumns = new List<AdditionalColumnViewModel>();
			if (EmployeesViewModel.AdditionalColumnTypes.IsNotNullOrEmpty())
			{
				var columns = AdditionalColumnHelper.GetByEmployee(Employee);
				if (columns != null)
				{
					foreach (var column in columns)
					{
						AdditionalColumns.Add(new AdditionalColumnViewModel(column, this));
					}
				}
			}
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

	public class AdditionalColumnViewModel
	{
		public EmployeeDetailsViewModel EmployeeDetailsViewModel { get; private set; }

		AdditionalColumn AdditionalColumn;

		public string Name { get; private set; }
		public AdditionalColumnType AdditionalColumnType { get; private set; }

			 
		public AdditionalColumnViewModel(AdditionalColumn additionalColumn, EmployeeDetailsViewModel employeeDetailsViewModel)
		{
			AdditionalColumn = additionalColumn;
			EmployeeDetailsViewModel = employeeDetailsViewModel;
			AdditionalColumnType = EmployeeDetailsViewModel.EmployeesViewModel.AdditionalColumnTypes.FirstOrDefault(x => x.UID == AdditionalColumn.AdditionalColumnTypeUID);
			Name = AdditionalColumnType.Name;
		}

	}

}