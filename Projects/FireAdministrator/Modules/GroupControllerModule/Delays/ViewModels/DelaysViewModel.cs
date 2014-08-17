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
	public class DelaysViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public DelaysViewModel()
		{
			Menu = new DelaysMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Delays = XManager.Delays == null ? new ObservableCollection<DelayViewModel>() : new ObservableCollection<DelayViewModel>(
				from delay in XManager.Delays
				orderby delay.Name
				select new DelayViewModel(delay));
			SelectedDelay = Delays.FirstOrDefault();
		}

		ObservableCollection<DelayViewModel> _delays;
		public ObservableCollection<DelayViewModel> Delays
		{
			get { return _delays; }
			set
			{
				_delays = value;
				OnPropertyChanged(() => Delays);
			}
		}

		DelayViewModel _selectedDelay;
		public DelayViewModel SelectedDelay
		{
			get { return _selectedDelay; }
			set
			{
				_selectedDelay = value;
				OnPropertyChanged("SelectedDelay");
			}
		}

		public bool HasSelectedDelay
		{
			get { return SelectedDelay != null; }
		}

		bool CanEditDelete()
		{
			return SelectedDelay != null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var delayDetailsViewModel = new DelayDetailsViewModel();
			if (DialogService.ShowModalWindow(delayDetailsViewModel))
			{
				XManager.Delays.Add(delayDetailsViewModel.Delay);
				var delayViewModel = new DelayViewModel(delayDetailsViewModel.Delay);
				Delays.Add(delayViewModel);
				SelectedDelay = delayViewModel;
				OnPropertyChanged("HasSelectedDelay");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить задержку " + SelectedDelay.Delay.Name);
			if (dialogResult == MessageBoxResult.Yes)
			{
				XManager.Delays.Remove(SelectedDelay.Delay);
				Delays.Remove(SelectedDelay);
				SelectedDelay = Delays.FirstOrDefault();
				OnPropertyChanged(() => HasSelectedDelay);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var delayDetailsViewModel = new DelayDetailsViewModel(SelectedDelay.Delay);
			if (DialogService.ShowModalWindow(delayDetailsViewModel))
			{
				SelectedDelay.Delay = delayDetailsViewModel.Delay;
				SelectedDelay.Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public void Select(Guid delayUID)
		{
			if (delayUID != Guid.Empty)
				SelectedDelay = Delays.FirstOrDefault(x => x.Delay.BaseUID == delayUID);
		}

		public override void OnShow()
		{
			base.OnShow();
			SelectedDelay = SelectedDelay;
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