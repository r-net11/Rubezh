using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RubezhAPI.GK
{
	public class GKDriver
	{
		public GKDriver()
		{
			Children = new List<GKDriverType>();
			Properties = new List<GKDriverProperty>();
			MeasureParameters = new List<GKMeasureParameter>();
			AutoCreateChildren = new List<GKDriverType>();
			AvailableStateBits = new List<GKStateBit>();
			AvailableStateClasses = new List<XStateClass>();
			AvailableCommandBits = new List<GKStateBit>();
			CanEditAddress = false;
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
		public int MaxAddress { get; set; }
		public byte MinAddress2 { get; set; }
		public byte MaxAddress2 { get; set; }
		public byte MaxAddressOnShleif { get; set; }
		public bool IsDeviceOnShleif { get; set; }
		public bool IsReal { get; set; }
		public bool HasLogic { get; set; }
		public bool IgnoreHasLogic { get; set; }
		public bool HasZone { get; set; }
		public bool HasMirror { get; set; }
		public bool IsEditMirror { get; set; }
		public bool HasGuardZone { get; set; }
		public bool IsControlDevice { get; set; }
		public bool IsPlaceable { get; set; }
		public bool IsGroupDevice { get; set; }
		public GKDriverType GroupDeviceChildType { get; set; }
		public byte GroupDeviceChildrenCount { get; set; }

		public bool IsKau
		{
			get { return DriverType == GKDriverType.RSR2_KAU; }
		}

		public bool IsAm
		{
			get
			{
				return DriverType == GKDriverType.RSR2_AM_1 || DriverType == GKDriverType.RSR2_AM_2 || DriverType == GKDriverType.RSR2_AM_4 || DriverType == GKDriverType.RSR2_MAP4_Group || DriverType == GKDriverType.RSR2_MAP4;
			}
		}

		public bool IsPump
		{
			get
			{
				return DriverType == GKDriverType.RSR2_Bush_Drenazh || DriverType == GKDriverType.RSR2_Bush_Jokey || DriverType == GKDriverType.RSR2_Bush_Fire;
			}
		}

		public bool IsCardReaderOrCodeReader
		{
			get
			{
				return DriverType == GKDriverType.RSR2_CardReader || DriverType == GKDriverType.RSR2_CodeReader || DriverType == GKDriverType.RSR2_CodeCardReader;
			}
		}

		public string ImageSource
		{
			get { return "/Controls;component/GKIcons/" + DriverType + ".png"; }
		}
		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is GKDriver))
				return false;
			return ((GKDriver)obj).UID == this.UID;
		}

		public DriverClassifications DriverClassification { get; set; }

		public enum DriverClassifications
		{
			[Description("Извещатели пожарные")]
			FireDetector = 1,

			[Description("Извещатели охранные")]
			IntruderDetector = 2,

			[Description("Исполнительные устройства")]
			ActuatingDevice = 3,

			[Description("Шкафы управления")]
			ControlCabinet = 4,

			[Description("Оповещатели")]
			Announcers = 5,

			[Description("Радиоканал")]
			RadioChannel = 6,

			[Description("Прочие")]
			Other = 7,

			[Description("Контроль доступа")]
			AccessControl = 8,
		}
	}
}