using Infrastructure.Common.TreeList;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ObjectViewModel : TreeNodeViewModel<ObjectViewModel>
	{
		public string PresentationName { get; set; }
		public string PresentationAddress { get; set; }
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
		public ObjectViewModel(XDevice device)
		{
			PresentationName = device.ShortName;
			PresentationAddress = "";
			PresentationAddress = device.PresentationAddress;
			ImageSource = "/Controls;component/GKIcons/" + device.Driver.DriverType + ".png"; 
			Device = device;
		}

		public ObjectViewModel(XZone zone)
		{
			PresentationName = zone.PresentationName;
			PresentationAddress = "";
			Zone = zone;
		}

		public ObjectViewModel(XDirection direction)
		{
			PresentationName = direction.PresentationName;
			PresentationAddress = "";
			Direction = direction;
		}
	}
}
