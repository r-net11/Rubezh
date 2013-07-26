using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows.Media;
using DeviceControls;
using Infrastructure.ViewModels;

namespace LibraryModule.ViewModels
{
	public class LibraryViewModel : MenuViewPartViewModel
	{
		public LibraryViewModel()
		{
			Menu = new LibraryMenuViewModel(this);
			AddDeviceCommand = new RelayCommand(OnAddDevice);
			RemoveDeviceCommand = new RelayCommand(OnRemoveDevice, CanRemoveDevice);
			AddAdditionalDeviceCommand = new RelayCommand(OnAddAdditionalDevice, CanAddAdditionalDevice);
			AddStateCommand = new RelayCommand(OnAddState, CanAddState);
			RemoveStateCommand = new RelayCommand(OnRemoveState, CanRemoveState);
			AddDevicePresenterCommand = new RelayCommand(OnAddDevicePresenter, CanAddDevicePresenter);
			Current = this;
		}
		public static LibraryViewModel Current { get; private set; }
		public void Initialize()
		{
			foreach (var libraryDevice in FiresecManager.DeviceLibraryConfiguration.Devices)
			{
				var driver = FiresecClient.FiresecManager.Drivers.FirstOrDefault(x => x.UID == libraryDevice.DriverId);
				if (driver != null)
				{
					libraryDevice.Driver = driver;
				}
				else
				{
					Logger.Error("LibraryViewModel.Initialize driver = null " + libraryDevice.DriverId.ToString());
				}
			}
			FiresecManager.DeviceLibraryConfiguration.Devices.RemoveAll(x => x.Driver == null);

			var devices = from LibraryDevice libraryDevice in FiresecManager.DeviceLibraryConfiguration.Devices orderby libraryDevice.Driver.DeviceClassName select libraryDevice;
			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in devices)
			{
				var deviceViewModel = new DeviceViewModel(device);
				Devices.Add(deviceViewModel);
			}
			SelectedDevice = Devices.FirstOrDefault();
		}

		ObservableCollection<DeviceViewModel> _devices;
		public ObservableCollection<DeviceViewModel> Devices
		{
			get { return _devices; }
			set
			{
				_devices = value;
				OnPropertyChanged("Devices");
			}
		}

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				var oldSelectedStateType = StateType.No;
				if (SelectedState != null)
				{
					oldSelectedStateType = SelectedState.State.StateType;
				}
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");

