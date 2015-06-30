using Infrastructure.Common.TreeList;

namespace GKModule.ViewModels
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
			return x.Device.IntAddress - y.Device.IntAddress;
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
	public class DeviceViewModelDescriptionComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			int result = string.Compare(x.Description, y.Description);
			if (result == 0)
				result = string.Compare(x.Driver.ShortName, y.Driver.ShortName);
			return result;
		}
	}
	public class DeviceViewModelProjectAddressComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			int result = string.Compare(x.ProjectAddress, y.ProjectAddress);
			if (result == 0)
				result = string.Compare(x.Driver.ShortName, y.Driver.ShortName);
			return result;
		}
	}
}