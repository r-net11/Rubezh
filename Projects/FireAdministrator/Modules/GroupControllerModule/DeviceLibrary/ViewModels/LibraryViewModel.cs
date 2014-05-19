using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Common;
using DeviceControls;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

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
			var devicesToRemove = new List<LibraryXDevice>();
			foreach (var libraryXDevice in XManager.DeviceLibraryConfiguration.XDevices)
			{
				var driver = XManager.Drivers.FirstOrDefault(x => x.UID == libraryXDevice.XDriverId);
				if (driver != null)
				{
					libraryXDevice.Driver = driver;
				}
				else
				{
					{
						devicesToRemove.Add(libraryXDevice);
						Logger.Error("XLibraryViewModel.Initialize driver = null " + libraryXDevice.XDriverId.ToString());
					}
				}
			}
			foreach (var libraryXDevice in devicesToRemove)
			{
				XManager.DeviceLibraryConfiguration.XDevices.RemoveAll(x => x == libraryXDevice);
			}
			if (devicesToRemove.Count > 0)
				ServiceFactory.SaveService.XLibraryChanged = true;
			var devices = from LibraryXDevice libraryXDevice in XManager.DeviceLibraryConfiguration.XDevices.Where(x => x.Driver != null) orderby libraryXDevice.Driver.DeviceClassName select libraryXDevice;
			Devices = new ObservableCollection<XDeviceViewModel>();
			foreach (var device in devices)
			{
				var deviceViewModel = new XDeviceViewModel(device);
				Devices.Add(deviceViewModel);
			}
			SelectedDevice = Devices.FirstOrDefault();
		}

		public static LibraryViewModel Current { get; private set; }

		ObservableCollection<XDeviceViewModel> _devices;
		public ObservableCollection<XDeviceViewModel> Devices
		{
			get { return _devices; }
			set
			{
				_devices = value;
				OnPropertyChanged(() => Devices);
			}
		}

		XDeviceViewModel _selectedDevice;
		public XDeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				var oldSelectedStateClass = XStateClass.No;
				if (SelectedState != null)
				{
					oldSelectedStateClass = SelectedState.State.XStateClass;
				}
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);

				if (value != null)
				{
					var driver = XManager.Drivers.FirstOrDefault(x => x.UID == SelectedDevice.LibraryDevice.XDriverId);
					States = new ObservableCollection<StateViewModel>();
					var libraryStates = from LibraryXState libraryState in SelectedDevice.LibraryDevice.XStates orderby libraryState.XStateClass descending select libraryState;
					foreach (var libraryState in libraryStates)
					{
						var stateViewModel = new StateViewModel(libraryState, driver);
						States.Add(stateViewModel);
					}
					SelectedState = States.FirstOrDefault(x => x.State.XStateClass == oldSelectedStateClass);
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
				XManager.DeviceLibraryConfiguration.XDevices.Add(deviceDetailsViewModel.SelectedDevice.LibraryDevice);
				Devices.Add(deviceDetailsViewModel.SelectedDevice);
				SelectedDevice = Devices.LastOrDefault();
				ServiceFactory.SaveService.XLibraryChanged = true;
			}
		}

		public RelayCommand RemoveDeviceCommand { get; private set; }
		void OnRemoveDevice()
		{
			XManager.DeviceLibraryConfiguration.XDevices.Remove(SelectedDevice.LibraryDevice);
			Devices.Remove(SelectedDevice);
			SelectedDevice = Devices.FirstOrDefault();
			ServiceFactory.SaveService.XLibraryChanged = true;
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
				SelectedDevice.LibraryDevice.XStates.Add(stateDetailsViewModel.SelectedState.State);
				States.Add(stateDetailsViewModel.SelectedState);
				SelectedState = States.LastOrDefault();
				ServiceFactory.SaveService.XLibraryChanged = true;
			}
		}
		bool CanAddState()
		{
			return SelectedDevice != null;
		}

		public RelayCommand RemoveStateCommand { get; private set; }
		void OnRemoveState()
		{
			SelectedDevice.LibraryDevice.XStates.Remove(SelectedState.State);
			States.Remove(SelectedState);
			SelectedState = States.FirstOrDefault();
			ServiceFactory.SaveService.XLibraryChanged = true;
		}
		bool CanRemoveState()
		{
			return (SelectedState != null && SelectedState.State.XStateClass != XStateClass.No);
		}

		public bool IsPreviewEnabled { get; private set; }
		public Brush PreviewBrush
		{
			get
			{
				var brush = (Brush)Brushes.Transparent;
				if (SelectedDevice != null && SelectedState != null)
					brush = PictureCacheSource.CreateDynamicBrush(SelectedState.State.XFrames);
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