				if (value != null)
				{
					var driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == SelectedDevice.LibraryDevice.DriverId);
					States = new ObservableCollection<StateViewModel>();
					var libraryStates = from LibraryState libraryState in SelectedDevice.States orderby libraryState.StateType descending select libraryState;
					foreach (var libraryState in libraryStates)
					{
						var stateViewModel = new StateViewModel(libraryState, driver);
						States.Add(stateViewModel);
					}
					SelectedState = States.FirstOrDefault(x => x.State.StateType == oldSelectedStateType);
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
			var addDeviceViewModel = new DeviceDetailsViewModel();
			if (DialogService.ShowModalWindow(addDeviceViewModel))
			{
				FiresecManager.DeviceLibraryConfiguration.Devices.Add(addDeviceViewModel.SelectedDevice.LibraryDevice);
				Devices.Add(addDeviceViewModel.SelectedDevice);
				SelectedDevice = Devices.LastOrDefault();
				ServiceFactory.SaveService.LibraryChanged = true;
			}
		}

		public RelayCommand RemoveDeviceCommand { get; private set; }
		void OnRemoveDevice()
		{
			if (SelectedDevice.Presenter == null)
			{
				FiresecManager.DeviceLibraryConfiguration.Devices.Remove(SelectedDevice.LibraryDevice);
				Devices.Remove(SelectedDevice);
				SelectedDevice = Devices.FirstOrDefault();
			}
			else
			{
				SelectedDevice.LibraryDevice.Presenters.Remove(SelectedDevice.Presenter);
				var parent = SelectedDevice.Parent;
				parent.RemoveChild(SelectedDevice);
				SelectedDevice = parent;
			}
			ServiceFactory.SaveService.LibraryChanged = true;
		}
		bool CanRemoveDevice()
		{
			return SelectedDevice != null;
		}

		public RelayCommand AddAdditionalDeviceCommand { get; private set; }
		void OnAddAdditionalDevice()
		{
			var libraryDevice = new LibraryDevice()
			{
				IsAlternative = true,
				Driver = SelectedDevice.LibraryDevice.Driver,
				DriverId = SelectedDevice.LibraryDevice.DriverId
			};
			var deviceViewModel = new DeviceViewModel(libraryDevice);

			FiresecManager.DeviceLibraryConfiguration.Devices.Add(libraryDevice);
			Devices.Add(deviceViewModel);
			SelectedDevice = Devices.LastOrDefault();
			ServiceFactory.SaveService.LibraryChanged = true;
		}
		bool CanAddAdditionalDevice()
		{
			return SelectedDevice != null && SelectedDevice.Presenter == null;
		}

		public RelayCommand AddDevicePresenterCommand { get; private set; }
		void OnAddDevicePresenter()
		{
			var presenterProperty = SelectedDevice.Driver.PresenterKeyProperty;
			if (presenterProperty != null)
			{
				var viewModel = new PresenterKeyViewModel(presenterProperty);
				if (DialogService.ShowModalWindow(viewModel))
				{
					if (SelectedDevice.LibraryDevice.Presenters == null)
						SelectedDevice.LibraryDevice.Presenters = new List<LibraryDevicePresenter>();
					var presenter = new LibraryDevicePresenter()
					{
						Key = viewModel.Value
					};
					SelectedDevice.LibraryDevice.Presenters.Add(presenter);
					var newDevice = new DeviceViewModel(SelectedDevice.LibraryDevice, presenter);
					SelectedDevice.AddChild(newDevice);
					SelectedDevice.IsExpanded = true;
					SelectedDevice = newDevice;
					ServiceFactory.SaveService.LibraryChanged = true;
				}
			}
		}
		bool CanAddDevicePresenter()
		{
			return SelectedDevice != null && SelectedDevice.Presenter == null && SelectedDevice.Driver.PresenterKeyProperty != null;
		}

		ObservableCollection<StateViewModel> _states;
		public ObservableCollection<StateViewModel> States
		{
			get { return _states; }
			set
			{
				_states = value;
				OnPropertyChanged("States");
			}
		}

		StateViewModel _selectedState;
		public StateViewModel SelectedState
		{
			get { return _selectedState; }
			set
			{
				_selectedState = value;
				OnPropertyChanged("SelectedState");
				OnPropertyChanged(() => PreviewBrush);
			}
		}

		public RelayCommand AddStateCommand { get; private set; }
		void OnAddState()
		{
			var stateDetailsViewModel = new StateDetailsViewModel(SelectedDevice);
			if (DialogService.ShowModalWindow(stateDetailsViewModel))
			{
				SelectedDevice.States.Add(stateDetailsViewModel.SelectedState.State);
				States.Add(stateDetailsViewModel.SelectedState);
				SelectedState = States.LastOrDefault();
				ServiceFactory.SaveService.LibraryChanged = true;
			}
		}
		bool CanAddState()
		{
			return SelectedDevice != null;
		}

		public RelayCommand RemoveStateCommand { get; private set; }
		void OnRemoveState()
		{
			SelectedDevice.States.Remove(SelectedState.State);
			States.Remove(SelectedState);
			SelectedState = States.FirstOrDefault();
			ServiceFactory.SaveService.LibraryChanged = true;
		}
		bool CanRemoveState()
		{
			return (SelectedState != null && SelectedState.State.StateType != StateType.No);
		}

		public bool IsPreviewEnabled { get; private set; }
		public Brush PreviewBrush
		{
			get
			{
				var brush = (Brush)Brushes.Transparent;
				if (SelectedDevice != null && SelectedState != null)
					brush = DevicePictureCache.CreatePreviewBrush(SelectedState.State.Frames);
				IsPreviewEnabled = brush != null && brush != Brushes.Transparent;
				OnPropertyChanged(() => IsPreviewEnabled);
				return brush;
			}
		}
		public void InvalidatePreview()
		{
			OnPropertyChanged(() => PreviewBrush);
		}

		public bool IsDebug
		{
			get { return GlobalSettingsHelper.GlobalSettings.IsDebug; }
		}
	}
}