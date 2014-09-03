using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SelectEscortViewModel : SaveCancelDialogViewModel
	{
		public SelectEscortViewModel(ShortDepartment department, Guid? escortUID = null)
		{
			Title = "Сопровождающий";
			var filter = new EmployeeFilter();
			filter.DepartmentUIDs.Add(department.UID);
			filter.OrganisationUIDs.Add(department.OrganisationUID);
			var employees = EmployeeHelper.Get(filter);
			if (employees == null)
				return;
			Employees = new List<SelectationEmployeeViewModel>();
			foreach (var employee in employees)
			{
				Employees.Add(new SelectationEmployeeViewModel(employee));
			}
			if (Employees.Count == 0)
				return;
			if (escortUID != null)
			{
				SelectedEmployee = Employees.FirstOrDefault(x => x.Employee.UID == escortUID.Value);
				if (SelectedEmployee == null)
					SelectedEmployee = Employees.FirstOrDefault();
			}
			else
				SelectedEmployee = Employees.FirstOrDefault();
			SelectedEmployee.IsChecked = true;
		}

		public List<SelectationEmployeeViewModel> Employees { get; private set; }

		SelectationEmployeeViewModel _selectedEmployee;
		public SelectationEmployeeViewModel SelectedEmployee
		{
			get { return _selectedEmployee; }
			set
			{
				_selectedEmployee = value;
				OnPropertyChanged(() => SelectedEmployee);
			}
		}

		protected override bool Save()
		{
			SelectedEmployee = Employees.FirstOrDefault(x => x.IsChecked);
			return base.Save();
		}
	}

	public class SelectationEmployeeViewModel : BaseViewModel
	{
		public ShortEmployee Employee { get; private set; }

		public SelectationEmployeeViewModel(ShortEmployee employee)
		{
			Employee = employee;
		}

		public string Name { get { return string.Format("{0} {1} {2}", Employee.LastName, Employee.FirstName, Employee.SecondName); } }
		public string PositionName { get { return Employee.PositionName; } }
		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}
	}
}