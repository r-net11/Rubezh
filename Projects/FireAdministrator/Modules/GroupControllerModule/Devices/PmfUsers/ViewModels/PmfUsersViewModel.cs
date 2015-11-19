using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
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
			if(pmf == null || pmf.DriverType != GKDriverType.GKMirror)
			{
				MessageBoxService.Show("Неверный тип устройства");
				return;
			}
			Pmf = pmf;
			Title = "Пользователи " + Pmf.PresentationName;
			Users = GKManager.PmfUsers == null ? 
				new ObservableCollection<PmfUserViewModel>() : 
				new ObservableCollection<PmfUserViewModel>(GKManager.PmfUsers.Where(x => x.DeviceUID == pmf.UID).OrderBy(x => x.GkNo).Select(x => new PmfUserViewModel(x)));
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
			var result = ClientManager.FiresecService.WriteAllGKUsers(Users.Select(x => x.User).ToList());
			if (result.HasError)
			{
				MessageBoxService.Show(result.Error);
				return;
			}
		}

		public RelayCommand ReadCommand { get { return new RelayCommand(OnRead); } }
		void OnRead()
		{
			return;
			var result = ClientManager.FiresecService.GetGKUsers(Pmf.UID);
			if (result.HasError)
			{
				MessageBoxService.Show(result.Error);
				return;
			}
			GKManager.PmfUsers.RemoveAll(x => x.DeviceUID == Pmf.UID);
			GKManager.PmfUsers.AddRange(result.Result);
			ServiceFactory.SaveService.GKChanged = true;
			Users = new ObservableCollection<PmfUserViewModel>(result.Result.Select(x => new PmfUserViewModel(x)));
			OnPropertyChanged(() => Users);
		}
		#endregion
	}
}
