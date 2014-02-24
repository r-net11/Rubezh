using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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
			MPTs = XManager.DeviceConfiguration.MPTs == null ? new ObservableCollection<MPTViewModel>() : new ObservableCollection<MPTViewModel>(
				from mpt in XManager.DeviceConfiguration.MPTs
				select new MPTViewModel(mpt));
			SelectedMPT = MPTs.FirstOrDefault();
		}

		ObservableCollection<MPTViewModel> _mpts;
		public ObservableCollection<MPTViewModel> MPTs
		{
			get { return _mpts; }
			set
			{
				_mpts = value;
				OnPropertyChanged("MPTs");
			}
		}

		MPTViewModel _selectedMPT;
		public MPTViewModel SelectedMPT
		{
			get { return _selectedMPT; }
			set
			{
				_selectedMPT = value;
				OnPropertyChanged("SelectedMPT");
			}
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
				XManager.DeviceConfiguration.MPTs.Add(mptDetailsViewModel.MPT);
				var mptViewModel = new MPTViewModel(mptDetailsViewModel.MPT);
                MPTs.Add(mptViewModel);
                SelectedMPT = mptViewModel;
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить фильтр " + SelectedMPT.MPT.Name);
			if (dialogResult == MessageBoxResult.Yes)
			{
				XManager.DeviceConfiguration.MPTs.Remove(SelectedMPT.MPT);
				MPTs.Remove(SelectedMPT);
				SelectedMPT = MPTs.FirstOrDefault();
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
				SelectedMPT = MPTs.FirstOrDefault(x => x.MPT.BaseUID == mptUID);
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
					new RibbonMenuItemViewModel("Добавить", AddCommand, "/Controls;component/Images/BAdd.png"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "/Controls;component/Images/BEdit.png"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "/Controls;component/Images/BDelete.png"),
				}, "/Controls;component/Images/BEdit.png") { Order = 2 }
			};
		}
	}
}