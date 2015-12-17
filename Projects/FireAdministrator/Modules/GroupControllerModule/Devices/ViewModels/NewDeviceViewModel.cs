﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using System;
using Infrastructure;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class NewDeviceViewModel : SaveCancelDialogViewModel
	{
		public NewDeviceViewModel(DeviceViewModel deviceViewModel)
		{
			Title = "Добавление устройства";
			ParentDeviceViewModel = deviceViewModel;
			ParentDevice = ParentDeviceViewModel.Device;
			AddedDevices = new List<DeviceViewModel>();
			if (ParentDevice.IsConnectedToKAU)
			{
				RealParentDevice = ParentDevice.MVPPartParent == null ? ParentDevice.KAUShleifParent : ParentDevice.MVPPartParent;
				Drivers = new ObservableCollection<GKDriver>(SortDrivers().Where(x => RealParentDevice.Driver.Children.Contains(x.DriverType)));
				SelectedDriver = Drivers.FirstOrDefault();
				MinAddress = 1;
				MaxAddress = 255;
			}
			else
			{
				Drivers = new ObservableCollection<GKDriver>(SortDrivers().Where(x => ParentDevice.Driver.Children.Contains(x.DriverType)));
				SelectedDriver = Drivers.FirstOrDefault();
				MinAddress = SelectedDriver.MinAddress;
				MaxAddress = SelectedDriver.MaxAddress;
			}

			Count = 1;
		}
		GKDevice RealParentDevice;
		DeviceViewModel ParentDeviceViewModel;
		GKDevice ParentDevice;
		public List<DeviceViewModel> AddedDevices { get; private set; }
		public ObservableCollection<GKDriver> Drivers { get; private  set; }
		public int MaxAddress { get; private set; }
		public int MinAddress { get; private set; }
		public bool AddInStartlList { get; set; }

		GKDriver _selectedDriver;
		public GKDriver SelectedDriver
		{
			get { return _selectedDriver; }
			set
			{
				_selectedDriver = value;
				OnPropertyChanged(() => SelectedDriver);
			}
		}

		int _count;
		public int Count
		{
			get { return _count; }
			set
			{
				_count = value;
				OnPropertyChanged(() => Count);
			}
		}

		public List<GKDriver> SortDrivers()
		{
			var driverCounters = new List<DriverCounter>();
			foreach (var driver in GKManager.Drivers)
			{
				var driverCounter = new DriverCounter()
				{
					Driver = driver,
					Count = 0
				};
				driverCounters.Add(driverCounter);
			}
			foreach (var device in GKManager.Devices)
			{
				var driverCounter = driverCounters.FirstOrDefault(x => x.Driver == device.Driver);
				if (driverCounter != null)
				{
					driverCounter.Count++;
				}
			}
			var sortedDrivers = driverCounters.OrderByDescending(x => x.Count).Select(x => x.Driver);
			return sortedDrivers.ToList();
		}

		public bool ShowCheckBox { get { return ParentDevice.DriverType == GKDriverType.RSR2_KAU_Shleif || ParentDevice.DriverType == GKDriverType.RSR2_MVP_Part; } }
		public bool CreateDevices()
		{
			AddedDevices = new List<DeviceViewModel>();
			int startAddress;
			if (RealParentDevice!= null && RealParentDevice.DriverType == GKDriverType.RSR2_MVP_Part)
				startAddress = GKManager.GetAddress(RealParentDevice.KAUShleifParent.AllChildren);
			else
			 startAddress = RealParentDevice == null ? GKManager.GetAddress(ParentDevice.Children.Where(x=> x.Driver.HasAddress)) : GKManager.GetAddress(RealParentDevice.AllChildren);

			if (RealParentDevice != null)
			{
				if (Count * Math.Max(1, (int)SelectedDriver.GroupDeviceChildrenCount) + startAddress > 255)
				{
					ServiceFactory.MessageBoxService.ShowWarning("При добавлении количество устройств на АЛС максимально допустимое значения в 255");
					return false;
				}
			}
			else
			{
				if (Count + startAddress > SelectedDriver.MaxAddress && SelectedDriver.HasAddress)
				{
					ServiceFactory.MessageBoxService.ShowWarning("При добавлении устройств количество будет превышать максимально допустимое значения в " + SelectedDriver.MaxAddress.ToString());
					return false;
				}
			}

			for (int i = 1; i <= Count; i++)
			{
				var address = RealParentDevice == null ? startAddress + i : 0;
				if (RealParentDevice == null || RealParentDevice == ParentDevice)
				{
					GKDevice device = GKManager.AddDevice(ParentDevice, SelectedDriver, address, AddInStartlList ? 0 : (int?)null);
					AddedDevices.Add(NewDeviceHelper.AddDevice(device, ParentDeviceViewModel, isStartList: AddInStartlList));
				}
				else
				{
					var index = RealParentDevice.Children.IndexOf(ParentDevice) + 1;
					GKDevice device = GKManager.AddDevice(RealParentDevice, SelectedDriver, address, index);
					var addedDevice = NewDeviceHelper.AddDevice(device, ParentDeviceViewModel, false);
					AddedDevices.Insert(0, addedDevice);
				}
			}
			if (RealParentDevice != null)
				GKManager.RebuildRSR2Addresses(ParentDevice);

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

		class DriverCounter
		{
			public GKDriver Driver { get; set; }
			public int Count { get; set; }
		}
	}
}