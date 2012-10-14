using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class DeviceLibraryConfiguration : VersionedConfiguration
	{
		public DeviceLibraryConfiguration()
		{
			Devices = new List<LibraryDevice>();
		}

		[DataMember]
		public List<LibraryDevice> Devices { get; set; }
	}
}