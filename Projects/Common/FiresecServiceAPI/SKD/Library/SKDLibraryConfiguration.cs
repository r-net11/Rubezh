using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI;

namespace XFiresecAPI
{
	[DataContract]
	public class SKDLibraryConfiguration : VersionedConfiguration
	{
		public SKDLibraryConfiguration()
		{
			Devices = new List<SKDLibraryDevice>();
		}

		[DataMember]
		public List<SKDLibraryDevice> Devices { get; set; }

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