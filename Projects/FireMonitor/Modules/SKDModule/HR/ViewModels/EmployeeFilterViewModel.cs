using FiresecAPI.Models;
using FiresecAPI.SKD;
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

			var hasEmployeePermission = FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Employees);
			var hasGuestPermission = FiresecManager.CurrentUser.HasPermission(PermissionType.Oper_SKD_Guests);
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
		public EmployeeFilter Save()
		{
			var employeeFilter = new EmployeeFilter();
			employeeFilter.FirstName = FirstName;
			employeeFilter.LastName = LastName;
			employeeFilter.SecondName = SecondName;
			return employeeFilter;
		}
	}
}