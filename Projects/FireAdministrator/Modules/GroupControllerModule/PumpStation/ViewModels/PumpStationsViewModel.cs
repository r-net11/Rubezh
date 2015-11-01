using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Common;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace GKModule.ViewModels
{
	public class PumpStationsViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public PumpStationsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			ChangePumpDevicesCommand = new RelayCommand(OnChangePumpDevices, CanChangePumpDevices);
			DeletePumpDeviceCommand = new RelayCommand(OnDeletePumpDevice, CanDeletePumpDevice);
			DeleteAllEmptyCommand = new RelayCommand(OnDeleteAllEmpty, CanDeleteAllEmpty);
			ShowDependencyItemsCommand = new RelayCommand(ShowDependencyItems);

			Menu = new PumpStationsMenuViewModel(this);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			PumpStations = GKManager.PumpStations == null ? new ObservableCollection<PumpStationViewModel>() : new ObservableCollection<PumpStationViewModel>(
				from pumpStation in GKManager.PumpStations
				orderby pumpStation.No
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
				OnPropertyChanged(() => SelectedPumpStation);
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
				GKManager.AddPumpStation(pumpStationDetailsViewModel.PumpStation);
				var pumpStationViewModel = new PumpStationViewModel(pumpStationDetailsViewModel.PumpStation);
				PumpStations.Add(pumpStationViewModel);
				SelectedPumpStation = pumpStationViewModel;
				OnPropertyChanged(() => HasSelectedPumpStation);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var pumpStationDetailsViewModel = new PumpStationDetailsViewModel(SelectedPumpStation.PumpStation);
			if (DialogService.ShowModalWindow(pumpStationDetailsViewModel))
			{
				GKManager.EditPumpStation(SelectedPumpStation.PumpStation);
				SelectedPumpStation.Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить НС " + SelectedPumpStation.PumpStation.PresentationName + " ?"))
			{
				var pumpDevices = new List<GKDevice>(SelectedPumpStation.PumpDevices.Select(x => x.Device));
				var index = PumpStations.IndexOf(SelectedPumpStation);
				GKManager.RemovePumpStation(SelectedPumpStation.PumpStation);
				PumpStations.Remove(SelectedPumpStation);
				index = Math.Min(index, PumpStations.Count - 1);
				if (index > -1)
					SelectedPumpStation = PumpStations[index];
			
				OnPropertyChanged(() => HasSelectedPumpStation);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteAllEmptyCommand { get; private set; }
		void OnDeleteAllEmpty()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить все пустые НС ?"))
			{

				var emptyPumpStations = PumpStations.Where(x => !x.PumpStation.StopLogic.GetObjects().Any() && !x.PumpStation.StartLogic.GetObjects().Any() && !x.PumpStation.NSDevices.Any() && !x.PumpStation.AutomaticOffLogic.GetObjects().Any());
				if (emptyPumpStations.Any())
				{
					
					for (var i = emptyPumpStations.Count()-1; i >= 0; i--)
					{
						GKManager.RemovePumpStation(emptyPumpStations.ElementAt(i).PumpStation);
						PumpStations.Remove(emptyPumpStations.ElementAt(i));
					}

					SelectedPumpStation = PumpStations.FirstOrDefault();
					ServiceFactory.SaveService.GKChanged = true;		
				}
			}
		}
		bool CanDeleteAllEmpty()
		{
			return PumpStations.Any(x => !x.PumpStation.StopLogic.GetObjects().Any() && !x.PumpStation.StartLogic.GetObjects().Any() && !x.PumpStation.NSDevices.Any() && !x.PumpStation.AutomaticOffLogic.GetObjects().Any());
		}

		public RelayCommand ChangePumpDevicesCommand { get; private set; }
		void OnChangePumpDevices()
		{
			SelectedPumpStation.ChangePumpDevices();
		}
		bool CanChangePumpDevices()
		{
			return SelectedPumpStation != null;
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

		public RelayCommand ShowDependencyItemsCommand { get; set; }

		void ShowDependencyItems()
		{
			if (SelectedPumpStation.PumpStation != null)
			{
				var dependencyItemsViewModel = new DependencyItemsViewModel(SelectedPumpStation.PumpStation.OutDependentElements);
				DialogService.ShowModalWindow(dependencyItemsViewModel);
			}
		}

		public void Select(Guid pumpStationUID)
		{
			if (pumpStationUID != Guid.Empty)
				SelectedPumpStation = PumpStations.FirstOrDefault(x => x.PumpStation.UID == pumpStationUID);
		}

		public override void OnShow()
		{
			base.OnShow();
			SelectedPumpStation = SelectedPumpStation;
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
					new RibbonMenuItemViewModel("Удалить все пустые НС", DeleteAllEmptyCommand, "BDeleteEmpty"),
				}, "BEdit") { Order = 2 }
			};
		}
	}
}