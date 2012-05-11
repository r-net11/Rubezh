using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XZone : XBinaryBase
	{
		public XZone()
		{
			DetectorCount = 2;
			DeviceUIDs = new List<Guid>();
		}

		public List<XDevice> Devices { get; set; }

		[DataMember]
		public short No { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public short DetectorCount { get; set; }

		[DataMember]
		public List<Guid> DeviceUIDs { get; set; }

		public string PresentationName
		{
			get { return No + "." + Name; }
		}
	}
}