using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Diagnostics;

namespace DevicesModule.ViewModels
{
	public class NewDeviceViewModel : SaveCancelDialogViewModel
	{
		DeviceViewModel _parentDeviceViewModel;
		Device _parent;

		public NewDeviceViewModel(DeviceViewModel parent)
		{
			Title = "Новые устройства";
			_parentDeviceViewModel = parent;
			_parent = _parentDeviceViewModel.Device;

			Drivers = new List<Driver>(
				from Driver driver in FiresecManager.Drivers
				where (_parent.Driver.AvaliableChildren.Contains(driver.UID))
				select driver);

			SelectedDriver = Drivers.FirstOrDefault();
			Count = 1;
		}

		public List<Driver> Drivers { get; private set; }

		Driver _selectedDriver;
		public Driver SelectedDriver
		{
			get { return _selectedDriver; }
			set
			{
				_selectedDriver = value;
				UpdateAddressRange();
				OnPropertyChanged("SelectedDriver");
			}
		}

		Device _startDevice;
		public Device StartDevice
		{
			get { return _startDevice; }
			set
			{
				_startDevice = value;
				OnPropertyChanged("StartDevice");
			}
		}

		string _startAddress;
		public string StartAddress
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

			StartDevice = new Device()
			{
				Driver = SelectedDriver,
				IntAddress = maxAddress,
				Parent = _parent
			};
			StartAddress = StartDevice.EditingPresentationAddress;
		}

		void CreateDevices()
		{
			if (SelectedDriver.HasAddress == false)
			{
				Device device = _parent.AddChild(SelectedDriver, 0);
				NewDeviceHelper.AddDevice(device, _parentDeviceViewModel);
				return;
			}

			int startAddress = AddressConverter.StringToIntAddress(SelectedDriver, StartAddress);

			for (int i = startAddress; i <= startAddress + Count * GetReserverCount(); i++)
			{
				if (_parent.Children.Any(x => x.IntAddress == i))
				{
					MessageBoxService.ShowWarning("В заданном диапазоне уже сущеспвуют устройства");
					return;
				}
			}

			int shleifNo = startAddress / 256;

			for (int i = 0; i < Count; i++)
			{
				int address = NewDeviceHelper.GetMinAddress(SelectedDriver, _parent);
				if (address + GetReserverCount() - 1 >= (shleifNo + 1)*256)
				{
					return;
				}
				Device device = _parent.AddChild(SelectedDriver, address);
				NewDeviceHelper.AddDevice(device, _parentDeviceViewModel);
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
			FiresecManager.DeviceConfiguration.Update();
			return base.Save();
		}
	}
}