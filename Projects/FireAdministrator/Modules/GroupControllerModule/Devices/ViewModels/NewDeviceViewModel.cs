﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows;

namespace GKModule.ViewModels
{
	public class NewDeviceViewModel : NewDeviceViewModelBase
	{
		public NewDeviceViewModel(DeviceViewModel deviceViewModel)
			: base(deviceViewModel)
		{
			var sortedDrivers = SortDrivers();
			foreach (var driver in sortedDrivers)
			{
				if (driver.IsIgnored)
					continue;

				if (ParentDevice.Driver.Children.Contains(driver.DriverType))
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
			int maxAddress = NewDeviceHelper.GetMinAddress(SelectedDriver, ParentDevice);
			StartAddress = (byte)(maxAddress % 256);
		}

		bool CreateDevices()
		{
			var step = Math.Max(SelectedDriver.GroupDeviceChildrenCount, (byte)1);

			for (int i = StartAddress; i < StartAddress + Count * step; i++)
			{
				if (ParentDevice.Children.Any(x => x.Driver.HasAddress && x.IntAddress == i))
				{
					MessageBoxService.ShowWarning("В заданном диапазоне уже существуют устройства");
					return false;
				}
			}

			if (ParentDevice.Driver.IsGroupDevice)
			{
				Count = Math.Min(Count, ParentDevice.Driver.GroupDeviceChildrenCount);
			}

			AddedDevices = new List<DeviceViewModel>();
			for (int i = 0; i < Count; i++)
			{
				var address = StartAddress + i * step;
				if (address + SelectedDriver.GroupDeviceChildrenCount >= 256)
				{
					return true;
				}

				GKDevice device = GKManager.AddChild(ParentDevice, null, SelectedDriver, (byte)address);
				var addedDevice = NewDeviceHelper.AddDevice(device, ParentDeviceViewModel);
				AddedDevices.Add(addedDevice);
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
				GKManager.DeviceConfiguration.Update();
			}
			return result;
		}
	}
}