using System.Collections.Generic;
using Infrastructure.Common.TreeList;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ObjectViewModel : TreeNodeViewModel<ObjectViewModel>
	{
		public string Name { get; set; }
		public string Address { get; set; }
		public bool HasDifferences { get; set; }
		public bool HasMissingDifferences { get; set; }
		public XDevice Device;
		public XZone Zone;
		public XDirection Direction;
		public string ImageSource { get; private set; }
		public ObjectViewModel Parent { get; set; }
		public List<ObjectViewModel> Children { get; set; }
        public bool IsDevice { get; set; }
        public bool IsZone { get; set; }
        public bool IsDirection { get; set; }
		public object Clone()
		{
			return MemberwiseClone();
		}

		public ObjectViewModel(XDevice device)
		{
			Name = device.ShortName;
			Address = InitializeAddress(device);
			ImageSource = "/Controls;component/GKIcons/" + device.Driver.DriverType + ".png"; 
			Device = device;
			Children = new List<ObjectViewModel>();
		}

		public ObjectViewModel(XZone zone)
		{
			Name = zone.PresentationName;
			Address = "";
			Zone = zone;
		}

		public ObjectViewModel(XDirection direction)
		{
			Name = direction.PresentationName;
			Address = "";
			Direction = direction;
		}

		string InitializeAddress(XDevice device)
		{
			if (device.Driver.DriverType == XDriverType.GK)
			{
				return device.Address;
			}
			if (device.Driver.HasAddress == false)
				return "";

			if (device.Driver.IsDeviceOnShleif == false)
				return device.IntAddress.ToString();
			if (device.ShleifNo != 0)
				return device.ShleifNo + "." + device.IntAddress;
			return device.IntAddress.ToString();
		}

		public bool IsVirtualDevice
		{
			get
			{
				if (Device == null)
					return false;
				if ((Device.Driver.IsGroupDevice) || (Device.Driver.DriverType == XDriverType.KAU_Shleif) || (Device.Driver.DriverType == XDriverType.RSR2_KAU_Shleif))
					return true;
				return false;
			}
		}
	}
}
