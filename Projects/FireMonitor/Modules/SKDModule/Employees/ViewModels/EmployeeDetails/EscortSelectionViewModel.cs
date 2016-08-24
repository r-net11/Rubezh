using System;
using System.Collections.Generic;
using System.Linq;
using Localization.SKD.Common;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EscortSelectionViewModel : SaveCancelDialogViewModel
	{
		public EscortSelectionViewModel(ShortDepartment department, ShortEmployee shortEmployee)
		{
			Title = CommonResources.Maintainer;
			var filter = new EmployeeFilter();
			filter.DepartmentUIDs.Add(department.UID);
			filter.OrganisationUIDs.Add(department.OrganisationUID);

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