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
			Address = "";
			Address = device.PresentationAddress;
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
	}
}
