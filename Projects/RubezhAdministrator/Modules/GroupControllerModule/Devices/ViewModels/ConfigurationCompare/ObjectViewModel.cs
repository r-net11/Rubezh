using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using System;

namespace GKModule.ViewModels
{
	public class ObjectViewModel : BaseViewModel, IComparable
	{
		public string Name { get; set; }
		public string Address { get; set; }
		public string PresentationZoneOrLogic { get; set; }
		public bool IsAbsent { get; set; }
		public bool IsPresent { get; set; }
		public bool IsDevice { get; private set; }
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

		public bool HasZoneOrLogic
		{
			get
			{
				return !String.IsNullOrEmpty(PresentationZoneOrLogic);
			}
		}

		public string SortingName { get; private set; }

		public string DifferenceDiscription { get; set; }
		public GKDevice Device;
		public GKZone Zone;
		public GKDirection Direction;
		public GKPumpStation PumpStation;
		public GKMPT MPT;
		public GKDelay Delay;
		public GKGuardZone GuardZone;
		public GKCode Code;
		public GKDoor Door;
		public GKSKDZone SKDZone { get; private set; }
		public string ImageSource { get; private set; }
		public ObjectType ObjectType { get; private set; }
		public GKDevice KAUParent { get; private set; }
		public object Clone()
		{
			return MemberwiseClone();
		}

		public ObjectViewModel(GKDevice device)
		{
			Device = device;
			IsDevice = true;
			Name = device.ShortName;
			Address = device.DottedPresentationAddress;
			PresentationZoneOrLogic = GKManager.GetPresentationZoneAndGuardZoneOrLogic(Device);
			ImageSource = "/Controls;component/GKIcons/" + device.DriverType + ".png";
			ObjectType = ObjectType.Device;
			KAUParent = device.KAUParent;
			SortingName = "a " +
				(device.KAUParent != null ? device.KAUParent.IntAddress * 256 * 256 * 256 : 0) +
				(device.ShleifNo * 256 * 256) +
				(!device.Driver.IsKau ? device.IntAddress * 256 : 0)
				+ device.Driver.DriverType;
		}

		public ObjectViewModel(GKZone zone)
		{
			Zone = zone;
			Name = zone.PresentationName;
			ImageSource = "/Controls;component/Images/Zone.png";
			Address = "";
			PresentationZoneOrLogic = "";
			ObjectType = ObjectType.Zone;
			SortingName = "b " + zone.No;
		}

		public ObjectViewModel(GKDirection direction)
		{
			Direction = direction;
			Name = direction.PresentationName;
			ImageSource = "/Controls;component/Images/Blue_Direction.png";
			Address = "";
			PresentationZoneOrLogic = GKManager.GetPresentationLogic(direction.Logic);
			ObjectType = ObjectType.Direction;
			SortingName = "c " + direction.No;
		}

		public ObjectViewModel(GKPumpStation pumpStation)
		{
			PumpStation = pumpStation;
			Name = pumpStation.PresentationName;
			ImageSource = "/Controls;component/Images/BPumpStation.png";
			Address = "";
			PresentationZoneOrLogic = "";
			ObjectType = ObjectType.PumpStation;
			SortingName = "d " + pumpStation.No;
		}

		public ObjectViewModel(GKMPT mpt)
		{
			MPT = mpt;
			Name = mpt.PresentationName;
			ImageSource = "/Controls;component/Images/BMPT.png";
			Address = "";
			PresentationZoneOrLogic = "";
			ObjectType = ObjectType.MPT;
			SortingName = "e " + mpt.No;
		}

		public ObjectViewModel(GKDelay delay)
		{
			Delay = delay;
			Name = delay.PresentationName;
			ImageSource = "/Controls;component/Images/Delay.png";
			Address = "";
			PresentationZoneOrLogic = GKManager.GetPresentationLogic(delay.Logic);
			ObjectType = ObjectType.Delay;
			SortingName = "f " + delay.No;
		}

