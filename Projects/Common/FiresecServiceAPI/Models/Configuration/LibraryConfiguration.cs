using System;
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

		public override bool ValidateVersion()
		{
			var result = true;
			foreach (var libraryDevice in Devices)
			{
				if (libraryDevice.UID == Guid.Empty)
				{
					libraryDevice.UID = Guid.NewGuid();
					result = false;
				}
			}
			return result;
		}
	}
}