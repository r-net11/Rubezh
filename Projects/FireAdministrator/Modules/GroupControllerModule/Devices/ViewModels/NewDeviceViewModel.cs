using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class NewDeviceViewModel : NewDeviceViewModelBase
	{
		public NewDeviceViewModel(DeviceViewModel deviceViewModel)
			: base(deviceViewModel)
		{
			foreach (var driver in XManager.DriversConfiguration.XDrivers)
			{
				if (driver.DriverType == XDriverType.AM1_O || driver.DriverType == XDriverType.AMP_1)
					continue;
				if (ParentDevice.Driver.Children.Contains(driver.DriverType))
					Drivers.Add(driver);
			}

			if (deviceViewModel.Driver.DriverType == XDriverType.MPT)
				Drivers = new ObservableCollection<XDriver>(
					from XDriver driver in XManager.DriversConfiguration.XDrivers
					where driver.DriverType == XDriverType.MPT
					select driver);

			SelectedDriver = Drivers.FirstOrDefault();
		}

		XDriver _selectedDriver;
		public XDriver SelectedDriver
		{
			get { return _selectedDriver; }
			set
			{
				_selectedDriver = value;
				UpdateShleif();
				OnPropertyChanged("SelectedDriver");
			}
		}

		void UpdateShleif()
		{
			AvailableShleifs.Clear();
			if (ParentDevice != null)
			{
				var parentShleif = ParentDevice;
				if (ParentDevice.Driver.DriverType == XDriverType.MPT)
					parentShleif = ParentDevice.Parent;
				if (parentShleif.Driver.IsKauOrRSR2Kau)
				{
					for (byte i = 1; i <= 8; i++)
					{
						AvailableShleifs.Add(i);
					}
				}
				else
				{
					AvailableShleifs.Add(1);
				}
			}
			SelectedShleif = AvailableShleifs.FirstOrDefault();
		}

		byte _selectedShleif;
		public byte SelectedShleif
		{
			get { return _selectedShleif; }
			set
			{
				_selectedShleif = value;
				OnPropertyChanged("SelectedShleif");
				UpdateAddressRange();
			}
		}

		int _startAddress;
		public int StartAddress
		{
			get { return _startAddress; }
			set
			{
				if (_startAddress != value)
				{
					_startAddress = value;
					OnPropertyChanged("StartAddress");
				}
			}
		}

		void UpdateAddressRange()
		{
			int maxAddress = NewDeviceHelper.GetMinAddress(SelectedDriver, ParentDevice, SelectedShleif);
			StartAddress = (byte)(maxAddress % 256);
		}

		bool CreateDevices()
		{
			var step = Math.Max(SelectedDriver.GroupDeviceChildrenCount, (byte)1);

			for (int i = StartAddress; i < StartAddress + Count * step; i++)
			{
				if (ParentDevice.Children.Any(x => x.IntAddress == i && x.ShleifNo == SelectedShleif))
				{
					MessageBoxService.ShowWarning("В заданном диапазоне уже существуют устройства");
					return false;
				}
			}

			if (ParentDevice.Driver.IsGroupDevice)
			{
				Count = Math.Min(Count, ParentDevice.Driver.GroupDeviceChildrenCount);
			}

			for (int i = 0; i < Count; i++)
			{
				var address = StartAddress + i * step;
				if (address + SelectedDriver.GroupDeviceChildrenCount >= 256)
				{
					return true;
				}

				XDevice device = XManager.AddChild(ParentDevice, SelectedDriver, SelectedShleif, (byte)address);
				AddedDevice = NewDeviceHelper.AddDevice(device, ParentDeviceViewModel);
			}
			return true;
		}

		protected override bool CanSave()
		{
			return (SelectedDriver != null);
		}

		protected override bool Save()
		{
			var result = CreateDevices();
			if (result)
			{
				ParentDeviceViewModel.Update();
				XManager.DeviceConfiguration.Update();
			}
			return result;
		}
	}
}