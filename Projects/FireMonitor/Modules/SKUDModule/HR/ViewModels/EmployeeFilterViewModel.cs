using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class EmployeeFilterViewModel : BaseViewModel
	{
		public EmployeeFilterViewModel(EmployeeFilter employeeFilter)
		{
			HasManyPersonTypes = FiresecManager.CurrentUser.IsEmployeesAllowed && FiresecManager.CurrentUser.IsGuestsAllowed;
			if (HasManyPersonTypes)
			{
				if (employeeFilter.PersonType == PersonType.Guest)
					IsGuestsAllowed = true;
				else
					IsEmployeesAllowed = true;
			}
		}

		public bool HasManyPersonTypes { get; private set; }

		bool _isEmployeesAllowed;
		public bool IsEmployeesAllowed
		{
			get { return _isEmployeesAllowed; }
			set
			{
				_isEmployeesAllowed = value;
				OnPropertyChanged(() => IsEmployeesAllowed);
			}
		}

		bool _isGuestsAllowed;
		public bool IsGuestsAllowed
		{
			get { return _isGuestsAllowed; }
			set
			{
				_isGuestsAllowed = value;
				OnPropertyChanged(() => IsGuestsAllowed);
			}
		}

		public EmployeeFilter Save()
		{
			var employeeFilter = new EmployeeFilter();

			if (IsGuestsAllowed)
				employeeFilter.PersonType = PersonType.Guest;
			else
				employeeFilter.PersonType = PersonType.Employee;

			return employeeFilter;
		}
	}
}