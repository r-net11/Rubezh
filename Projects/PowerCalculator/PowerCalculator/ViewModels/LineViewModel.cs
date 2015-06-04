using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;
using PowerCalculator.Processor;
using System.Collections;
using System.Collections.Generic;
using System;

namespace PowerCalculator.ViewModels
{
	public class LineViewModel : BaseViewModel
	{
        public LineViewModel(Line line)
		{
			Line = line;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove, CanRemove);
			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in line.Devices)
			{
				var deviceViewModel = new DeviceViewModel(device, this);
				Devices.Add(deviceViewModel);
			}
			SelectedDevice = Devices.FirstOrDefault();
			UpdateAddresses();
            Calculate();
            Devices.CollectionChanged += Devices_CollectionChanged;
		}

        void Devices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Calculate();
        }

		public Line Line { get; set; }
		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

        public string Name
        {
            get { return Line.Name; }
            set
            {
                Line.Name = value;
                OnPropertyChanged(() => Name);
            }
        }

		DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}

        public void Calculate()
        {
            var deviceIndicators = Processor.Processor.CalculateLine(Line).ToList();

            foreach (var deviceViewModel in Devices)
            {
                var deviceIndicator = deviceIndicators.FirstOrDefault(x => x.Device == deviceViewModel.Device);
                if (deviceIndicator != null)
                {
                    deviceViewModel.Current = deviceIndicator.I;
                    deviceViewModel.HasIError = deviceIndicator.HasIError;
                    deviceViewModel.Voltage = deviceIndicator.U;
                    deviceViewModel.HasUError = deviceIndicator.HasUError;
                }
                else
                {
                    deviceViewModel.Current = 0;
                    deviceViewModel.HasIError = false;
                    deviceViewModel.Voltage = 0;
                    deviceViewModel.HasUError = false;
                }
            }
        }

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var newDeviceViewModel = new NewDeviceViewModel();
			if (DialogService.ShowModalWindow(newDeviceViewModel))
			{
				var maxAddress = GetMaxAddress();
				var driver = DriversHelper.GetDriver(newDeviceViewModel.SelectedDriver.DriverType);
				var newMaxAddress = maxAddress + driver.Mult * newDeviceViewModel.Count;
				if (newMaxAddress > 255)
				{
					MessageBoxService.ShowWarning("Количество устройств на шлейфе не должно превышать 255");
					return;
				}

				var selectedIndex = Devices.IndexOf(SelectedDevice) + 1;
				for (int i = 0; i < newDeviceViewModel.Count; i++)
				{
					var device = new Device();
					device.DriverType = newDeviceViewModel.SelectedDriver.DriverType;
					device.Cable.Resistivity = newDeviceViewModel.CableResistivity;
					device.Cable.Length = newDeviceViewModel.CableLenght;
					Line.Devices.Insert(selectedIndex, device);
					var deviceViewModel = new DeviceViewModel(device, this);
					Devices.Insert(selectedIndex, deviceViewModel);
					SelectedDevice = deviceViewModel;
				}
				UpdateAddresses();
			}
		}
		bool CanAdd()
		{
			return SelectedDevice != null && GetMaxAddress() < 255;
		}

		uint GetMaxAddress()
		{
			if (!Devices.Any())
				return 0;
			return Devices.LastOrDefault().Address;
		}

		public RelayCommand<object> RemoveCommand { get; private set; }
		void OnRemove(object parameter)
		{
			var devicesIndex = Devices.IndexOf(SelectedDevice);

			IList selectedDevices = (IList)parameter;
			var deviceViewModels = new List<DeviceViewModel>();
			foreach (var device in selectedDevices)
			{
				var deviceViewModel = device as DeviceViewModel;
				if (deviceViewModel != null)
					deviceViewModels.Add(deviceViewModel);
			}
			foreach (var deviceViewModel in deviceViewModels)
			{
				if (deviceViewModel.Device.DriverType != DriverType.RSR2_KAU)
				{
					Line.Devices.Remove(deviceViewModel.Device);
					Devices.Remove(deviceViewModel);
				}
			}

			devicesIndex = Math.Min(devicesIndex, Devices.Count - 1);
			if (devicesIndex > -1)
				SelectedDevice = Devices[devicesIndex];

			UpdateAddresses();
		}
		bool CanRemove(object parameter)
		{
			return SelectedDevice != null && SelectedDevice.Device.DriverType != DriverType.RSR2_KAU;
		}

		void UpdateAddresses()
		{
			uint currentAddress = 1;
			foreach (var deviceViewModel in Devices)
			{
				deviceViewModel.Address = currentAddress;
				currentAddress += deviceViewModel.Device.Driver.Mult;
			}
		}

		bool _hasError;
		public bool HasError
		{
			get { return _hasError; }
			set
			{
				_hasError = value;
				OnPropertyChanged(() => HasError);
			}
		}
	}
}