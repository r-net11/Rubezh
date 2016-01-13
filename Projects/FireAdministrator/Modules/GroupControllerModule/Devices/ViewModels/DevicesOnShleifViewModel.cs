using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class DevicesOnShleifViewModel : DialogViewModel
	{
		private List<GKDevice> _devicesToCopy { get; set; }
		public GKDevice ShleifDevice { get; private set; }
		public ObservableCollection<DeviceOnShleifViewModel> Devices { get; private set; }

		public DevicesOnShleifViewModel(GKDevice shleifDevice)
		{
			Title = "Выбор устройств на " + shleifDevice.PresentationName;
			ShleifDevice = shleifDevice;
			SelectAllCommand = new RelayCommand(OnSelectAll);
			DeSelectAllCommand = new RelayCommand(OnDeSelectAll);
			CopyCommand = new RelayCommand(OnCopy);
			RemoveCommand = new RelayCommand(OnRemove);
			CutCommand = new RelayCommand(OnCut);
			_devicesToCopy = new List<GKDevice>();

			Devices = new ObservableCollection<DeviceOnShleifViewModel>();
			foreach (var device in shleifDevice.Children)
			{
				var deviceOnShleifViewModel = new DeviceOnShleifViewModel(device);
				Devices.Add(deviceOnShleifViewModel);
			}
		}
		public RelayCommand CutCommand { get; set; }
		void OnCut()
		{
			_devicesToCopy.Clear();
			foreach (var device in Devices)
			{
				if (device.IsActive)
				{
					_devicesToCopy.Add(GKManager.CopyDevice(device.Device, true));
				}
			}
			DevicesViewModel.Current.DevicesToCopy = _devicesToCopy;
			OnRemove();
		}
		public RelayCommand SelectAllCommand { get; private set; }
		void OnSelectAll()
		{
			foreach (var device in Devices)
			{
				device.IsActive = true;
			}
		}

		public RelayCommand DeSelectAllCommand { get; private set; }
		void OnDeSelectAll()
		{
			foreach (var device in Devices)
			{
				device.IsActive = false;
			}
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			_devicesToCopy.Clear();
			foreach (var device in Devices)
			{
				if (device.IsActive)
				{
					_devicesToCopy.Add(GKManager.CopyDevice(device.Device, false));
				}
			}
			DevicesViewModel.Current.DevicesToCopy = _devicesToCopy;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			foreach (var deviceOnShleif in Devices)
			{
				var deviceViewModel = DevicesViewModel.Current.AllDevices.Find(x => x.Device.UID == deviceOnShleif.Device.UID);
				if (deviceViewModel != null && deviceOnShleif.IsActive)
				{
						deviceViewModel.Remove(false);
				}
			}
			if (ShleifDevice.KAUParent != null)
				GKManager.RebuildRSR2Addresses(ShleifDevice);
			Devices = new ObservableCollection<DeviceOnShleifViewModel>(Devices.Where(x => !x.IsActive));
			OnPropertyChanged(() => Devices);
		}
	}

	public class DeviceOnShleifViewModel : BaseViewModel
	{
		public GKDevice Device { get; private set; }

		public DeviceOnShleifViewModel(GKDevice device)
		{
			Device = device;
		}

		bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				_isActive = value;
				OnPropertyChanged(() => IsActive);
			}
		}
	}
}