using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using FiresecAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure;
using System.Collections.ObjectModel;

namespace GKModule.ViewModels
{
	class ReflectionUsersViewModel : SaveCancelDialogViewModel
	{
		public ReflectionUsersViewModel(GKDevice device)
		{
			AddCommand = new RelayCommand(Add);
			DeleteCommand = new RelayCommand(Delete);
			EditCommand = new RelayCommand(Edit);
			ReadCommand = new RelayCommand(Read);
			WriteCommand = new RelayCommand(Write);

			Title = "Права доступа для отражения";
			Device = device;
			Users = new ObservableCollection<MirrorUserNewModel>();
			foreach (var mirrorUser in device.GKReflectionItem.MirrorUsers)
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
			var accessDetailsUserReflrctionViewModel = new ReflectionUserDetailsViewModel(Users);
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
				var accessDetailsUserReflrctionViewModel = new ReflectionUserDetailsViewModel(mirrorUsers, SelectedUser.MirrorUser);
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
			var operationResult = FiresecManager.FiresecService.GKReadMirrorUsers(Device);
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
			var operationResult = FiresecManager.FiresecService.GKWriteMirrorUsers(Device, Device.GKReflectionItem.MirrorUsers);
			if(operationResult.HasError)
				MessageBoxService.ShowWarning(operationResult.Error);
		}
		
		protected override bool Save()
		{
			Device.GKReflectionItem.MirrorUsers = new List<MirrorUser>(Users.Select(x=>x.MirrorUser));
			ServiceFactory.SaveService.GKChanged = true;
			return base.Save();
		}
	}
}
