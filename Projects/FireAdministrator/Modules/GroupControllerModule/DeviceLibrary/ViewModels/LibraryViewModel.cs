using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Common;
using DeviceControls;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using System.Diagnostics;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class LibraryViewModel : ViewPartViewModel
	{
		public LibraryViewModel()
		{
			AddDeviceCommand = new RelayCommand(OnAddDevice);
			RemoveDeviceCommand = new RelayCommand(OnRemoveDevice, CanRemoveDevice);
			AddStateCommand = new RelayCommand(OnAddState, CanAddState);
			RemoveStateCommand = new RelayCommand(OnRemoveState, CanRemoveState);
			Current = this;
			var devicesToRemove = new List<GKLibraryDevice>();
			foreach (var libraryDevice in GKManager.DeviceLibraryConfiguration.GKDevices)
			{
				var driver = GKManager.Drivers.FirstOrDefault(x => x.UID == libraryDevice.DriverUID);
				if (driver != null)
				{
					libraryDevice.Driver = driver;
				}
				else
				{
					devicesToRemove.Add(libraryDevice);
					Logger.Error("LibraryViewModel.Initialize driver = null " + libraryDevice.DriverUID.ToString());
				}
			}
			foreach (var libraryDevice in devicesToRemove)
			{
				GKManager.DeviceLibraryConfiguration.GKDevices.RemoveAll(x => x == libraryDevice);
			}
			if (devicesToRemove.Count > 0)
				ServiceFactory.SaveService.GKLibraryChanged = true;
			var devices = from GKLibraryDevice libraryDevice in GKManager.DeviceLibraryConfiguration.GKDevices.Where(x => x.Driver != null) orderby libraryDevice.Driver.DeviceClassName select libraryDevice;
			Devices = new ObservableCollection<LibraryDeviceViewModel>();
			foreach (var device in devices)
			{
				var deviceViewModel = new LibraryDeviceViewModel(device);
				Devices.Add(deviceViewModel);
			}
			SelectedDevice = Devices.FirstOrDefault();
		}

		public static LibraryViewModel Current { get; private set; }

		ObservableCollection<LibraryDeviceViewModel> _devices;
		public ObservableCollection<LibraryDeviceViewModel> Devices
		{
			get { return _devices; }
			set
			{
				_devices = value;
				OnPropertyChanged(() => Devices);
			}
		}

		LibraryDeviceViewModel _selectedDevice;
		public LibraryDeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				var oldSelectedStateClass = XStateClass.No;
				if (SelectedState != null)
				{
					oldSelectedStateClass = SelectedState.State.StateClass;
				}
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);

				if (value != null)
				{
					var driver = GKManager.Drivers.FirstOrDefault(x => x.UID == SelectedDevice.LibraryDevice.DriverUID);
					States = new ObservableCollection<StateViewModel>();
					var libraryStates = from GKLibraryState libraryState in SelectedDevice.LibraryDevice.States orderby libraryState.StateClass descending select libraryState;
					foreach (var libraryState in libraryStates)
					{
						var stateViewModel = new StateViewModel(libraryState, driver);
						States.Add(stateViewModel);
					}
					SelectedState = States.FirstOrDefault(x => x.State.StateClass == oldSelectedStateClass);
					if (SelectedState == null)
						SelectedState = States.FirstOrDefault();
				}
				else
				{
					SelectedState = null;
				}
			}
		}

		public RelayCommand AddDeviceCommand { get; private set; }
		void OnAddDevice()
		{
			var deviceDetailsViewModel = new DeviceDetailsViewModel();
			if (DialogService.ShowModalWindow(deviceDetailsViewModel))
			{
				GKManager.DeviceLibraryConfiguration.GKDevices.Add(deviceDetailsViewModel.SelectedDevice.LibraryDevice);
				Devices.Add(deviceDetailsViewModel.SelectedDevice);
				SelectedDevice = Devices.LastOrDefault();
				ServiceFactory.SaveService.GKLibraryChanged = true;
			}
		}

		public RelayCommand RemoveDeviceCommand { get; private set; }
		void OnRemoveDevice()
		{
			GKManager.DeviceLibraryConfiguration.GKDevices.Remove(SelectedDevice.LibraryDevice);
			Devices.Remove(SelectedDevice);
			SelectedDevice = Devices.FirstOrDefault();
			ServiceFactory.SaveService.GKLibraryChanged = true;
		}
		bool CanRemoveDevice()
		{
			return SelectedDevice != null;
		}

		ObservableCollection<StateViewModel> _states;
		public ObservableCollection<StateViewModel> States
		{
			get { return _states; }
			set
			{
				_states = value;
				OnPropertyChanged(() => States);
			}
		}

		StateViewModel _selectedState;
		public StateViewModel SelectedState
		{
			get { return _selectedState; }
			set
			{
				_selectedState = value;
				OnPropertyChanged(() => SelectedState);
				OnPropertyChanged(() => PreviewBrush);
			}
		}

		public RelayCommand AddStateCommand { get; private set; }
		void OnAddState()
		{
			var stateDetailsViewModel = new StateDetailsViewModel(SelectedDevice.LibraryDevice);
			if (DialogService.ShowModalWindow(stateDetailsViewModel))
			{
				SelectedDevice.LibraryDevice.States.Add(stateDetailsViewModel.SelectedState.State);
				States.Add(stateDetailsViewModel.SelectedState);
				SelectedState = States.LastOrDefault();
				ServiceFactory.SaveService.GKLibraryChanged = true;
			}
		}
		bool CanAddState()
		{
			return SelectedDevice != null;
		}

		public RelayCommand RemoveStateCommand { get; private set; }
		void OnRemoveState()
		{
			SelectedDevice.LibraryDevice.States.Remove(SelectedState.State);
			States.Remove(SelectedState);
			SelectedState = States.FirstOrDefault();
			ServiceFactory.SaveService.GKLibraryChanged = true;
		}
		bool CanRemoveState()
		{
			return (SelectedState != null && SelectedState.State.StateClass != XStateClass.No);
		}

		public bool IsPreviewEnabled { get; private set; }
		public Brush PreviewBrush
		{
			get
			{
				var brush = (Brush)Brushes.Transparent;
				if (SelectedDevice != null && SelectedState != null)
					brush = PictureCacheSource.CreateDynamicBrush(SelectedState.State.Frames);
				IsPreviewEnabled = brush != null && brush != Brushes.Transparent;
				OnPropertyChanged(() => IsPreviewEnabled);
				return brush;
			}
		}
		public void InvalidatePreview()
		{
			OnPropertyChanged(() => SelectedState);
			OnPropertyChanged(() => PreviewBrush);
		}

		public bool IsDebug
		{
			get { return GlobalSettingsHelper.GlobalSettings.IsDebug; }
		}
	}
}