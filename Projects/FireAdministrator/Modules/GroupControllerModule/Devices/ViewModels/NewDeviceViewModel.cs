using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows;

namespace GKModule.ViewModels
{
	public class NewDeviceViewModel : NewDeviceViewModelBase
	{
		public NewDeviceViewModel(DeviceViewModel deviceViewModel)
			: base(deviceViewModel)
		{
			foreach (var driver in SortDrivers().Where(x => ParentDevice.Driver.Children.Contains(x.DriverType)))
			{
					Drivers.Add(driver);
			}

			SelectedDriver = Drivers.FirstOrDefault();
		}

		GKDriver _selectedDriver;
		public GKDriver SelectedDriver
		{
			get { return _selectedDriver; }
			set
			{
				_selectedDriver = value;
				OnPropertyChanged(() => SelectedDriver);
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
					OnPropertyChanged(() => StartAddress);
				}
			}
		}

		void UpdateAddressRange()
		{
			var children = ParentDevice.Children.Where(x => x.Driver.HasAddress).ToList();
			if (children.Count > 0)
				StartAddress = children.Max(x => x.IntAddress) + 1;
			else
			StartAddress = 1;	
		}

		bool CreateDevices()
		{
			if ((StartAddress-1) + Count > SelectedDriver.MaxAddress && SelectedDriver.HasAddress)
			{
				MessageBoxService.ShowWarning("При добавлении устройств количество будет превышать максимально допустимое значения в " + SelectedDriver.MaxAddress.ToString());
				return false;
			}

			for (int i = StartAddress; i < StartAddress + Count; i++)
			{
				if (ParentDevice.Children.Any(x => x.Driver.HasAddress && x.IntAddress == i))
				{
					MessageBoxService.ShowWarning("В заданном диапазоне уже существуют устройства");
					return false;
				}
			}
			
			AddedDevices = new List<DeviceViewModel>();

				for (int i = 0; i < Count; i++)
				{
					var address = StartAddress + i;
					if (address > SelectedDriver.MaxAddress && SelectedDriver.HasAddress)
					{
						return true;
					}

					GKDevice device = GKManager.AddChild(ParentDevice, null, SelectedDriver, address);
					var addedDevice = NewDeviceHelper.AddDevice(device, ParentDeviceViewModel);
					AddedDevices.Add(addedDevice);
				}

			return true;
		}

		protected override bool CanSave()
		{
			return SelectedDriver != null;
		}

		protected override bool Save()
		{
			var result = CreateDevices();
			if (result)
			{
				ParentDeviceViewModel.Update();
				GKManager.DeviceConfiguration.Update();
			}
			return result;
		}
	}
}