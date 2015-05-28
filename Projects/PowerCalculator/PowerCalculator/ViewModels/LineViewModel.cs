using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;
using PowerCalculator.Processor;

namespace PowerCalculator.ViewModels
{
	public class LineViewModel : BaseViewModel
	{
		public LineViewModel(Line line)
		{
			Line = line;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in line.Devices)
			{
				var deviceViewModel = new DeviceViewModel(device);
				Devices.Add(deviceViewModel);
			}
			SelectedDevice = Devices.FirstOrDefault();
			UpdateAddresses();
		}

		public Line Line { get; set; }
		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

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

				for (int i = 0; i < newDeviceViewModel.Count; i++)
				{
					var device = new Device();
					device.DriverType = newDeviceViewModel.SelectedDriver.DriverType;
					device.Cable.Resistivity = newDeviceViewModel.CableResistivity;
					device.Cable.Length = newDeviceViewModel.CableLenght;
					Line.Devices.Add(device);
					var deviceViewModel = new DeviceViewModel(device);
					Devices.Add(deviceViewModel);
				}
				SelectedDevice = Devices.LastOrDefault();
				UpdateAddresses();
			}
		}
		bool CanAdd()
		{
			return GetMaxAddress() < 255;
		}

		uint GetMaxAddress()
		{
			if (!Devices.Any())
				return 0;
			return Devices.LastOrDefault().Address;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			Line.Devices.Remove(SelectedDevice.Device);
			Devices.Remove(SelectedDevice);
			SelectedDevice = Devices.FirstOrDefault();
			UpdateAddresses();
		}
		bool CanRemove()
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
	}
}