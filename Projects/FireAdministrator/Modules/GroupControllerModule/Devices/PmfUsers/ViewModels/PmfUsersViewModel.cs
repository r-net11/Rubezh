using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.ViewModels;
using RubezhAPI.GK;
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
	public class PmfUsersViewModel : DialogViewModel
	{
		public GKDevice Pmf { get; private set; }
		
		public PmfUsersViewModel(GKDevice pmf)
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			WriteCommand = new RelayCommand(OnWrite);
			ReadCommand = new RelayCommand(OnRead);
			ServiceFactory.Events.GetEvent<GetPmfUsersEvent>().Unsubscribe(OnGetUsers);
			ServiceFactory.Events.GetEvent<GetPmfUsersEvent>().Subscribe(OnGetUsers);
			
			if(pmf == null || pmf.DriverType != GKDriverType.GKMirror)
			{
				MessageBoxService.Show("Неверный тип устройства");
				return;
			}
			Pmf = pmf;
			Title = "Пользователи " + Pmf.PresentationName;
			Users = new ObservableCollection<PmfUserViewModel>(pmf.PmfUsers.OrderBy(x => x.Password).Select(x => new PmfUserViewModel(x)));
			SelectedUser = Users.FirstOrDefault();
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

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var pmfUserDetailsViewModel = new PmfUserDetailsViewModel(this);
			if(DialogService.ShowModalWindow(pmfUserDetailsViewModel))
			{
				var user = pmfUserDetailsViewModel.User;
				Pmf.PmfUsers.Add(user);
				ServiceFactory.SaveService.GKChanged = true;
				Users.Add(new PmfUserViewModel(user));
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var pmfUserDetailsViewModel = new PmfUserDetailsViewModel(this, SelectedUser.User);
			if (DialogService.ShowModalWindow(pmfUserDetailsViewModel))
			{
				var user = pmfUserDetailsViewModel.User;
				Pmf.PmfUsers.Remove(SelectedUser.User);
				Pmf.PmfUsers.Add(user);
				ServiceFactory.SaveService.GKChanged = true;
				SelectedUser.Update(user);
			}
		}
		bool CanEdit()
		{
			return SelectedUser != null;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var index = Users.IndexOf(SelectedUser);
			var user = SelectedUser.User;
			Users.Remove(SelectedUser);
			Pmf.PmfUsers.RemoveAll(x => x.Password == user.Password);
			if(index > 0)
				index--;
			if(Users.Count > 0)
				SelectedUser = Users.ElementAt(index);
			ServiceFactory.SaveService.GKChanged = true;
		}
		bool CanDelete()
		{
			return SelectedUser != null;
		}

		public RelayCommand WriteCommand { get; private set; }
		void OnWrite()
		{
			var result = ClientManager.FiresecService.RewritePmfUsers(Pmf.UID, Users.Select(x => x.User).ToList());
			if (result.HasError)
			{
				MessageBoxService.Show(result.Error);
				return;
			}
		}

		public RelayCommand ReadCommand { get; private set; }
		void OnRead()
		{
			var result = ClientManager.FiresecService.GKGetUsers(Pmf);
			if (result.HasError)
			{
				MessageBoxService.Show(result.Error);
				return;
			}
		}
		void OnGetUsers(List<GKUser> users)
		{
			Pmf.PmfUsers = new List<GKUser>(users.Where(x => x.IsActive));
			ServiceFactory.SaveService.GKChanged = true;
			Users = new ObservableCollection<PmfUserViewModel>(Pmf.PmfUsers.Select(x => new PmfUserViewModel(x)));
			OnPropertyChanged(() => Users);
		}
	}
}
