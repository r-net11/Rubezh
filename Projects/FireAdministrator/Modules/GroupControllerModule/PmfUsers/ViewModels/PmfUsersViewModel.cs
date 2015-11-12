using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;

namespace GKModule.ViewModels
{
	public class PmfUsersViewModel : MenuViewPartViewModel
	{
		public PmfUsersViewModel()
		{
			Menu = new PmfUsersMenuViewModel(this);
			Users = GKManager.PmfUsers == null ? 
				new ObservableCollection<PmfUserViewModel>() : 
				new ObservableCollection<PmfUserViewModel>(GKManager.PmfUsers.OrderBy(x => x.GkNo).Select(x => new PmfUserViewModel(x)));
			SelectedUser = Users.FirstOrDefault();
			RegisterShortcuts();
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
		}

		public ObservableCollection<PmfUserViewModel> Users { get; private set; }
		PmfUserViewModel _selectedUser;
		public PmfUserViewModel SelectedUser
		{
			get { return _selectedUser; }
			set
			{
				_selectedUser = value;
				OnPropertyChanged(() => SelectedUser);
			}
		}

		#region Commands
		public RelayCommand AddCommand { get { return new RelayCommand(OnAdd); } }
		void OnAdd()
		{
			var pmfUserDetailsViewModel = new PmfUserDetailsViewModel(this);
			if(DialogService.ShowModalWindow(pmfUserDetailsViewModel))
			{
				var user = pmfUserDetailsViewModel.User;
				Users.Add(new PmfUserViewModel(user));
			}
		}

		public RelayCommand EditCommand { get { return new RelayCommand(OnEdit, CanEdit); } }
		void OnEdit()
		{
			var pmfUserDetailsViewModel = new PmfUserDetailsViewModel(this, SelectedUser.User);
			if (DialogService.ShowModalWindow(pmfUserDetailsViewModel))
			{
				var user = pmfUserDetailsViewModel.User;
				SelectedUser.Update(user);
			}
		}
		bool CanEdit()
		{
			return SelectedUser != null;
		}

		public RelayCommand DeleteCommand { get { return new RelayCommand(OnDelete, CanDelete); } }
		void OnDelete()
		{
			Users.Remove(SelectedUser);
			SelectedUser = Users.FirstOrDefault();
			GKManager.PmfUsers.Remove(SelectedUser.User);
			ServiceFactory.SaveService.GKChanged = true;
		}
		bool CanDelete()
		{
			return SelectedUser != null;
		}
		
		public RelayCommand WriteCommand { get { return new RelayCommand(OnWrite); } }
		void OnWrite()
		{
			return;
		}
		#endregion
	}
}
