using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows;

namespace GKModule.ViewModels
{
	public class RSR2NewDeviceViewModel : NewDeviceViewModelBase
	{
		GKDevice RealParentDevice;

		public RSR2NewDeviceViewModel(DeviceViewModel deviceViewModel)
			: base(deviceViewModel)
		{
			RealParentDevice = ParentDevice.MVPPartParent;
			if (RealParentDevice == null)
				RealParentDevice = ParentDevice.KAURSR2ShleifParent;

			var sortedDrivers = SortDrivers();
			foreach (var driver in sortedDrivers)
			{
				if (!driver.IsIgnored && RealParentDevice.Driver.Children.Contains(driver.DriverType))
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
			}
		}

		bool CreateDevices()
		{
			AddedDevices = new List<DeviceViewModel>();
			var allChildren = RealParentDevice.AllChildren;
			int maxAddressOnShleif = 0;
			if (allChildren.Count > 0)
				maxAddressOnShleif = allChildren.Max(x => x.IntAddress);
			if (maxAddressOnShleif + Count * Math.Max(1, (int)SelectedDriver.GroupDeviceChildrenCount) > 255)
			{
				MessageBoxService.ShowWarningExtended("При добавлении количество устройств на шлейфе будет превышать 255");
				return false;
			}

			for (int i = 0; i < Count; i++)
			{
				if (RealParentDevice == ParentDevice)
				{
					GKDevice device = GKManager.AddChild(ParentDevice, null, SelectedDriver, 0);
					var addedDevice = NewDeviceHelper.AddDevice(device, ParentDeviceViewModel);
					AddedDevices.Add(addedDevice);
				}
				else
				{
					GKDevice device = GKManager.AddChild(RealParentDevice, ParentDevice, SelectedDriver, 0);
					var addedDevice = NewDeviceHelper.InsertDevice(device, ParentDeviceViewModel);
					AddedDevices.Add(addedDevice);
				}
			}
			GKManager.RebuildRSR2Addresses(ParentDevice.KAURSR2Parent);
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