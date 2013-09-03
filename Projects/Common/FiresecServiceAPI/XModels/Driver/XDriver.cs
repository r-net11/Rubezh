using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XDriver
	{
		public static Guid System_UID = new Guid("938947C5-4624-4A1A-939C-60AEEBF7B65C");

		public XDriver()
		{
			Children = new List<XDriverType>();
			Properties = new List<XDriverProperty>();
			AUParameters = new List<XAUParameter>();
			AutoCreateChildren = new List<XDriverType>();
			AvailableStateBits = new List<XStateBit>();
			AvailableStateClasses = new List<XStateClass>();
			AvailableCommandBits = new List<XStateBit>();
			CanEditAddress = true;
			HasAddress = true;
			IsDeviceOnShleif = true;
            IsPlaceable = false;
		}

		[DataMember]
		public XDriverType DriverType { get; set; }
		[DataMember]
		public ushort DriverTypeNo { get; set; }
		[DataMember]
		public Guid UID { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string ShortName { get; set; }
        [DataMember]
        public string DeviceClassName { get; set; }
        [DataMember]
        public List<XDriverState> XStates { get; set; }

		[DataMember]
		public List<XDriverProperty> Properties { get; set; }
		[DataMember]
		public List<XStateBit> AvailableStateBits { get; set; }
		[DataMember]
		public List<XStateClass> AvailableStateClasses { get; set; }
		[DataMember]
		public List<XStateBit> AvailableCommandBits { get; set; }
		[DataMember]
		public List<XAUParameter> AUParameters { get; set; }

		[DataMember]
		public List<XDriverType> Children { get; set; }
		[DataMember]
		public List<XDriverType> AutoCreateChildren { get; set; }

		[DataMember]
		public bool HasAddress { get; set; }
		[DataMember]
		public bool CanEditAddress { get; set; }
		[DataMember]
		public bool IsRangeEnabled { get; set; }
		[DataMember]
		public bool IsAutoCreate { get; set; }
		[DataMember]
		public byte MinAddress { get; set; }
		[DataMember]
		public byte MaxAddress { get; set; }
		[DataMember]
		public byte MaxAddressOnShleif { get; set; }
		[DataMember]
		public bool IsDeviceOnShleif { get; set; }

		[DataMember]
		public bool HasLogic { get; set; }
		[DataMember]
		public bool IgnoreHasLogic { get; set; }
		[DataMember]
		public bool HasZone { get; set; }
		[DataMember]
		public bool IsControlDevice { get; set; }
        [DataMember]
        public bool IsPlaceable { get; set; }

		[DataMember]
		public bool IsGroupDevice { get; set; }
		[DataMember]
		public XDriverType GroupDeviceChildType { get; set; }
		[DataMember]
		public byte GroupDeviceChildrenCount { get; set; }

		public bool IsKauOrRSR2Kau
		{
			get { return DriverType == XDriverType.KAU || DriverType == XDriverType.RSR2_KAU; }
		}

		public string ImageSource
		{
			get { return "/Controls;component/GKIcons/" + this.DriverType.ToString() + ".png"; }
		}
	}
}