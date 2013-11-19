using System;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ObjectViewModel : BaseViewModel, IComparable  
	{
		int IComparable.CompareTo(object a)
		{
			return Compare(this, a);
		}

		public int Compare(object a, object b)
		{
			var object1 = (ObjectViewModel)a;
			var object2 = (ObjectViewModel)b;
			if (object1.ObjectType > object2.ObjectType)
				return 1;
			if (object1.ObjectType < object2.ObjectType)
				return -1;

			if (object1.ObjectType == ObjectType.Device)
			{
				var orderNo1 = 
					(object1.Device.KAUParent != null ? object1.Device.KAUParent.IntAddress * 256 * 256 :0) +
					(object1.Device.ShleifNo * 256) +
					(!object1.Device.Driver.IsKauOrRSR2Kau ? object1.Device.IntAddress : 0);
				var orderNo2 = 
					(object2.Device.KAUParent != null ? object2.Device.KAUParent.IntAddress * 256 * 256 : 0) +
					(object2.Device.ShleifNo * 256) +
					(!object2.Device.Driver.IsKauOrRSR2Kau ? object2.Device.IntAddress : 0);
				if (orderNo1 > orderNo2)
					return 1;
				if (orderNo1 < orderNo2)
					return -1;
				return 0;
			}

			if (object1.ObjectType == ObjectType.Zone)
			{
				if (object1.Zone.No > object2.Zone.No)
					return 1;
				if (object1.Zone.No < object2.Zone.No)
					return -1;
				return 0;
			}

			if (object1.ObjectType == ObjectType.Direction)
			{
				if (object1.Direction.No > object2.Direction.No)
					return 1;
				if (object1.Direction.No < object2.Direction.No)
					return -1;
			}
			return 0;
		}

		public string Name { get; set; }
		public string Address { get; set; }
		public bool IsAbsent { get; set; }
		public bool IsPresent { get; set; }
		public XDevice Device;
		public XZone Zone;
		public XDirection Direction;
		public string ImageSource { get; private set; }
		public ObjectType ObjectType { get; set; }
		public object Clone()
		{
			return MemberwiseClone();
		}

		public ObjectViewModel(XDevice device)
		{
			Device = device;
			Name = device.ShortName;
			Address = device.Address;
			ImageSource = "/Controls;component/GKIcons/" + device.DriverType + ".png";
			ObjectType = ObjectType.Device;
		}

		public ObjectViewModel(XZone zone)
		{
			Zone = zone;
			Name = zone.PresentationName;
			ImageSource = "/Controls;component/Images/zone.png";
			Address = "";
			ObjectType = ObjectType.Zone;
		}

		public ObjectViewModel(XDirection direction)
		{
			Direction = direction;
			Name = direction.PresentationName;
			ImageSource = "/Controls;component/Images/Blue_Direction.png";
			Address = "";
			ObjectType = ObjectType.Zone;
		}
	}

	public enum ObjectType
	{
		Device = 0,
		Zone = 1,
		Direction = 2
	}
}