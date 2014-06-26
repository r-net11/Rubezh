using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	public class SKDDeviceInfo
	{
		[DataMember]
		public DateTime SoftwareBuildDate { get; set; }

		[DataMember]
		public string DeviceType { get; set; }

		[DataMember]
		public string SoftwareVersion { get; set; }

		[DataMember]
		public string IP { get; set; }

		[DataMember]
		public string SubnetMask { get; set; }

		[DataMember]
		public string DefaultGateway { get; set; }

		[DataMember]
		public int MTU;

		[DataMember]
		public DateTime CurrentDateTime;
	}
}