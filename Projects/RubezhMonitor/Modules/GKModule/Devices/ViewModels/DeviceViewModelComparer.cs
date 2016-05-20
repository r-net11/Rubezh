using RubezhAPI.GK;
using Infrastructure.Common.TreeList;

namespace GKModule.ViewModels
{
	public class DeviceViewModelDeviceComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			return string.Compare(x.Device.ShortName, y.Device.ShortName);
		}
	}
	public class DeviceViewModelAddressComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			return GetSortingAddress(x.Device) - GetSortingAddress(y.Device);
		}
		int GetSortingAddress(GKDevice device)
		{
			if (device.Driver.IsKau)
				return 256 + device.IntAddress;
			return device.ShleifNo * 256 + device.IntAddress;
		}
	}
	public class DeviceViewModelZoneComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			int result = string.Compare(x.PresentationZone, y.PresentationZone);
			if (result == 0)
				result = string.Compare(x.Device.Driver.ShortName, y.Device.Driver.ShortName);
			return result;
		}
	}
	public class DeviceViewModelDescriptionComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			int result = string.Compare(x.Device.Description, y.Device.Description);
			if (result == 0)
				result = string.Compare(x.Device.Driver.ShortName, y.Device.Driver.ShortName);
			return result;
		}
	}
}