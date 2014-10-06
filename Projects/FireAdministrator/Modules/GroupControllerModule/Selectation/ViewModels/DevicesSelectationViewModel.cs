using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class DevicesSelectationViewModel : SaveCancelDialogViewModel
	{
		List<GKDevice> SourceDevices;

		public DevicesSelectationViewModel(List<GKDevice> devices, List<GKDevice> sourceDevices = null)
		{
			Title = "Выбор устройства";
			AddCommand = new RelayCommand<object>(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
			AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
			RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);

			if (sourceDevices != null)
				SourceDevices = sourceDevices;
			else
				SourceDevices = GKManager.Devices;
			DevicesList = new List<GKDevice>(devices);
			UpdateDevices();
		}

		void UpdateDevices()
		{
			AvailableDevices = new ObservableCollection<GKDevice>();
			Devices = new ObservableCollection<GKDevice>();
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

			OnPropertyChanged(() => Devices);
			OnPropertyChanged(() => AvailableDevices);
		}

		public List<GKDevice> DevicesList { get; private set; }
		public ObservableCollection<GKDevice> AvailableDevices { get; private set; }
		public ObservableCollection<GKDevice> Devices { get; private set; }

		GKDevice _selectedAvailableDevice;
		public GKDevice SelectedAvailableDevice
		{
			get { return _selectedAvailableDevice; }
			set
			{
				_selectedAvailableDevice = value;
				OnPropertyChanged("SelectedAvailableDevice");
			}
		}

		GKDevice _selectedDevice;
		public GKDevice SelectedDevice
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
			var availabledeviceViewModels = new List<GKDevice>();
			foreach (var availabledevice in SelectedAvailableDevices)
			{
				var availabledeviceViewModel = availabledevice as GKDevice;
				if (availabledeviceViewModel != null)
					availabledeviceViewModels.Add(availabledeviceViewModel);
			}
			foreach (var availabledeviceViewModel in availabledeviceViewModels)
			{
				Devices.Add(availabledeviceViewModel);
				AvailableDevices.Remove(availabledeviceViewModel);
			}
			SelectedDevice = Devices.LastOrDefault();
			OnPropertyChanged(() => AvailableDevices);

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
			var deviceViewModels = new List<GKDevice>();
			foreach (var device in SelectedDevices)
			{
				var deviceViewModel = device as GKDevice;
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
			DevicesList = new List<GKDevice>();
			foreach (var device in Devices)
			{
				DevicesList.Add(device);
			}
			return base.Save();
		}
	}
}