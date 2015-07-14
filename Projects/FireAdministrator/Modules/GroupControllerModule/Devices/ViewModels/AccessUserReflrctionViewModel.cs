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
			MirrorUserNewModels = new ObservableCollection<MirrorUserNewModel>();
			foreach (var mirrorUser in device.GKReflectionItem.MirrorUsers)
			{
				var mirrorUserNewModel = new MirrorUserNewModel(mirrorUser);
				MirrorUserNewModels.Add(mirrorUserNewModel);
			}
			SelectedUser = MirrorUserNewModels.FirstOrDefault();

			AddCommand = new RelayCommand(Add);
			DeleteCommand = new RelayCommand(Delete);
			EditCommand = new RelayCommand(Edit);
			ReadCommand = new RelayCommand(Read);
			WriteCommand = new RelayCommand(Write);
		}

		GKDevice Device { get; set; }
		private ObservableCollection<MirrorUserNewModel> _mirrorUserNewModel;
		public ObservableCollection<MirrorUserNewModel> MirrorUserNewModels
		{
			get { return _mirrorUserNewModel; }
			private set
			{
				_mirrorUserNewModel = value;
				OnPropertyChanged(() => MirrorUserNewModels);
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
			var accessDetailsUserReflrctionViewModel = new AccessDetailsUserReflectionViewModel(MirrorUserNewModels);
			if (DialogService.ShowModalWindow(accessDetailsUserReflrctionViewModel))
			{
				var mirrorUser = accessDetailsUserReflrctionViewModel.MirrorUser;
				MirrorUserNewModels.Add(new MirrorUserNewModel(mirrorUser));
				SelectedUser = MirrorUserNewModels.FirstOrDefault();
				Save();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void Delete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить " + SelectedUser.Name))
			{
				var index = MirrorUserNewModels.IndexOf(SelectedUser);
				MirrorUserNewModels.Remove(SelectedUser);
				index = Math.Min(index, MirrorUserNewModels.Count - 1);
				if (index > -1)
					SelectedUser = MirrorUserNewModels[index];
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void Edit()
		{
			if (SelectedUser != null)
			{
				var mirrorUsers = new ObservableCollection<MirrorUserNewModel>(MirrorUserNewModels.Where(x => x.Password != SelectedUser.Password));
				var accessDetailsUserReflrctionViewModel = new AccessDetailsUserReflectionViewModel(mirrorUsers, SelectedUser.MirrorUser);
				if (DialogService.ShowModalWindow(accessDetailsUserReflrctionViewModel))
				{
					var mirrorUser = accessDetailsUserReflrctionViewModel.MirrorUser;
					SelectedUser.Update(mirrorUser);
				}
				ServiceFactory.SaveService.GKChanged = true;
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
			MirrorUserNewModels = new ObservableCollection<MirrorUserNewModel>();
			foreach (var mirrorUser in operationResult.Result )
			{
				var mirrorUserNewModel = new MirrorUserNewModel(mirrorUser);
				MirrorUserNewModels.Add(mirrorUserNewModel);
			}
			SelectedUser = MirrorUserNewModels.FirstOrDefault();
			ServiceFactory.SaveService.GKChanged = true;
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
			Device.GKReflectionItem.MirrorUsers = new List<MirrorUser>(MirrorUserNewModels.Select(x=>x.MirrorUser));
			return base.Save();
		}
	}
}
