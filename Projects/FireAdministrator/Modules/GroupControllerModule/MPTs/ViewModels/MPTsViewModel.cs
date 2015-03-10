using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace GKModule.ViewModels
{
	public class MPTsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public MPTsViewModel()
		{
			Menu = new MPTsMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			MPTs = new ObservableCollection<MPTViewModel>();
			foreach (var mpt in GKManager.DeviceConfiguration.MPTs.OrderBy(x => x.No))
			{
				var mptViewModel = new MPTViewModel(mpt);
				MPTs.Add(mptViewModel);
			}
			SelectedMPT = MPTs.FirstOrDefault();
		}

		ObservableCollection<MPTViewModel> _mpts;
		public ObservableCollection<MPTViewModel> MPTs
		{
			get { return _mpts; }
			set
			{
				_mpts = value;
				OnPropertyChanged(() => MPTs);
			}
		}

		MPTViewModel _selectedMPT;
		public MPTViewModel SelectedMPT
		{
			get { return _selectedMPT; }
			set
			{
				_selectedMPT = value;
				OnPropertyChanged(() => SelectedMPT);
				OnPropertyChanged(() => HasSelectedMPT);
			}
		}

		public bool HasSelectedMPT
		{
			get { return SelectedMPT != null; }
		}

		bool CanEditDelete()
		{
			return SelectedMPT != null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var mptDetailsViewModel = new MPTDetailsViewModel();
			if (DialogService.ShowModalWindow(mptDetailsViewModel))
			{
				GKManager.DeviceConfiguration.MPTs.Add(mptDetailsViewModel.MPT);
				var mptViewModel = new MPTViewModel(mptDetailsViewModel.MPT);
				MPTs.Add(mptViewModel);
				SelectedMPT = mptViewModel;
				OnPropertyChanged(() => HasSelectedMPT);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить МПТ " + SelectedMPT.MPT.Name))
			{
				foreach (var mptDevice in SelectedMPT.MPT.MPTDevices)
				{
					MPTViewModel.ChangeIsInMPT(mptDevice.Device, false);
				}
				var index = MPTs.IndexOf(SelectedMPT);
				GKManager.DeviceConfiguration.MPTs.Remove(SelectedMPT.MPT);
				MPTs.Remove(SelectedMPT);
				index = Math.Min(index, MPTs.Count - 1);
				if (index > -1)
					SelectedMPT = MPTs[index];
				OnPropertyChanged(() => HasSelectedMPT);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var mptDetailsViewModel = new MPTDetailsViewModel(SelectedMPT.MPT);
			if (DialogService.ShowModalWindow(mptDetailsViewModel))
			{
				SelectedMPT.MPT = mptDetailsViewModel.MPT;
				SelectedMPT.Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public void Select(Guid mptUID)
		{
			if (mptUID != Guid.Empty)
				SelectedMPT = MPTs.FirstOrDefault(x => x.MPT.UID == mptUID);
		}

		public override void OnShow()
		{
			base.OnShow();
			SelectedMPT = SelectedMPT;
		}

		public override void OnHide()
		{
			base.OnHide();
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
					new RibbonMenuItemViewModel("Добавить", AddCommand, "BAdd"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "BEdit"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "BDelete"),
				}, "BEdit") { Order = 2 }
			};
		}
	}
}