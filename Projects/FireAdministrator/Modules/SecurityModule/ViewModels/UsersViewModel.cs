using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Ribbon;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using Infrastructure.Events;

namespace SecurityModule.ViewModels
{
	public class UsersViewModel : MenuViewPartViewModel
	{
		public UsersViewModel()
		{
			Menu = new UsersMenuViewModel(this);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			AddCommand = new RelayCommand(OnAdd);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Users = new ObservableCollection<UserViewModel>();
			if (ClientManager.SecurityConfiguration.Users != null)
			{
				foreach (var user in ClientManager.SecurityConfiguration.Users)
					Users.Add(new UserViewModel(user));
			}
			SelectedUser = Users.FirstOrDefault();
		}

		ObservableCollection<UserViewModel> _users;
		public ObservableCollection<UserViewModel> Users
		{
			get { return _users; }
			set
			{
				_users = value;
				OnPropertyChanged(() => Users);
			}
		}

		UserViewModel _selectedUser;
		public UserViewModel SelectedUser
		{
			get { return _selectedUser; }
			set
			{
				_selectedUser = value;
				OnPropertyChanged(() => SelectedUser);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var userDetailsViewModel = new UserDetailsViewModel();
			if (DialogService.ShowModalWindow(userDetailsViewModel))
			{
				ClientManager.SecurityConfiguration.Users.Add(userDetailsViewModel.User);
				var userViewModel = new UserViewModel(userDetailsViewModel.User);
				Users.Add(userViewModel);
				SelectedUser = userViewModel;
				ServiceFactory.SaveService.SecurityChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion(string.Format("Вы уверенны, что хотите удалить пользователя \"{0}\" из списка", SelectedUser.User.Name)))
			{	
				ClientManager.SecurityConfiguration.Users.Remove(SelectedUser.User);
				Users.Remove(SelectedUser);

				ServiceFactory.SaveService.SecurityChanged = true;
			}
		}
		bool CanDelete()
		{
			return SelectedUser != null && SelectedUser.User != ClientManager.CurrentUser;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var userDetailsViewModel = new UserDetailsViewModel(SelectedUser.User);
			if (DialogService.ShowModalWindow(userDetailsViewModel))
			{
				ClientManager.SecurityConfiguration.Users.Remove(SelectedUser.User);
				SelectedUser.User = userDetailsViewModel.User;
				ClientManager.SecurityConfiguration.Users.Add(SelectedUser.User);

				ServiceFactory.SaveService.SecurityChanged = true;
			}
		}
		bool CanEdit()
		{
			return SelectedUser != null;
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}

		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить (Ctrl+N)", AddCommand, "BAdd"),
					new RibbonMenuItemViewModel("Редактировать (Ctrl+E)", EditCommand, "BEdit"),
					new RibbonMenuItemViewModel("Удалить (Ctrl+Del)", DeleteCommand, "BDelete"),
				}, "BEdit") { Order = 2 }
			};
		}
	}
}