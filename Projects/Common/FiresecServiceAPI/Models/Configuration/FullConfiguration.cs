using System.Runtime.Serialization;
using XFiresecAPI;

namespace FiresecAPI.Models
{
	[DataContract]
	public class FullConfiguration : VersionedConfiguration
	{
		public FullConfiguration()
		{
			DeviceConfiguration = new DeviceConfiguration();
			DeviceLibraryConfiguration = new DeviceLibraryConfiguration();
			PlansConfiguration = new PlansConfiguration();
			SecurityConfiguration = new SecurityConfiguration();
			SystemConfiguration = new SystemConfiguration();
			XDeviceConfiguration = new XDeviceConfiguration();
		}

		[DataMember]
		public DeviceConfiguration DeviceConfiguration { get; set; }

		[DataMember]
		public DeviceLibraryConfiguration DeviceLibraryConfiguration { get; set; }

		[DataMember]
		public PlansConfiguration PlansConfiguration { get; set; }

		[DataMember]
		public SecurityConfiguration SecurityConfiguration { get; set; }

		[DataMember]
		public SystemConfiguration SystemConfiguration { get; set; }

		[DataMember]
		public XDeviceConfiguration XDeviceConfiguration { get; set; }

		public override bool ValidateVersion()
		{
			return DeviceConfiguration.ValidateVersion() &&
			XDeviceConfiguration.ValidateVersion() &&
			PlansConfiguration.ValidateVersion();
		}
	}
}