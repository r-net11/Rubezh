using System;
using System.Collections;
using System.Collections.Generic;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ObjectViewModel : BaseViewModel, IComparer, IComparable  
	{
		int IComparable.CompareTo(object a)
		{
			return Compare(this, a);
		}
		int IComparer.Compare(object a, object b)
		{
			return Compare(a, b);
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

			if (object1.Direction.No > object2.Direction.No)
				return 1;
			if (object1.Direction.No < object2.Direction.No)
				return -1;
			return 0;
		}

		public string Name { get; set; }
		public string Address { get; set; }
		public bool HasDifferences { get; set; }
		public bool HasMissingDifferences { get; set; }
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
			Name = device.ShortName;
			Address = device.Address;
			ImageSource = "/Controls;component/GKIcons/" + device.DriverType + ".png";
			Device = device;
		}

		public ObjectViewModel(XZone zone)
		{
			Name = zone.PresentationName;
			ImageSource = "/Controls;component/Images/zone.png";
			Address = "";
			Zone = zone;
		}

		public ObjectViewModel(XDirection direction)
		{
			Name = direction.PresentationName;
			ImageSource = "/Controls;component/Images/Blue_Direction.png";
			Address = "";
			Direction = direction;
		}

		public static bool Equels(ObjectViewModel object1, ObjectViewModel object2)
		{
			if((object1.ObjectType == ObjectType.Device)&&(object2.ObjectType == ObjectType.Device))
				return (object1.Name == object2.Name) && (object1.Address == object2.Address) && (object1.Device.Parent.ShortName == object2.Device.Parent.ShortName) && (object1.Device.Parent.Address == object2.Device.Parent.Address);
			if (((object1.ObjectType == ObjectType.Zone) && (object2.ObjectType == ObjectType.Zone)) || ((object1.ObjectType == ObjectType.Direction) && (object2.ObjectType == ObjectType.Direction)))
				return (object1.Name == object2.Name);
			return false;
		}
		
	}
	public enum ObjectType
	{
		Device,
		Zone,
		Direction
	}
}