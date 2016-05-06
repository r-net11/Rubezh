using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Common;
using DeviceControls;
using StrazhAPI.GK;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
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
			var devicesToRemove = new List<SKDLibraryDevice>();
			foreach (var libraryDevice in SKDManager.SKDLibraryConfiguration.Devices)
			{
				var driver = SKDManager.Drivers.FirstOrDefault(x => x.UID == libraryDevice.DriverId);
				if (driver != null)
				{
					libraryDevice.Driver = driver;
				}
				else
				{
					devicesToRemove.Add(libraryDevice);
					Logger.Error("SKDLibraryViewModel.Initialize driver = null " + libraryDevice.DriverId.ToString());
				}
			}
			foreach (var libraryXDevice in devicesToRemove)
			{
				SKDManager.SKDLibraryConfiguration.Devices.RemoveAll(x => x == libraryXDevice);
			}
			if (devicesToRemove.Count > 0)
				ServiceFactory.SaveService.SKDLibraryChanged = true;
			var devices = from SKDLibraryDevice libraryDevice in SKDManager.SKDLibraryConfiguration.Devices.Where(x => x.Driver != null) orderby libraryDevice.Driver.Name select libraryDevice;
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
					var driver = SKDManager.Drivers.FirstOrDefault(x => x.UID == SelectedDevice.LibraryDevice.DriverId);
					States = new ObservableCollection<StateViewModel>();
					var libraryStates = from SKDLibraryState skdLibraryState in SelectedDevice.LibraryDevice.States orderby skdLibraryState.StateClass descending select skdLibraryState;
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
			var libraryDeviceDetailsViewModel = new LibraryDeviceDetailsViewModel();
			if (DialogService.ShowModalWindow(libraryDeviceDetailsViewModel))
			{
				SKDManager.SKDLibraryConfiguration.Devices.Add(libraryDeviceDetailsViewModel.SelectedDevice.LibraryDevice);
				Devices.Add(libraryDeviceDetailsViewModel.SelectedDevice);
				SelectedDevice = Devices.LastOrDefault();
				ServiceFactory.SaveService.SKDLibraryChanged = true;
			}
		}

		public RelayCommand RemoveDeviceCommand { get; private set; }
		void OnRemoveDevice()
		{
			SKDManager.SKDLibraryConfiguration.Devices.Remove(SelectedDevice.LibraryDevice);
			Devices.Remove(SelectedDevice);
			SelectedDevice = Devices.FirstOrDefault();
			ServiceFactory.SaveService.SKDLibraryChanged = true;
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
			var libraryStateDetailsViewModel = new LibraryStateDetailsViewModel(SelectedDevice.LibraryDevice);
			if (DialogService.ShowModalWindow(libraryStateDetailsViewModel))
			{
				SelectedDevice.LibraryDevice.States.Add(libraryStateDetailsViewModel.SelectedState.State);
				States.Add(libraryStateDetailsViewModel.SelectedState);
				SelectedState = States.LastOrDefault();
				ServiceFactory.SaveService.SKDLibraryChanged = true;
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
			ServiceFactory.SaveService.SKDLibraryChanged = true;
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
	}
}