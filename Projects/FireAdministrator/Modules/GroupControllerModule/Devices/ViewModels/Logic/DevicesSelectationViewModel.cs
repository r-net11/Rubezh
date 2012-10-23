using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DevicesSelectationViewModel : SaveCancelDialogViewModel
	{
		List<XDevice> SourceDevices;

		public DevicesSelectationViewModel(List<XDevice> devices, List<XDevice> sourceDevices = null)
		{
			Title = "Выбор устройства";
			AddOneCommand = new RelayCommand(OnAddOne, CanAddOne);
			RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemoveOne);
			AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
			RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);

			if (sourceDevices != null)
				SourceDevices = sourceDevices;
			else
				SourceDevices = XManager.DeviceConfiguration.Devices;
			DevicesList = new List<XDevice>(devices);
			UpdateDevices();
		}

		void UpdateDevices()
		{
			AvailableDevices = new ObservableCollection<XDevice>();
			Devices = new ObservableCollection<XDevice>();
			SelectedAvailableDevice = null;
			SelectedDevice = null;

			foreach (var device in SourceDevices)
			{
				if (device.Driver.IsDeviceOnShleif && !device.Driver.IsGroupDevice)
				{
					if (DevicesList.Contains(device))
						Devices.Add(device);
					else
						AvailableDevices.Add(device);
				}
			}

			SelectedDevice = Devices.FirstOrDefault();
			SelectedAvailableDevice = AvailableDevices.FirstOrDefault();

			OnPropertyChanged("Devices");
			OnPropertyChanged("AvailableDevices");
		}

        public List<XDevice> DevicesList { get; private set; }
		public ObservableCollection<XDevice> AvailableDevices { get; private set; }
		public ObservableCollection<XDevice> Devices { get; private set; }

		XDevice _selectedAvailableDevice;
		public XDevice SelectedAvailableDevice
		{
			get { return _selectedAvailableDevice; }
			set
			{
				_selectedAvailableDevice = value;
				OnPropertyChanged("SelectedAvailableDevice");
			}
		}

		XDevice _selectedDevice;
		public XDevice SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged("SelectedDevice");
			}
		}

		public bool CanAddOne()
		{
			return (SelectedAvailableDevice != null);
		}

		public bool CanAddAll()
		{
			return (AvailableDevices.Count > 0);
		}

		public bool CanRemoveOne()
		{
			return (SelectedDevice != null);
		}

		public bool CanRemoveAll()
		{
			return (Devices.Count > 0);
		}

		public RelayCommand AddOneCommand { get; private set; }
		void OnAddOne()
		{
			DevicesList.Add(SelectedAvailableDevice);
			UpdateDevices();
		}

		public RelayCommand AddAllCommand { get; private set; }
		void OnAddAll()
		{
			foreach (var device in AvailableDevices)
			{
				DevicesList.Add(device);
			}
			UpdateDevices();
		}

		public RelayCommand RemoveOneCommand { get; private set; }
		void OnRemoveOne()
		{
			DevicesList.Remove(SelectedDevice);
			UpdateDevices();
		}

		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			DevicesList.Clear();
			UpdateDevices();
		}

		protected override bool Save()
		{
			DevicesList = new List<XDevice>();
			foreach (var device in Devices)
			{
				DevicesList.Add(device);
			}
			return base.Save();
		}
	}
}