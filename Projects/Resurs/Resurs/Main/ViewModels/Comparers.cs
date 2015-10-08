using Infrastructure.Common.TreeList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class DeviceViewModelNameComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			return string.Compare(x.Device.Name, y.Device.Name);
		}
	}

	public class DeviceViewModelDescriptionComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			return string.Compare(x.Device.Description, y.Device.Description);
		}
	}

	public class DeviceViewModelAddressComparer : TreeNodeComparer<DeviceViewModel>
	{
		protected override int Compare(DeviceViewModel x, DeviceViewModel y)
		{
			if (x.Device.Address > y.Device.Address)
				return 1;
			if (x.Device.Address < y.Device.Address)
				return -1;
			return 0;
			//return string.Compare(x.Device.FullAddress, y.Device.FullAddress);
		}
	}

	public class ConsumerViewModelNameComparer : TreeNodeComparer<ConsumerViewModel>
	{
		protected override int Compare(ConsumerViewModel x, ConsumerViewModel y)
		{
			return string.Compare(x.Consumer.Name, y.Consumer.Name);
		}
	}

	public class ConsumerViewModelAddressComparer : TreeNodeComparer<ConsumerViewModel>
	{
		protected override int Compare(ConsumerViewModel x, ConsumerViewModel y)
		{
			return string.Compare(x.Consumer.Address, y.Consumer.Address);
		}
	}

	public class ConsumerViewModelDescriptionComparer : TreeNodeComparer<ConsumerViewModel>
	{
		protected override int Compare(ConsumerViewModel x, ConsumerViewModel y)
		{
			return string.Compare(x.Consumer.Description, y.Consumer.Description);
		}
	}
}