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
		public object Clone()
		{
			return MemberwiseClone();
		}

		public string ParentPath
		{
			get
			{
				var path = "";
				if (Device != null)
				{
					path = ((Device.GKParent == null) ? "" : Device.GKParent.PresentationDriverAndAddress) +
					       ((Device.KAUParent == null) ? "" : Device.KAUParent.PresentationDriverAndAddress) +
					       ((Device.KAURSR2Parent == null) ? "" : Device.KAURSR2Parent.PresentationDriverAndAddress);
				}
				return path;
			}
		}
		public ObjectViewModel(XDevice device)
		{
			Name = device.ShortName;
			Address = "";
			Address = device.PresentationAddress;
			ImageSource = "/Controls;component/GKIcons/" + device.Driver.DriverType + ".png"; 
			Device = device;
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
	}
}
