﻿using Common;
using Infrastructure.Common.TreeList;

namespace StrazhModule.ViewModels
{
	public class DeviceViewModelDeviceComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			if (!x.Device.Driver.IsController || !y.Device.Driver.IsController) return 0;

			return string.Compare(x.Device.Name, y.Device.Name);
		}
	}
	public class DeviceViewModelAddressComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			System.Net.IPAddress firstIP;
			System.Net.IPAddress secondIP;

			if (System.Net.IPAddress.TryParse(x.Device.Address, out firstIP)
				&& System.Net.IPAddress.TryParse(y.Device.Address, out secondIP))
			{
				return firstIP.Compare(secondIP);
			}

			return 0;
		}
	}
	public class DeviceViewModelZoneComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			int result = string.Compare(x.EditingPresentationZone, y.EditingPresentationZone);
			if (result == 0)
				result = string.Compare(x.Driver.ShortName, y.Driver.ShortName);
			return result;
		}
	}
}