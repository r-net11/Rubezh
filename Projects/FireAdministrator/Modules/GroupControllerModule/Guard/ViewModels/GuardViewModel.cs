using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.ViewModels;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;
using Infrastructure.Common.Ribbon;
using System.Collections.Generic;

namespace GKModule.ViewModels
{
	public class GuardViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public GuardViewModel()
		{
			Menu = new GuardMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Users = new ObservableCollection<UserViewModel>();
			foreach (var guardUser in XManager.DeviceConfiguration.GuardUsers)
			{
				var userViewModel = new UserViewModel(guardUser);
				Users.Add(userViewModel);
			}
			SelectedUser = Users.FirstOrDefault();
		}

		public ObservableCollection<UserViewModel> Users { get; private set; }

		UserViewModel _selectedUser;
		public UserViewModel SelectedUser
		{
			get { return _selectedUser; }
			set
			{
				_selectedUser = value;
				OnPropertyChanged("SelectedUser");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var userDetailsViewModel = new UserDetailsViewModel();
			if (DialogService.ShowModalWindow(userDetailsViewModel))
			{
				XManager.DeviceConfiguration.GuardUsers.Add(userDetailsViewModel.GuardUser);
				var userViewModel = new UserViewModel(userDetailsViewModel.GuardUser);
				Users.Add(userViewModel);
				SelectedUser = userViewModel;
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			int oldIndex = Users.IndexOf(SelectedUser);

			XManager.DeviceConfiguration.GuardUsers.Remove(SelectedUser.GuardUser);
			Users.Remove(SelectedUser);
			SelectedUser = Users.FirstOrDefault();
			ServiceFactory.SaveService.GKChanged = true;

			if (Users.Count > 0)
				SelectedUser = Users[System.Math.Min(oldIndex, Users.Count - 1)];
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var userDetailsViewModel = new UserDetailsViewModel(SelectedUser.GuardUser);
			if (DialogService.ShowModalWindow(userDetailsViewModel))
			{
				SelectedUser.GuardUser = userDetailsViewModel.GuardUser;
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		bool CanEditDelete()
		{
			return (SelectedUser != null);
		}

		void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}

		void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", AddCommand, "/Controls;component/Images/BAdd.png"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "/Controls;component/Images/BEdit.png"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "/Controls;component/Images/BDelete.png"),
				}, "/Controls;component/Images/BFilter.png") { Order = 2 }
			};
		}

		#region ISelectable<Guid> Members
		public void Select(Guid userUID)
		{
			if (userUID != Guid.Empty)
				SelectedUser = Users.FirstOrDefault(x => x.GuardUser.UID == userUID);
		}
		#endregion
	}
}