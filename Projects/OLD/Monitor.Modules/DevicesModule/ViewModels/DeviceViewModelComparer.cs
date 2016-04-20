using Infrastructure.Common.Windows.TreeList;

namespace DevicesModule.ViewModels
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
			return x.Device.IntAddress - y.Device.IntAddress;
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
	public class DeviceViewModelFailureTypeComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			int result = string.Compare(x.FailureType, y.FailureType);
			if (result == 0)
				result = string.Compare(x.Device.Driver.ShortName, y.Device.Driver.ShortName);
			return result;
		}
	}
	public class DeviceViewModelAlarmReasonComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			int result = string.Compare(x.AlarmReason, y.AlarmReason);
			if (result == 0)
				result = string.Compare(x.Device.Driver.ShortName, y.Device.Driver.ShortName);
			return result;
		}
	}
	public class DeviceViewModelSmokinessComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			int result = string.Compare(x.Smokiness, y.Smokiness);
			if (result == 0)
				result = string.Compare(x.Device.Driver.ShortName, y.Device.Driver.ShortName);
			return result;
		}
	}
	public class DeviceViewModelDustinessComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			int result = string.Compare(x.Dustiness, y.Dustiness);
			if (result == 0)
				result = string.Compare(x.Device.Driver.ShortName, y.Device.Driver.ShortName);
			return result;
		}
	}
	public class DeviceViewModelTemperatureComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			int result = string.Compare(x.Temperature, y.Temperature);
			if (result == 0)
				result = string.Compare(x.Device.Driver.ShortName, y.Device.Driver.ShortName);
			return result;
		}
	}
}
