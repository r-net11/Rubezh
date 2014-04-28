using FiresecAPI;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeFilterViewModel : BaseViewModel
	{
		public EmployeeFilterViewModel(EmployeeFilter employeeFilter)
		{
			FirstName = employeeFilter.FirstName;
			LastName = employeeFilter.LastName;
			SecondName = employeeFilter.SecondName;

			HasManyPersonTypes = FiresecManager.CurrentUser.IsEmployeesAllowed && FiresecManager.CurrentUser.IsGuestsAllowed;
			if (HasManyPersonTypes)
			{
				if (employeeFilter.PersonType == PersonType.Guest)
					IsGuestsAllowed = true;
				else
					IsEmployeesAllowed = true;
			}
		}

		string _firstName;
		public string FirstName
		{
			get { return _firstName; }
			set
			{
				_firstName = value;
				OnPropertyChanged(() => FirstName);
			}
		}

		string _lastName;
		public string LastName
		{
			get { return _lastName; }
			set
			{
				_lastName = value;
				OnPropertyChanged(() => LastName);
			}
		}

		string _secondName;
		public string SecondName
		{
			get { return _secondName; }
			set
			{
				_secondName = value;
				OnPropertyChanged(() => SecondName);
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

			employeeFilter.FirstName = FirstName;
			employeeFilter.LastName = LastName;
			employeeFilter.SecondName = SecondName;

			if (IsGuestsAllowed)
				employeeFilter.PersonType = PersonType.Guest;
			else
				employeeFilter.PersonType = PersonType.Employee;

			return employeeFilter;
		}
	}
}