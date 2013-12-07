using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure.Common.Windows;

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

			CopyDevices = new List<XDevice>();
			Devices = new ObservableCollection<DeviceOnShleifViewModel>();
			foreach (var device in XManager.GetAllDeviceChildren(shleifDevice))
			{
				if (device.IsRealDevice)
				{
					var deviceOnShleifViewModel = new DeviceOnShleifViewModel(device);
					Devices.Add(deviceOnShleifViewModel);
				}
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
			CopyDevices = new List<XDevice>();
			foreach (var device in Devices)
			{
				if (device.IsActive)
				{
					CopyDevices.Add(device.Device);
				}
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			// Group devices, mpt, mro
			foreach (var deviceOnShleif in Devices)
			{
				if (deviceOnShleif.IsActive)
				{
					if (deviceOnShleif.Device.Parent.Driver.IsGroupDevice)
					{
						foreach (var childDevice in deviceOnShleif.Device.Children)
						{
							var device = Devices.FirstOrDefault(x => x.Device == childDevice);
							if (device != null)
							{
								if (!device.IsActive)
								{
									MessageBoxService.ShowError("Групповое устройство " + deviceOnShleif.Device.Parent.PredefinedName + " можно удалить только пометив все его дочерние устройста");
									return;
								}
							}
						}
					}
				}
			}

			foreach (var device in Devices)
			{
				if (device.IsActive)
				{
					var deviceViewModel = DevicesViewModel.Current.AllDevices.First(x => x.Device == device.Device);
					if (deviceViewModel != null)
					{
						deviceViewModel.RemoveCommand.Execute();
					}
					//XManager.RemoveDevice(device.Device);
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