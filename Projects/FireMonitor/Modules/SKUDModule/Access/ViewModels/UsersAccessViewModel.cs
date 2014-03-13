using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient.SKDHelpers;
using System.Diagnostics;
using System;

namespace SKDModule.ViewModels
{
	public class UsersAccessViewModel : ViewPartViewModel
	{
		public static UsersAccessViewModel Current { get; private set; }

		public UsersAccessViewModel()
		{
			Current = this;
			Filter = new EmployeeFilter();
			ShowFilterCommand = new RelayCommand(OnShowFilter);
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			AddCardCommand = new RelayCommand(OnAddCard, CanAddCard);
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

		void UpdateEmployees()
		{
			Users = new ObservableCollection<EmployeeViewModel>();
			var employees = EmployeeHelper.Get(Filter);
			if (employees == null)
				return;
			foreach (var employee in employees)
			{
				Users.Add(new EmployeeViewModel(employee));
			}
			SelectedUser = Users.FirstOrDefault();
		}

		ObservableCollection<EmployeeViewModel> _users;
		public ObservableCollection<EmployeeViewModel> Users
		{
			get { return _users; }
			set
			{
				_users = value;
				OnPropertyChanged("Users");
			}
		}

		EmployeeViewModel selectedUser;
		public EmployeeViewModel SelectedUser
		{
			get { return selectedUser; }
			set
			{
				RealSelectedUser = value;

				if (ResetUser)
				{
					value = null;
					ResetUser = false;
				}

				selectedUser = value;
				OnPropertyChanged("SelectedUser");

				IsUser = value != null;
				IsCard = !IsUser;
			}
		}

		public EmployeeViewModel RealSelectedUser;

		EmployeeCardViewModel selectedCard;
		public EmployeeCardViewModel SelectedCard
		{
			get { return selectedCard; }
			set
			{
				selectedCard = value;
				OnPropertyChanged("SelectedCard");

				IsCard = value != null;
				IsUser = !IsCard;

				foreach (var user in Users)
				{
					foreach (var card in user.Cards)
					{
						card.IsBold = false;
					}
				}

				if (value != null)
				{
					value.IsBold = true;
				}
			}
		}

		bool _isUser;
		public bool IsUser
		{
			get { return _isUser; }
			set
			{
				_isUser = value;
				OnPropertyChanged("IsUser");
			}
		}

		bool _isCard;
		public bool IsCard
		{
			get { return _isCard; }
			set
			{
				_isCard = value;
				OnPropertyChanged("IsCard");
			}
		}

		public bool ResetUser = false;

		public RelayCommand ShowFilterCommand { get; private set; }
		void OnShowFilter()
		{
			var employeeFilterViewModel = new EmployeeFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(employeeFilterViewModel))
			{
				Filter = employeeFilterViewModel.Filter;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var employeeDetailsViewModel = new EmployeeDetailsViewModel(null);
			if (DialogService.ShowModalWindow(employeeDetailsViewModel))
			{
				var employee = employeeDetailsViewModel.Employee;
				var saveResult = EmployeeHelper.Save(employee);
				if (!saveResult)
					return;
				var employeeViewModel = new EmployeeViewModel(employee);
				Users.Add(employeeViewModel);
				SelectedUser = employeeViewModel;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var employee = SelectedUser.Employee;
			var removeResult = EmployeeHelper.MarkDeleted(employee);
			if (!removeResult)
				return;

			var index = Users.IndexOf(SelectedUser);
			Users.Remove(SelectedUser);
			index = Math.Min(index, Users.Count - 1);
			if (index > -1)
				SelectedUser = Users[index];
		}
		bool CanRemove()
		{
			return SelectedUser != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var employeeDetailsViewModel = new EmployeeDetailsViewModel(null, SelectedUser.Employee);
			if (DialogService.ShowModalWindow(employeeDetailsViewModel))
			{
				var employee = employeeDetailsViewModel.Employee;
				var saveResult = EmployeeHelper.Save(employee);
				if (!saveResult)
					return;

				SelectedUser.Update(employee);
			}
		}
		bool CanEdit()
		{
			return SelectedUser != null;
		}

		public RelayCommand AddCardCommand { get; private set; }
		void OnAddCard()
		{
			if (RealSelectedUser.CanAddCard())
				RealSelectedUser.AddCardCommand.Execute();
		}
		bool CanAddCard()
		{
			return RealSelectedUser != null;
		}
	}
}