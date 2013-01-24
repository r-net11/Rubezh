using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace DevicesModule.ViewModels
{
	[SaveSizeAttribute]
	public class NewDeviceViewModel : SaveCancelDialogViewModel
	{
		public DeviceViewModel CreatedDeviceViewModel { get; private set; }
		DeviceViewModel ParentDeviceViewModel;
		Device ParentDevice;

		public NewDeviceViewModel(DeviceViewModel parent)
		{
			Title = "Новые устройства";
			ParentDeviceViewModel = parent;
			ParentDevice = ParentDeviceViewModel.Device;
			AvailableShleifs = new ObservableCollection<int>();

			Drivers = new ObservableCollection<Driver>(
				from Driver driver in FiresecManager.Drivers
				where (ParentDevice.Driver.AvaliableChildren.Contains(driver.UID))
				orderby driver.ShortName
				select driver);
			SelectedDriver = Drivers.FirstOrDefault();
			Count = 1;
		}

		public ObservableCollection<Driver> Drivers { get; private set; }

		Driver _selectedDriver;
		public Driver SelectedDriver
		{
			get { return _selectedDriver; }
			set
			{
				_selectedDriver = value;
				UpdateMinAddress();
				UpdateShleif();
				OnPropertyChanged("SelectedDriver");
			}
		}

		void UpdateShleif()
		{
			SelectedShleif = 0;
			AvailableShleifs.Clear();
			if (ParentDevice != null)
			{
				var parentShleif = ParentDevice;
				if (ParentDevice.Driver.DriverType == DriverType.MPT)
					parentShleif = ParentDevice.Parent;
				for (int i = 0; i < parentShleif.Driver.ShleifCount; i++)
				{
					AvailableShleifs.Add(i + 1);
				}
				if (ParentDevice.Driver.DriverType == DriverType.MRK_30)
				{
					AvailableShleifs.Clear();
					AvailableShleifs.Add(ParentDevice.IntAddress / 256);
				}
			}
			SelectedShleif = AvailableShleifs.FirstOrDefault();
		}

		public ObservableCollection<int> AvailableShleifs { get; private set; }

		int _selectedShleif;
		public int SelectedShleif
		{
			get { return _selectedShleif; }
			set
			{
				_selectedShleif = value;
				UpdateMinAddress();
				UpdateFreeAddresses();
				OnPropertyChanged("SelectedShleif");
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

		void UpdateFreeAddresses()
		{
			FreeAddresses = new ObservableCollection<int>();
			for (int i = 1; i <= 256; i++)
			{
				FreeAddresses.Add(i);
			}
			if (ParentDevice.Driver.DriverType == DriverType.MRK_30)
			{
				FreeAddresses = new ObservableCollection<int>();
				for (int i = ParentDevice.IntAddress % 256; i <= Math.Min(256, ParentDevice.IntAddress % 256 + 30); i++)
				{
					FreeAddresses.Add(i);
				}
			}
			RemoveBuisyAddress(ParentDevice);
		}

		void RemoveBuisyAddress(Device device)
		{
			foreach (var child in device.Children)
			{
				if (child.IntAddress / 256 == SelectedShleif)
				{
					FreeAddresses.Remove(child.IntAddress % 256);
					RemoveBuisyAddress(child);
				}
			}		
		}

		ObservableCollection<int> _freeAddresses;
		public ObservableCollection<int> FreeAddresses
		{
			get { return _freeAddresses; }
			set
			{
				_freeAddresses = value;
				OnPropertyChanged("FreeAddresses");
			}
		}

		int _selectedFreeAddress;
		public int SelectedFreeAddress
		{
			get { return _selectedFreeAddress; }
			set
			{
				_selectedFreeAddress = value;
				StartAddress = value;
				OnPropertyChanged("SelectedFreeAddress");
			}
		}

		int _count;
		public int Count
		{
			get { return _count; }
			set
			{
				_count = value;
				OnPropertyChanged("Count");
			}
		}

		void UpdateMinAddress()
		{
			int maxAddress = NewDeviceHelper.GetMinAddress(SelectedDriver, ParentDevice, SelectedShleif);
			StartAddress = maxAddress % 256;
		}

		void CreateDevices()
		{
			if (SelectedDriver.HasAddress == false)
			{
				Device device = FiresecManager.FiresecConfiguration.AddDevice(ParentDevice, SelectedDriver, 0);
				CreatedDeviceViewModel = NewDeviceHelper.AddDevice(device, ParentDeviceViewModel);
				return;
			}

			int startAddress = SelectedShleif * 256 + StartAddress;
			int endAddress = startAddress + Count * GetReserverCount();
			if (SelectedDriver.MaxAddress > 0)
			{
				if (endAddress > SelectedDriver.MaxAddress)
					endAddress = SelectedDriver.MaxAddress + 1;
			}
			for (int i = startAddress; i < endAddress; i++)
			{
				if (ParentDevice.Children.Any(x => x.IntAddress == i))
				{
					MessageBoxService.ShowWarning("В заданном диапазоне уже существуют устройства");
					return;
				}
			}

			if (ParentDevice.Driver.IsChildAddressReservedRange)
			{
				Count = Math.Min(Count, ParentDevice.GetReservedCount());
			}

			for (int i = 0; i < Count; i++)
			{
				var address = startAddress + i * GetReserverCount();
				if (address + GetReserverCount() - 1 >= (SelectedShleif + 1) * 256)
				{
					return;
				}
				if (SelectedDriver.MaxAddress > 0)
				{
					if (address > SelectedDriver.MaxAddress)
						return;
				}

				Device device = FiresecManager.FiresecConfiguration.AddDevice(ParentDevice, SelectedDriver, address);
				CreatedDeviceViewModel = NewDeviceHelper.AddDevice(device, ParentDeviceViewModel);
			}
		}

		int GetReserverCount()
		{
			if (SelectedDriver.IsChildAddressReservedRange)
			{
				int reservedCount = SelectedDriver.ChildAddressReserveRangeCount;
				if (SelectedDriver.DriverType == DriverType.MRK_30)
					reservedCount = 31;
				return reservedCount;
			}
			return 1;
		}

		protected override bool CanSave()
		{
			return (SelectedDriver != null);
		}

		protected override bool Save()
		{
			CreateDevices();
			ParentDeviceViewModel.Update();
			return base.Save();
		}
	}
}