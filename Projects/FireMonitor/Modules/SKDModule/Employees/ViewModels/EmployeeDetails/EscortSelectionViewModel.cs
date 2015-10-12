using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EscortSelectionViewModel : SaveCancelDialogViewModel
	{
		public EscortSelectionViewModel(EmployeeItem department, ShortEmployee shortEmployee, Guid organisationUID)
		{
			Title = "Сопровождающий";
			var filter = new EmployeeFilter();
			if(EmployeeItem.IsNotNullOrEmpty(department))
				filter.DepartmentUIDs.Add(department.UID);
			filter.OrganisationUIDs.Add(organisationUID);

			Employees = new List<ShortEmployee>();
			var employees = EmployeeHelper.Get(filter);
			if (employees != null)
			{
				foreach (var employee in employees)
				{
					Employees.Add(employee);
				}
			}

			if (shortEmployee != null)
			{
				SelectedEmployee = Employees.FirstOrDefault(x => x.UID == shortEmployee.UID);
			}
		}

		public List<ShortEmployee> Employees { get; private set; }

		ShortEmployee _selectedEmployee;
		public ShortEmployee SelectedEmployee
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
			return base.Save();
		}
	}
}