		public ObjectViewModel(GKGuardZone guardZone)
		{
			GuardZone = guardZone;
			Name = guardZone.PresentationName;
			ImageSource = "/Controls;component/Images/GuardZone.png";
			Address = "";
			PresentationZoneOrLogic = "";
			ObjectType = ObjectType.GuardZone;
			SortingName = "g " + guardZone.No;
		}

		public ObjectViewModel(GKCode code)
		{
			Code = code;
			Name = code.PresentationName;
			ImageSource = "/Controls;component/Images/Code.png";
			Address = "";
			PresentationZoneOrLogic = "";
			ObjectType = ObjectType.Code;
			SortingName = "h " + code.No;
		}

		public ObjectViewModel(GKDoor door)
		{
			Door = door;
			Name = door.PresentationName;
			ImageSource = "/Controls;component/Images/Door.png";
			Address = "";
			PresentationZoneOrLogic = "";
			ObjectType = ObjectType.Door;
			SortingName = "k " + door.No;
		}
		public ObjectViewModel(GKSKDZone skdZone)
		{
			SKDZone = skdZone;
			Name = skdZone.PresentationName;
			ImageSource = skdZone.ImageSource;
			Address = "";
			PresentationZoneOrLogic = "";
			ObjectType = ObjectType.SKDZone;
			SortingName = "l " + skdZone.No;
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
				var device1 = object1.Device;
				var device2 = object2.Device;
				var intAddress1 = 0;
				var intAddress2 = 0;
				if (device1.KAUParent != null)
					intAddress1 = device1.KAUParent.IntAddress;
				else if (device1.MirrorParent != null)
					intAddress1 = device1.MirrorParent.IntAddress;
				if (device2.KAUParent != null)
					intAddress2 = device2.KAUParent.IntAddress;
				else if (device2.MirrorParent != null)
					intAddress2 = device2.MirrorParent.IntAddress;

				var orderNo1 = (intAddress1 * 256 * 256 * 256) + (device1.ShleifNo * 256 * 256) + (!device1.Driver.IsKau && !device1.Driver.IsEditMirror && device1.Driver.DriverType != GKDriverType.GKIndicator ? device1.IntAddress * 256 : 0) + device1.Driver.DriverType;
				var orderNo2 = (intAddress2 * 256 * 256 * 256) + (device2.ShleifNo * 256 * 256) + (!device2.Driver.IsKau && !device2.Driver.IsEditMirror && device1.Driver.DriverType != GKDriverType.GKIndicator ? device2.IntAddress * 256 : 0) + device2.Driver.DriverType;
				if (orderNo1 > orderNo2)
					return 1;
				if (orderNo1 < orderNo2)
					return -1;
				if (device1.IntAddress > device2.IntAddress)
					return 1;
				if (device1.IntAddress < device2.IntAddress)
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
				if (object1.Delay.No > object2.Delay.No)
					return 1;
				if (object1.Delay.No < object2.Delay.No)
					return -1;
				return 0;
			}

			if (object1.ObjectType == ObjectType.GuardZone)
			{
				if (object1.GuardZone.No > object2.GuardZone.No)
					return 1;
				if (object1.GuardZone.No < object2.GuardZone.No)
					return -1;
				return 0;
			}

			if (object1.ObjectType == ObjectType.Code)
			{
				if (object1.Code.No > object2.Code.No)
					return 1;
				if (object1.Code.No < object2.Code.No)
					return -1;
				return 0;
			}

			if (object1.ObjectType == ObjectType.Door)
			{
				if (object1.Door.No > object2.Door.No)
					return 1;
				if (object1.Door.No < object2.Door.No)
					return -1;
				return 0;
			}

			if (object1.ObjectType == ObjectType.SKDZone)
			{
				if (object1.SKDZone.No > object2.SKDZone.No)
					return 1;
				if (object1.ObjectType == ObjectType.SKDZone)
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
		Code = 7,
		Door = 8,
		SKDZone = 9
	}
}