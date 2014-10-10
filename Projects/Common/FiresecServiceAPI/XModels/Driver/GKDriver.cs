using System;
using System.Collections.Generic;

namespace FiresecAPI.GK
{
	public class GKDriver
	{
		public static Guid System_UID = new Guid("938947C5-4624-4A1A-939C-60AEEBF7B65C");

		public GKDriver()
		{
			Children = new List<GKDriverType>();
			Properties = new List<GKDriverProperty>();
			MeasureParameters = new List<GKMeasureParameter>();
			AutoCreateChildren = new List<GKDriverType>();
			AvailableStateBits = new List<GKStateBit>();
			AvailableStateClasses = new List<XStateClass>();
			AvailableCommandBits = new List<GKStateBit>();
			CanEditAddress = true;
			HasAddress = true;
			IsDeviceOnShleif = true;
			IsReal = true;
			IsPlaceable = false;
		}

		public GKDriverType DriverType { get; set; }
		public ushort DriverTypeNo { get; set; }
		public Guid UID { get; set; }
		public string Name { get; set; }
		public string ShortName { get; set; }
		public string DeviceClassName { get; set; }
		public List<GKDriverState> XStates { get; set; }

		public List<GKDriverProperty> Properties { get; set; }
		public List<GKStateBit> AvailableStateBits { get; set; }
		public List<XStateClass> AvailableStateClasses { get; set; }
		public List<GKStateBit> AvailableCommandBits { get; set; }
		public List<GKMeasureParameter> MeasureParameters { get; set; }

		public List<GKDriverType> Children { get; set; }
		public List<GKDriverType> AutoCreateChildren { get; set; }

		public bool HasAddress { get; set; }
		public bool CanEditAddress { get; set; }
		public bool IsRangeEnabled { get; set; }
		public bool IsAutoCreate { get; set; }
		public byte MinAddress { get; set; }
		public byte MaxAddress { get; set; }
		public byte MinAddress2 { get; set; }
		public byte MaxAddress2 { get; set; }
		public byte MaxAddressOnShleif { get; set; }
		public bool IsDeviceOnShleif { get; set; }
		public bool IsReal { get; set; }

		public bool HasLogic { get; set; }
		public bool IgnoreHasLogic { get; set; }
		public bool HasZone { get; set; }
		public bool IsControlDevice { get; set; }
		public bool IsPlaceable { get; set; }
		public bool IsGroupDevice { get; set; }
		public GKDriverType GroupDeviceChildType { get; set; }
		public byte GroupDeviceChildrenCount { get; set; }
		public bool IsIgnored { get; set; }

		public bool IsKauOrRSR2Kau
		{
			get { return DriverType == GKDriverType.KAU || DriverType == GKDriverType.RSR2_KAU; }
		}

		public bool IsPump
		{
			get
			{
				return (DriverType == GKDriverType.FirePump || DriverType == GKDriverType.JockeyPump ||
						DriverType == GKDriverType.DrainagePump || DriverType == GKDriverType.RSR2_Bush_Drenazh);
			}
		}


		public string ImageSource
		{
			get { return "/Controls;component/GKIcons/" + this.DriverType.ToString() + ".png"; }
		}
	}
}