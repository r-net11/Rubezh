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
			CopyCommand = new RelayCommand<object>(OnCopy, CanCutCopy);
			CutCommand = new RelayCommand<object>(OnCut, CanCutCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);

			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in line.Devices)
			{
				var deviceViewModel = new DeviceViewModel(device, this);
				Devices.Add(deviceViewModel);
			}
			SelectedDevice = Devices.FirstOrDefault();
			UpdateAddresses();
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

        public bool IsCircular
        {
            get { return Line.IsCircular; }
            set 
            {
                Line.IsCircular = value;
                OnPropertyChanged(() => IsCircular);
                Calculate();
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

        public void ChangeDriver(DeviceViewModel deviceViewModel, DriverType newDriverType)
        {
            int maxAdress = (int)GetMaxAddress();
            if (maxAdress - deviceViewModel.Device.Driver.Mult + Processor.DriversHelper.GetDriver(newDriverType).Mult > 255)
            {
                MessageBoxService.ShowWarning("Количество устройств на шлейфе не должно превышать 255");
                return;
            }
            deviceViewModel.Device.DriverType = newDriverType;
            UpdateAddresses(Devices.IndexOf(deviceViewModel));
            Calculate();
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

            HasError = Devices.Any(x => x.HasIError || x.HasUError);
        }

        public List<int> GetPatch()
        {
            return Processor.Processor.GetLinePatch(Line);
        }

        public void InstallPatch(IEnumerable<int> patch)
        {
            foreach(int index in patch)
            {
                var supplier = new Device();
                supplier.DriverType = DriverType.RSR2_MP; 
                supplier.Cable.CableType = Devices[index].CableType;
                supplier.Cable.Resistivity = Devices[index].CableResistivity;
                supplier.Cable.Length = Devices[index].CableLength;

                Line.Devices.Insert(index, supplier);
                Devices.Insert(index, new DeviceViewModel(supplier, this));
            }
            UpdateAddresses();
            Calculate();
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
                DeviceViewModel deviceViewModel = null;
                for (int i = 0; i < newDeviceViewModel.Count; i++)
				{
					var device = new Device();
                    device.DriverType = newDeviceViewModel.SelectedDriver.DriverType;
                    device.Cable.CableType = newDeviceViewModel.SelectedCableType;
                    device.Cable.Resistivity = newDeviceViewModel.CableResistivity;
					device.Cable.Length = newDeviceViewModel.CableLength;
					Line.Devices.Insert(selectedIndex, device);
					deviceViewModel = new DeviceViewModel(device, this);
					Devices.Insert(selectedIndex, deviceViewModel);
				}
                SelectedDevice = deviceViewModel;
                UpdateAddresses(selectedIndex - 1);
                Calculate();
			}
		}
		bool CanAdd()
		{
			return GetMaxAddress() <= 255;
		}

		uint GetMaxAddress()
		{
            return Line.MaxAdress;
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
				Line.Devices.Remove(deviceViewModel.Device);
				Devices.Remove(deviceViewModel);
			}

			devicesIndex = Math.Min(devicesIndex, Devices.Count - 1);
			if (devicesIndex > -1)
				SelectedDevice = Devices[devicesIndex];

            UpdateAddresses(devicesIndex - 1);
            Calculate();
		}
		bool CanRemove(object parameter)
		{
			return SelectedDevice != null;
		}

		void UpdateAddresses(int startIndex = 0)
		{
            if (startIndex >= Devices.Count)
                return;
            if (startIndex < 0)
                startIndex = 0;

            var currentAddress = startIndex == 0 ? 1 : Devices[startIndex].Address;
            
            for (int i = startIndex; i < Devices.Count; i++ )
            {
                Devices[i].Address = currentAddress;
                currentAddress += Devices[i].Device.Driver.Mult;
            }
		}

		#region CopyPaste
		static List<Device> DevicesToCopy = new List<Device>();

		bool CanCutCopy(object parameter)
		{
			return SelectedDevice != null;
		}

		public RelayCommand<object> CopyCommand { get; private set; }
		void OnCopy(object parameter)
		{
			DevicesToCopy = new List<Device>();

			IList selectedDevices = (IList)parameter;
			var deviceViewModels = new List<DeviceViewModel>();
			foreach (var device in selectedDevices)
			{
				var deviceViewModel = device as DeviceViewModel;
				if (deviceViewModel != null)
					DevicesToCopy.Add(deviceViewModel.Device);
			}
		}

		public RelayCommand<object> CutCommand { get; private set; }
		void OnCut(object parameter)
		{
			DevicesToCopy = new List<Device>();

			IList selectedDevices = (IList)parameter;
            var firstIndex = selectedDevices.Count == 0 ? 0 : Devices.IndexOf(selectedDevices[0] as DeviceViewModel) - 1;
			var deviceViewModels = new List<DeviceViewModel>();
			foreach (var device in selectedDevices)
			{
				var deviceViewModel = device as DeviceViewModel;
				if (deviceViewModel != null)
				{
					DevicesToCopy.Add(deviceViewModel.Device);
					Line.Devices.Remove(deviceViewModel.Device);
				}
			}
			foreach (var deviceToCopy in DevicesToCopy)
			{
                var deviceViewModel = Devices.FirstOrDefault(x => x.Device == deviceToCopy);
				if(deviceViewModel != null)
				{
					Devices.Remove(deviceViewModel);
				}
			}
            UpdateAddresses(firstIndex);
            Calculate();
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var maxAddress = GetMaxAddress();
			var newMaxAddress = maxAddress + DevicesToCopy.Sum(x => x.Driver.Mult);
			if (newMaxAddress > 255)
			{
				MessageBoxService.ShowWarning("Количество устройств на шлейфе не должно превышать 255");
				return;
			}
            var firstIndex = Devices.IndexOf(SelectedDevice);
			foreach (var deviceToCopy in DevicesToCopy)
			{
				var selectedIndex = Devices.IndexOf(SelectedDevice);
                if (selectedIndex < 0) selectedIndex = 0;

				var device = new Device();
				device.DriverType = deviceToCopy.DriverType;
                device.Cable.CableType = deviceToCopy.Cable.CableType;
				device.Cable.Resistivity = deviceToCopy.Cable.Resistivity;
				device.Cable.Length = deviceToCopy.Cable.Length;
				Line.Devices.Insert(selectedIndex, device);
				var deviceViewModel = new DeviceViewModel(device, this);
				Devices.Insert(selectedIndex, deviceViewModel);
				SelectedDevice = deviceViewModel;
			}
			UpdateAddresses(firstIndex - 1);
            Calculate();
		}
		bool CanPaste()
		{
			return DevicesToCopy.Count > 0;
		}
		#endregion

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