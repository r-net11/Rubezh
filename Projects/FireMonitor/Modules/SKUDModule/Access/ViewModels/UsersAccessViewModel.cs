using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class UsersAccessViewModel : ViewPartViewModel
	{
		public UsersAccessViewModel()
		{
			Filter = new EmployeeFilter();
			ShowFilterCommand = new RelayCommand(OnShowFilter);
		}

		void UpdateEmployees()
		{
			Users = new ObservableCollection<UserAccessViewModel>();
			var employees = FiresecManager.GetEmployees(Filter);
			foreach (var employee in employees)
			{
				Users.Add(new UserAccessViewModel(employee));
			}
			SelectedUser = Users.FirstOrDefault();
		}

		EmployeeFilter filter;
		public EmployeeFilter Filter
		{
			get { return filter; }
			set
			{
				filter = value;
				UpdateEmployees();
			}
		}

		ObservableCollection<UserAccessViewModel> _users;
		public ObservableCollection<UserAccessViewModel> Users
		{
			get { return _users; }
			set
			{
				_users = value;
				OnPropertyChanged("Users");
			}
		}

		UserAccessViewModel selectedUser;
		public UserAccessViewModel SelectedUser
		{
			get { return selectedUser; }
			set
			{
				selectedUser = value;
				OnPropertyChanged("SelectedUser");
			}
		}

		public RelayCommand ShowFilterCommand { get; private set; }
		void OnShowFilter()
		{
			var employeeFilterViewModel = new EmployeeFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(employeeFilterViewModel))
			{
				Filter = employeeFilterViewModel.Filter;
			}
		}
	}
}