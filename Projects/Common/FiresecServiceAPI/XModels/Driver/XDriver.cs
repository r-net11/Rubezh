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
			Children = new List<Guid>();
			AutoCreateChildren = new List<Guid>();
			UseOffBitInLogic = true;
		}

		[DataMember]
		public short DriverTypeNo { get; set; }

		[DataMember]
		public XDriverType DriverType { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public Guid OldDriverUID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string ShortName { get; set; }

		[DataMember]
		public string ImageSource { get; set; }

		[DataMember]
		public bool HasImage { get; set; }

		[DataMember]
		public bool CanEditAddress { get; set; }

		[DataMember]
		public bool IsChildAddressReservedRange { get; set; }

		[DataMember]
		public List<XDriverProperty> Properties { get; set; }

		[DataMember]
		public List<Guid> Children { get; set; }

		[DataMember]
		public List<Guid> AutoCreateChildren { get; set; }

		[DataMember]
		public bool IsAutoCreate { get; set; }

		[DataMember]
		public Guid AutoChild { get; set; }

		[DataMember]
		public byte AutoChildCount { get; set; }

		[DataMember]
		public byte MinAutoCreateAddress { get; set; }

		[DataMember]
		public byte MaxAutoCreateAddress { get; set; }

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
		public bool UseOffBitInLogic { get; set; }
	}
}