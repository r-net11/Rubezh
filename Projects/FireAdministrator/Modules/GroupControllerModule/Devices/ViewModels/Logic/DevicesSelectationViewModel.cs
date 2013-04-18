using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System;

namespace GKModule.ViewModels
{
	public class DevicesSelectationViewModel : SaveCancelDialogViewModel
	{
		List<XDevice> SourceDevices;

		public DevicesSelectationViewModel(List<XDevice> devices, List<XDevice> sourceDevices = null)
		{
			Title = "Выбор устройства";
			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
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
				if (!device.Driver.IsGroupDevice)
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

		public RelayCommand<object> AddCommand { get; private set; }
		public IList SelectedAvailableDevices;
		void OnAdd(object parameter)
		{
			var index = AvailableDevices.IndexOf(SelectedAvailableDevice);

			SelectedAvailableDevices = (IList)parameter;
			var availabledeviceViewModels = new List<XDevice>();
			foreach (var availabledevice in SelectedAvailableDevices)
			{
				var availabledeviceViewModel = availabledevice as XDevice;
				if (availabledeviceViewModel != null)
					availabledeviceViewModels.Add(availabledeviceViewModel);
			}
			foreach (var availabledeviceViewModel in availabledeviceViewModels)
			{
				Devices.Add(availabledeviceViewModel);
				AvailableDevices.Remove(availabledeviceViewModel);
			}
			SelectedDevice = Devices.LastOrDefault();
			OnPropertyChanged("AvailableDevices");

			index = Math.Min(index, AvailableDevices.Count - 1);
			if (index > -1)
				SelectedAvailableDevice = AvailableDevices[index];
		}

		public RelayCommand<object> RemoveCommand { get; private set; }
		public IList SelectedDevices;
		void OnRemove(object parameter)
		{
			var index = Devices.IndexOf(SelectedDevice);

			SelectedDevices = (IList)parameter;
			var deviceViewModels = new List<XDevice>();
			foreach (var device in SelectedDevices)
			{
				var deviceViewModel = device as XDevice;
				if (deviceViewModel != null)
					deviceViewModels.Add(deviceViewModel);
			}
			foreach (var deviceViewModel in deviceViewModels)
			{
				AvailableDevices.Add(deviceViewModel);
				Devices.Remove(deviceViewModel);
			}
			SelectedAvailableDevice = AvailableDevices.LastOrDefault();
			OnPropertyChanged("Devices");

			index = Math.Min(index, Devices.Count - 1);
			if (index > -1)
				SelectedDevice = Devices[index];
		}

		public bool CanAdd(object parameter)
		{
			return SelectedAvailableDevice != null;
		}

		public bool CanRemove(object parameter)
		{
			return SelectedDevice != null;
		}

		public bool CanAddAll()
		{
			return (AvailableDevices.Count > 0);
		}

		public bool CanRemoveAll()
		{
			return (Devices.Count > 0);
		}


		public RelayCommand AddAllCommand { get; private set; }
		void OnAddAll()
		{
			foreach (var device in AvailableDevices)
			{
				Devices.Add(device);
			}
			AvailableDevices.Clear();
			SelectedDevice = Devices.FirstOrDefault();
		}


		public RelayCommand RemoveAllCommand { get; private set; }
		void OnRemoveAll()
		{
			foreach (var device in Devices)
			{
				AvailableDevices.Add(device);
			}
			Devices.Clear();
			SelectedAvailableDevice = AvailableDevices.FirstOrDefault();
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