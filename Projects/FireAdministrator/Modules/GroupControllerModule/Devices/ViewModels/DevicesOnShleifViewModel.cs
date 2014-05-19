using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DevicesOnShleifViewModel : DialogViewModel
	{
		public XDevice ShleifDevice { get; private set; }

		public DevicesOnShleifViewModel(XDevice shleifDevice)
		{
			Title = "Выбор устройств на шлейфе " + shleifDevice.PresentationName;
			ShleifDevice = shleifDevice;
			SelectAllCommand = new RelayCommand(OnSelectAll);
			DeSelectAllCommand = new RelayCommand(OnDeSelectAll);
			CopyCommand = new RelayCommand(OnCopy);
			RemoveCommand = new RelayCommand(OnRemove);

			Devices = new ObservableCollection<DeviceOnShleifViewModel>();
			foreach (var device in shleifDevice.Children)
			{
				var deviceOnShleifViewModel = new DeviceOnShleifViewModel(device);
				Devices.Add(deviceOnShleifViewModel);
			}
		}

		public List<XDevice> CopyDevices { get; private set; }
		public ObservableCollection<DeviceOnShleifViewModel> Devices { get; private set; }

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
			var devicesToCopy = new List<XDevice>();
			foreach (var device in Devices)
			{
				if (device.IsActive)
				{
					devicesToCopy.Add(device.Device);
				}
			}
			DevicesViewModel.Current.DevicesToCopy = devicesToCopy;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			foreach (var deviceOnShleif in Devices)
			{
				if (deviceOnShleif.IsActive)
				{
					var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == deviceOnShleif.Device);
					if (deviceViewModel != null)
					{
						deviceViewModel.RemoveCommand.Execute();
					}
				}
			}
			Devices = new ObservableCollection<DeviceOnShleifViewModel>(Devices.Where(x => !x.IsActive));
			OnPropertyChanged("Devices");
		}
	}

	public class DeviceOnShleifViewModel : BaseViewModel
	{
		public XDevice Device { get; private set; }

		public DeviceOnShleifViewModel(XDevice device)
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
				OnPropertyChanged("IsActive");
			}
		}
	}
}