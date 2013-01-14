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
	[SaveSize]
	public class NewDeviceViewModel : SaveCancelDialogViewModel
	{
		public DeviceViewModel CreatedDeviceViewModel { get; private set; }
		DeviceViewModel _parentDeviceViewModel;
		Device _parent;

		public NewDeviceViewModel(DeviceViewModel parent)
		{
			Title = "Новые устройства";
			_parentDeviceViewModel = parent;
			_parent = _parentDeviceViewModel.Device;
			AvailableShleifs = new ObservableCollection<int>();

			Drivers = new ObservableCollection<Driver>(
				from Driver driver in FiresecManager.Drivers
				where (_parent.Driver.AvaliableChildren.Contains(driver.UID))
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
				UpdateAddressRange();
				UpdateShleif();
				OnPropertyChanged("SelectedDriver");
			}
		}

		void UpdateShleif()
		{
			SelectedShleif = 0;
			AvailableShleifs.Clear();
			if (_parent != null)
			{
				var parentShleif = _parent;
				if (_parent.Driver.DriverType == DriverType.MRK_30 || _parent.Driver.DriverType == DriverType.MPT)
					parentShleif = _parent.Parent;
				for (int i = 0; i < parentShleif.Driver.ShleifCount; i++)
				{
					AvailableShleifs.Add(i + 1);
				}
				if (_parent.Driver.DriverType == DriverType.MRK_30)
				{
					AvailableShleifs.Clear();
					AvailableShleifs.Add(_parent.IntAddress / 256);
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

		void UpdateAddressRange()
		{
			int maxAddress = NewDeviceHelper.GetMinAddress(SelectedDriver, _parent);
			StartAddress = maxAddress % 256;
		}

		void CreateDevices()
		{
			if (SelectedDriver.HasAddress == false)
			{
				Device device = FiresecManager.FiresecConfiguration.AddDevice(_parent, SelectedDriver, 0);
				CreatedDeviceViewModel = NewDeviceHelper.AddDevice(device, _parentDeviceViewModel);
				return;
			}

			//int startAddress = AddressConverter.StringToIntAddress(SelectedDriver, StartAddress);
			int startAddress = SelectedShleif * 256 + StartAddress;
			int endAddress = startAddress + Count * GetReserverCount();
			if (SelectedDriver.MaxAddress > 0)
			{
				if (endAddress > SelectedDriver.MaxAddress)
					endAddress = SelectedDriver.MaxAddress + 1;
			}
			for (int i = startAddress; i < endAddress; i++)
			{
				if (_parent.Children.Any(x => x.IntAddress == i))
				{
					MessageBoxService.ShowWarning("В заданном диапазоне уже существуют устройства");
					return;
				}
			}

			if (_parent.Driver.IsChildAddressReservedRange)
			{
				Count = Math.Min(Count, _parent.GetReservedCount());
			}

			int shleifNo = startAddress / 256;
			for (int i = 0; i < Count; i++)
			{
				var address = startAddress + i * GetReserverCount();
				if (address + GetReserverCount() - 1 >= (shleifNo + 1) * 256)
				{
					return;
				}
				if (SelectedDriver.MaxAddress > 0)
				{
					if (address > SelectedDriver.MaxAddress)
						return;
				}

				Device device = FiresecManager.FiresecConfiguration.AddDevice(_parent, SelectedDriver, address);
				CreatedDeviceViewModel = NewDeviceHelper.AddDevice(device, _parentDeviceViewModel);
			}
		}

		int GetReserverCount()
		{
			if (SelectedDriver.IsChildAddressReservedRange)
			{
				int reservedCount = SelectedDriver.ChildAddressReserveRangeCount;
				if (SelectedDriver.DriverType == DriverType.MRK_30)
					reservedCount = 30;
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
			_parentDeviceViewModel.Update();
			return base.Save();
		}
	}
}