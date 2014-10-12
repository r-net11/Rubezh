using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class DeviceViewModelDeviceComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			return string.Compare(x.Device.Driver.ShortName, y.Device.Driver.ShortName);
		}
	}
	public class DeviceViewModelAddressComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			if (x.Device.Address != y.Device.Address)
			{
				return string.Compare(x.Device.Address, y.Device.Address);
			}
			else
			{
				return 0;
			}
		}
	}
	public class DeviceViewModelZoneComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			int result = string.Compare(x.Zone != null ? x.Zone.PresentationName : "", y.Zone != null ? y.Zone.PresentationName : "");
			if (result == 0)
				result = string.Compare(x.Device.Driver.ShortName, y.Device.Driver.ShortName);
			return result;
		}
	}
}