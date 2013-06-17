using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.TreeList;

namespace DevicesModule.ViewModels
{
	public class DeviceViewModelDeviceComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			return string.Compare(x.Driver.ShortName, y.Driver.ShortName);
		}
	}
	public class DeviceViewModelAddressComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			return string.Compare(x.Address, y.Address);
		}
	}
	public class DeviceViewModelZoneComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			return string.Compare(x.EditingPresentationZone, y.EditingPresentationZone);
		}
	}
	public class DeviceViewModelDescriptionComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			return string.Compare(x.Description, y.Description);
		}
	}
}
