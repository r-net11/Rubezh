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

	public class ApartmentViewModelNameComparer : TreeNodeComparer<ApartmentViewModel>
	{
		protected override int Compare(ApartmentViewModel x, ApartmentViewModel y)
		{
			return string.Compare(x.Apartment.Name, y.Apartment.Name);
		}
	}

	public class ApartmentViewModelAddressComparer : TreeNodeComparer<ApartmentViewModel>
	{
		protected override int Compare(ApartmentViewModel x, ApartmentViewModel y)
		{
			return string.Compare(x.Apartment.Address, y.Apartment.Address);
		}
	}

	public class ApartmentViewModelDescriptionComparer : TreeNodeComparer<ApartmentViewModel>
	{
		protected override int Compare(ApartmentViewModel x, ApartmentViewModel y)
		{
			return string.Compare(x.Apartment.Description, y.Apartment.Description);
		}
	}
}