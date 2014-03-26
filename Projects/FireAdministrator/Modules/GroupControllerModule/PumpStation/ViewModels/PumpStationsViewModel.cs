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
	public class PumpStationsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public PumpStationsViewModel()
		{
			Menu = new PumpStationsMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			ChangePumpDevicesCommand = new RelayCommand(OnChangePumpDevices);
			DeletePumpDeviceCommand = new RelayCommand(OnDeletePumpDevice, CanDeletePumpDevice);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			PumpStations = XManager.PumpStations == null ? new ObservableCollection<PumpStationViewModel>() : new ObservableCollection<PumpStationViewModel>(
				from pumpStation in XManager.PumpStations
				orderby pumpStation.Name
				select new PumpStationViewModel(pumpStation));
			SelectedPumpStation = PumpStations.FirstOrDefault();
		}

		ObservableCollection<PumpStationViewModel> _pumpStations;
		public ObservableCollection<PumpStationViewModel> PumpStations
		{
			get { return _pumpStations; }
			set
			{
				_pumpStations = value;
				OnPropertyChanged("PumpStations");
			}
		}

		PumpStationViewModel _selectedPumpStation;
		public PumpStationViewModel SelectedPumpStation
		{
			get { return _selectedPumpStation; }
			set
			{
				_selectedPumpStation = value;
				OnPropertyChanged("SelectedPumpStation");
			}
		}

		public bool HasSelectedPumpStation
		{
			get { return SelectedPumpStation != null; }
		}

		bool CanEditDelete()
		{
			return SelectedPumpStation != null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var pumpStationDetailsViewModel = new PumpStationDetailsViewModel();
			if (DialogService.ShowModalWindow(pumpStationDetailsViewModel))
			{
				XManager.PumpStations.Add(pumpStationDetailsViewModel.PumpStation);
				var pumpStationViewModel = new PumpStationViewModel(pumpStationDetailsViewModel.PumpStation);
				PumpStations.Add(pumpStationViewModel);
				SelectedPumpStation = pumpStationViewModel;
				OnPropertyChanged("HasSelectedPumpStation");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить НС " + SelectedPumpStation.PumpStation.Name);
			if (dialogResult == MessageBoxResult.Yes)
			{
				XManager.PumpStations.Remove(SelectedPumpStation.PumpStation);
				PumpStations.Remove(SelectedPumpStation);
				SelectedPumpStation = PumpStations.FirstOrDefault();
				OnPropertyChanged("HasSelectedPumpStation");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var pumpStationDetailsViewModel = new PumpStationDetailsViewModel(SelectedPumpStation.PumpStation);
			if (DialogService.ShowModalWindow(pumpStationDetailsViewModel))
			{
				SelectedPumpStation.PumpStation = pumpStationDetailsViewModel.PumpStation;
				SelectedPumpStation.Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand ChangePumpDevicesCommand { get; private set; }
		void OnChangePumpDevices()
		{
			SelectedPumpStation.ChangePumpDevices();
		}

		public RelayCommand DeletePumpDeviceCommand { get; private set; }
		void OnDeletePumpDevice()
		{
			SelectedPumpStation.DeletePumpDevice();
		}
		bool CanDeletePumpDevice()
		{
			return SelectedPumpStation != null && SelectedPumpStation.SelectedPumpDevice != null;
		}

		public void Select(Guid pumpStationUID)
		{
			if (pumpStationUID != Guid.Empty)
				SelectedPumpStation = PumpStations.FirstOrDefault(x => x.PumpStation.BaseUID == pumpStationUID);
		}

		public override void OnShow()
		{
			base.OnShow();
			SelectedPumpStation = SelectedPumpStation;
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