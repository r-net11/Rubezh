using System;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
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

		public static DateTime LastSoftwareBuildDate { get { return new DateTime(2016, 09, 23); } }
	}
}