using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class RSR2NewDeviceViewModel : NewDeviceViewModelBase
	{
		XDevice RealParentDevice;

		public RSR2NewDeviceViewModel(DeviceViewModel deviceViewModel)
			: base(deviceViewModel)
		{
			RealParentDevice = ParentDevice.KAURSR2ShleifParent;

			var sortedDrivers = SortDrivers();
			foreach (var driver in sortedDrivers)
			{
				if (RealParentDevice.Driver.Children.Contains(driver.DriverType))
					Drivers.Add(driver);
			}

			if (RealParentDevice != null)
			{
				SelectedShleif = RealParentDevice.IntAddress;
			}
			else
			{
				SelectedShleif = 1;
			}

			SelectedDriver = Drivers.FirstOrDefault();
		}

		byte SelectedShleif;

		XDriver _selectedDriver;
		public XDriver SelectedDriver
		{
			get { return _selectedDriver; }
			set
			{
				_selectedDriver = value;
				OnPropertyChanged("SelectedDriver");
			}
		}

		bool CreateDevices()
		{
			for (int i = 0; i < Count; i++)
			{
				if (ParentDevice.Driver.DriverType == XDriverType.RSR2_KAU_Shleif)
				{
					var maxAddressOnShleif = 0;
					if (ParentDevice.Children.Count > 0)
					{
						maxAddressOnShleif = ParentDevice.Children.Max(x => x.IntAddress + Math.Max(0, x.Driver.GroupDeviceChildrenCount - 1));
					}
					maxAddressOnShleif += 1;

					if (maxAddressOnShleif + Math.Min(0, SelectedDriver.GroupDeviceChildrenCount - 1) > 255)
					{
						return true;
					}

					XDevice device = XManager.AddChild(ParentDevice, SelectedDriver, SelectedShleif, (byte)maxAddressOnShleif);
					AddedDevice = NewDeviceHelper.AddDevice(device, ParentDeviceViewModel);
				}
				else if (ParentDevice.Parent != null && ParentDevice.Parent.Driver.DriverType == XDriverType.RSR2_KAU_Shleif)
				{
					var maxPreviousAddress = ParentDevice.IntAddress + Math.Max(0, ParentDevice.Driver.GroupDeviceChildrenCount - 1) + 1;
					if(maxPreviousAddress > 255)
					{
						return true;
					}
					XDevice device = XManager.InsertChild(RealParentDevice, ParentDevice, SelectedDriver, SelectedShleif, (byte)maxPreviousAddress);
					AddedDevice = NewDeviceHelper.InsertDevice(device, ParentDeviceViewModel);
				}
			}
			XManager.RebuildRSR2Addresses(ParentDevice.KAURSR2Parent);
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