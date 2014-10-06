using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKDeviceLibraryConfiguration : VersionedConfiguration
	{
		public GKDeviceLibraryConfiguration()
		{
			GKDevices = new List<GKLibraryDevice>();
		}

		[DataMember]
		public List<GKLibraryDevice> GKDevices { get; set; }

		public override bool ValidateVersion()
		{
			var result = true;
			foreach (var libraryDevice in GKDevices)
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