using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.GK;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.ViewModels
{
	class MirrorUsersViewModel : SaveCancelDialogViewModel
	{
		public MirrorUsersViewModel(GKDevice device)
		{
			AddCommand = new RelayCommand(Add);
			DeleteCommand = new RelayCommand(Delete);
			EditCommand = new RelayCommand(Edit);
			ReadCommand = new RelayCommand(Read);
			WriteCommand = new RelayCommand(Write);

			Title = "Права доступа для отражения";
			Device = device;
			Users = new ObservableCollection<MirrorUserNewModel>();
			foreach (var mirrorUser in device.GKMirrorItem.MirrorUsers)
			{
				var mirrorUserNewModel = new MirrorUserNewModel(mirrorUser);
				Users.Add(mirrorUserNewModel);
			}
			SelectedUser = Users.FirstOrDefault();
		}

		GKDevice Device { get; set; }

		private ObservableCollection<MirrorUserNewModel> _users;
		public ObservableCollection<MirrorUserNewModel> Users
		{
			get { return _users; }
			private set
			{
				_users = value;
				OnPropertyChanged(() => Users);
			}
		}

		private MirrorUserNewModel _selectedUser;
		public MirrorUserNewModel SelectedUser
		{
			get { return _selectedUser; }
			set
			{
				_selectedUser = value;
				OnPropertyChanged(() => SelectedUser);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void Add()
		{
			var accessDetailsUserReflrctionViewModel = new MirrorUserDetailsViewModel(Users);
			if (DialogService.ShowModalWindow(accessDetailsUserReflrctionViewModel))
			{
				var mirrorUser = accessDetailsUserReflrctionViewModel.MirrorUser;
				Users.Add(new MirrorUserNewModel(mirrorUser));
				SelectedUser = Users.LastOrDefault();

			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void Delete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить " + SelectedUser.Name))
			{
				var index = Users.IndexOf(SelectedUser);
				Users.Remove(SelectedUser);
				index = Math.Min(index, Users.Count - 1);
				if (index > -1)
					SelectedUser = Users[index];

			}
		}

		public RelayCommand EditCommand { get; private set; }
		void Edit()
		{
			if (SelectedUser != null)
			{
				var mirrorUsers = new ObservableCollection<MirrorUserNewModel>(Users.Where(x => x.Password != SelectedUser.Password));
				var accessDetailsUserReflrctionViewModel = new MirrorUserDetailsViewModel(mirrorUsers, SelectedUser.MirrorUser);
				if (DialogService.ShowModalWindow(accessDetailsUserReflrctionViewModel))
				{
					var mirrorUser = accessDetailsUserReflrctionViewModel.MirrorUser;
					SelectedUser.Update(mirrorUser);
				}

			}
		}
		public RelayCommand ReadCommand { get; private set; }
		void Read()
		{
			var operationResult = ClientManager.FiresecService.GKReadMirrorUsers(Device);
			if (operationResult.HasError)
				MessageBoxService.ShowWarning(operationResult.Error);
			else
			{
				Users = new ObservableCollection<MirrorUserNewModel>();
				foreach (var mirrorUser in operationResult.Result)
				{
					var mirrorUserNewModel = new MirrorUserNewModel(mirrorUser);
					Users.Add(mirrorUserNewModel);
				}
				SelectedUser = Users.LastOrDefault();
			}
		}

		public RelayCommand WriteCommand { get; private set; }
		void Write()
		{
			var operationResult = ClientManager.FiresecService.GKWriteMirrorUsers(Device, Device.GKMirrorItem.MirrorUsers);
			if (operationResult.HasError)
				MessageBoxService.ShowWarning(operationResult.Error);
		}

		protected override bool Save()
		{
			Device.GKMirrorItem.MirrorUsers = new List<MirrorUser>(Users.Select(x => x.MirrorUser));
			ServiceFactory.SaveService.GKChanged = true;
			return base.Save();
		}
	}
}
