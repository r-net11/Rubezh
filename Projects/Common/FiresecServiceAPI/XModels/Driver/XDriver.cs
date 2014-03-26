using System;
using System.Collections.Generic;

namespace XFiresecAPI
{
	public class XDriver
	{
		public static Guid System_UID = new Guid("938947C5-4624-4A1A-939C-60AEEBF7B65C");

		public XDriver()
		{
			Children = new List<XDriverType>();
			Properties = new List<XDriverProperty>();
			MeasureParameters = new List<XMeasureParameter>();
			AutoCreateChildren = new List<XDriverType>();
			AvailableStateBits = new List<XStateBit>();
			AvailableStateClasses = new List<XStateClass>();
			AvailableCommandBits = new List<XStateBit>();
			CanEditAddress = true;
			HasAddress = true;
			IsDeviceOnShleif = true;
			IsPlaceable = false;
		}

		public XDriverType DriverType { get; set; }
		public ushort DriverTypeNo { get; set; }
		public Guid UID { get; set; }
		public string Name { get; set; }
		public string ShortName { get; set; }
		public string DeviceClassName { get; set; }
		public List<XDriverState> XStates { get; set; }

		public List<XDriverProperty> Properties { get; set; }
		public List<XStateBit> AvailableStateBits { get; set; }
		public List<XStateClass> AvailableStateClasses { get; set; }
		public List<XStateBit> AvailableCommandBits { get; set; }
		public List<XMeasureParameter> MeasureParameters { get; set; }

		public List<XDriverType> Children { get; set; }
		public List<XDriverType> AutoCreateChildren { get; set; }

		public bool HasAddress { get; set; }
		public bool CanEditAddress { get; set; }
		public bool IsRangeEnabled { get; set; }
		public bool IsAutoCreate { get; set; }
		public byte MinAddress { get; set; }
		public byte MaxAddress { get; set; }
		public byte MaxAddressOnShleif { get; set; }
		public bool IsDeviceOnShleif { get; set; }

		public bool HasLogic { get; set; }
		public bool IgnoreHasLogic { get; set; }
		public bool HasZone { get; set; }
		public bool IsControlDevice { get; set; }
		public bool IsPlaceable { get; set; }
		public bool IsGroupDevice { get; set; }
		public XDriverType GroupDeviceChildType { get; set; }
		public byte GroupDeviceChildrenCount { get; set; }

		public bool IsKauOrRSR2Kau
		{
			get { return DriverType == XDriverType.KAU || DriverType == XDriverType.RSR2_KAU; }
		}

		public bool IsPump
		{
			get
			{
				return (DriverType == XDriverType.FirePump || DriverType == XDriverType.JockeyPump ||
						DriverType == XDriverType.DrainagePump || DriverType == XDriverType.RSR2_Bush);
			}
		}


		public string ImageSource
		{
			get { return "/Controls;component/GKIcons/" + this.DriverType.ToString() + ".png"; }
		}
	}
}