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
	class AccessUserReflrctionViewModel : SaveCancelDialogViewModel
	{
		public AccessUserReflrctionViewModel(GKDevice device)
		{
			Device = device;
			Title = "Права доступа для отражения";
			MirrorUsers = new ObservableCollection <MirrorUser>();
			MirrorUsers.Add(new MirrorUser() { Name = "eddddd", DateEndAccess = DateTime.Now.AddYears(1), Type = GKCardType.Manufactor });
			MirrorUsers.Add(new MirrorUser() { Name = "eddddd", DateEndAccess = DateTime.Now.AddYears(1), Type = GKCardType.Manufactor });
			SelectedUser = MirrorUsers.FirstOrDefault();

			AddCommand = new RelayCommand(Add);
			DeleteCommand = new RelayCommand(Delete);
			EditCommand = new RelayCommand(Edit);

		}

		GKDevice Device { get; set; }
		private ObservableCollection <MirrorUser> _mirrorUsers;
		public ObservableCollection <MirrorUser> MirrorUsers
		{
			get { return _mirrorUsers; }
			private set
			{
				_mirrorUsers = value;
				OnPropertyChanged(() => MirrorUsers);
			}
		}

		private MirrorUser _selectedUser;
		public MirrorUser SelectedUser
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
			var accessDetailsUserReflrctionViewModel = new AccessDetailsUserReflrctionViewModel();
			DialogService.ShowModalWindow(accessDetailsUserReflrctionViewModel);
		}

		public RelayCommand DeleteCommand { get; private set; }
		void Delete()
		{
			var index = MirrorUsers.IndexOf(SelectedUser);
			MirrorUsers.Remove(SelectedUser);
			index = Math.Min(index, MirrorUsers.Count - 1);
			if (index > -1)
				SelectedUser = MirrorUsers[index];
			ServiceFactory.SaveService.GKChanged= true;
		}

		public RelayCommand EditCommand { get; private set; }
		void Edit()
		{
			var accessDetailsUserReflrctionViewModel = new AccessDetailsUserReflrctionViewModel(SelectedUser);
			if (DialogService.ShowModalWindow(accessDetailsUserReflrctionViewModel))
			{			
						
			}
		}
		
		protected override bool Save()
		{
			Device.GKReflectionItem.MirrorUsers = new List<MirrorUser>(MirrorUsers);
			return base.Save();
		}
	}
}
