using System;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class ObjectViewModel : BaseViewModel, IComparable  
	{
		public string Name { get; set; }
		public string Address { get; set; }
		public string PresentationZone { get; set; }
		public bool IsAbsent { get; set; }
		public bool IsPresent { get; set; }
		public bool HasNonStructureDifferences
		{
			get
			{
				if (IsAbsent || IsPresent || !ConfigurationCompareViewModel.ConfigFromFile)
					return false;
				return !String.IsNullOrEmpty(DifferenceDiscription);
			}
		}

		public bool HasDifferences
		{
			get
			{
				return !String.IsNullOrEmpty(DifferenceDiscription);
			}
		}

		public string DifferenceDiscription { get; set; }
		public XDevice Device;
		public XZone Zone;
		public XDirection Direction;
		public XPumpStation PumpStation;
		public XMPT MPT;
		public XDelay Delay;
		public XGuardZone GuardZone;
		public string ImageSource { get; private set; }
		public ObjectType ObjectType { get; private set; }
		public object Clone()
		{
			return MemberwiseClone();
		}

		public ObjectViewModel(XDevice device)
		{
			Device = device;
			Name = device.ShortName;
			Address = device.DottedPresentationAddress;
			PresentationZone = device.IsNotUsed ? "" : XManager.GetPresentationZone(Device);
			ImageSource = "/Controls;component/GKIcons/" + device.DriverType + ".png";
			ObjectType = ObjectType.Device;
		}

		public ObjectViewModel(XZone zone)
		{
			Zone = zone;
			Name = zone.PresentationName;
			ImageSource = "/Controls;component/Images/Zone.png";
			Address = "";
			PresentationZone = "";
			ObjectType = ObjectType.Zone;
		}

		public ObjectViewModel(XDirection direction)
		{
			Direction = direction;
			Name = direction.PresentationName;
			ImageSource = "/Controls;component/Images/Blue_Direction.png";
			Address = "";
			PresentationZone = "";
			ObjectType = ObjectType.Direction;
		}

		public ObjectViewModel(XPumpStation pumpStation)
		{
			PumpStation = pumpStation;
			Name = pumpStation.PresentationName;
			ImageSource = "/Controls;component/Images/BPumpStation.png";
			Address = "";
			PresentationZone = "";
			ObjectType = ObjectType.PumpStation;
		}

		public ObjectViewModel(XMPT mpt)
		{
			MPT = mpt;
			Name = mpt.PresentationName;
			ImageSource = "/Controls;component/Images/BMPT.png";
			Address = "";
			PresentationZone = "";
			ObjectType = ObjectType.MPT;
		}

		public ObjectViewModel(XDelay delay)
		{
			Delay = delay;
			Name = delay.PresentationName;
			ImageSource = "/Controls;component/Images/Delay.png";
			Address = "";
			PresentationZone = "";
			ObjectType = ObjectType.Delay;
		}

		public ObjectViewModel(XGuardZone guardZone)
		{
			GuardZone = guardZone;
			Name = guardZone.PresentationName;
			ImageSource = "/Controls;component/Images/GuardZone.png";
			Address = "";
			PresentationZone = "";
			ObjectType = ObjectType.GuardZone;
		}

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
					(object1.Device.KAUParent != null ? object1.Device.KAUParent.IntAddress * 256 * 256 * 256 : 0) +
					(object1.Device.ShleifNo * 256 * 256) +
					(!object1.Device.Driver.IsKauOrRSR2Kau ? object1.Device.IntAddress * 256 : 0)
					+ object1.Device.Driver.DriverType;
				var orderNo2 =
					(object2.Device.KAUParent != null ? object2.Device.KAUParent.IntAddress * 256 * 256 * 256 : 0) +
					(object2.Device.ShleifNo * 256 * 256) +
					(!object2.Device.Driver.IsKauOrRSR2Kau ? object2.Device.IntAddress * 256 : 0)
					+ object2.Device.Driver.DriverType;
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
				return 0;
			}
			if (object1.ObjectType == ObjectType.PumpStation)
			{
				if (object1.PumpStation.No > object2.PumpStation.No)
					return 1;
				if (object1.PumpStation.No < object2.PumpStation.No)
					return -1;
			}
			if (object1.ObjectType == ObjectType.MPT)
			{
				return string.Compare(object1.MPT.Name, object2.MPT.Name);
			}
			if (object1.ObjectType == ObjectType.Delay)
			{
				return string.Compare(object1.Delay.Name, object2.Delay.Name);
			}

			if (object1.ObjectType == ObjectType.GuardZone)
			{
				if (object1.GuardZone.No > object2.GuardZone.No)
					return 1;
				if (object1.GuardZone.No < object2.GuardZone.No)
					return -1;
				return 0;
			}
			return 0;
		}
	}

	public enum ObjectType
	{
		Device = 0,
		Zone = 1,
		Direction = 2,
		PumpStation = 3,
		MPT = 4,
		Delay = 5,
		GuardZone = 6,
	}
}