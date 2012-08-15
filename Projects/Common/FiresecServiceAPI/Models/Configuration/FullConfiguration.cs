using System.Runtime.Serialization;
using XFiresecAPI;
using System.Collections.Generic;

namespace FiresecAPI.Models
{
	[DataContract]
	public class FullConfiguration : VersionedConfiguration
	{
		public FullConfiguration()
		{
			DeviceConfiguration = new DeviceConfiguration();
			LibraryConfiguration = new LibraryConfiguration();
			PlansConfiguration = new PlansConfiguration();
			SecurityConfiguration = new SecurityConfiguration();
			SystemConfiguration = new SystemConfiguration();
			XDeviceConfiguration = new XDeviceConfiguration();
			XDriversConfiguration = new XDriversConfiguration();
		}

		[DataMember]
		public DeviceConfiguration DeviceConfiguration { get; set; }

		[DataMember]
		public LibraryConfiguration LibraryConfiguration { get; set; }

		[DataMember]
		public PlansConfiguration PlansConfiguration { get; set; }

		[DataMember]
		public SecurityConfiguration SecurityConfiguration { get; set; }

		[DataMember]
		public SystemConfiguration SystemConfiguration { get; set; }

		[DataMember]
		public XDeviceConfiguration XDeviceConfiguration { get; set; }

		[DataMember]
		public XDriversConfiguration XDriversConfiguration { get; set; }

		public override void ValidateVersion()
		{
			PlansConfiguration.ValidateVersion();
		}
	}
}