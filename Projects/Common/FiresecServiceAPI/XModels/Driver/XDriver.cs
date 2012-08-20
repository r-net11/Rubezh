using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XDriver
	{
		public XDriver()
		{
			Properties = new List<XDriverProperty>();
			Children = new List<XDriverType>();
			AutoCreateChildren = new List<XDriverType>();
			DriverTypeMappedProperties = new List<XDriverTypeMappedProperty>();
			CanEditAddress = true;
			HasAddress = true;
			IsChildAddressReservedRange = true;
			IsDeviceOnShleif = true;
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
		public bool CanEditAddress { get; set; }

		[DataMember]
		public bool IsChildAddressReservedRange { get; set; }

		[DataMember]
		public List<XDriverProperty> Properties { get; set; }

		[DataMember]
		public List<XDriverType> Children { get; set; }

		[DataMember]
		public List<XDriverType> AutoCreateChildren { get; set; }

		[DataMember]
		public bool IsAutoCreate { get; set; }

		[DataMember]
		public XDriverType AutoChild { get; set; }

		[DataMember]
		public byte AutoChildCount { get; set; }

		[DataMember]
		public bool UseParentAddressSystem { get; set; }

		[DataMember]
		public bool IsRangeEnabled { get; set; }

		[DataMember]
		public byte MinAddress { get; set; }

		[DataMember]
		public byte MaxAddress { get; set; }

		[DataMember]
		public bool HasAddress { get; set; }

		[DataMember]
		public byte ChildAddressReserveRangeCount { get; set; }

		[DataMember]
		public bool IsDeviceOnShleif { get; set; }

		[DataMember]
		public bool HasLogic { get; set; }

		[DataMember]
		public bool HasZone { get; set; }

		[DataMember]
		public bool IsGroupDevice { get; set; }

		public List<XDriverTypeMappedProperty> DriverTypeMappedProperties { get; set; }

		[DataMember]
		public int MaxAddressOnShleif { get; set; }

        [DataMember]
        public bool IsControlDevice { get; set; }

		public string ImageSource
		{
			get
			{
				return "/Controls;component/GKIcons/" + this.DriverType.ToString() + ".png";
			}
		}
	}
}