﻿using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	public class SKDDeviceInfo
	{
		[DataMember]
		public string DeviceType { get; set; }

		[DataMember]
		public string SoftwareVersion { get; set; }

		[DataMember]
		public DateTime SoftwareBuildDate { get; set; }

		[DataMember]
		public string IP { get; set; }

		[DataMember]
		public string SubnetMask { get; set; }

		[DataMember]
		public string DefaultGateway { get; set; }

		[DataMember]
		public DateTime CurrentDateTime { get; set; }
	}
}