using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI;

namespace XFiresecAPI
{
	[DataContract]
	public class XDeviceLibraryConfiguration : VersionedConfiguration
	{
		public XDeviceLibraryConfiguration()
		{
			XDevices = new List<LibraryXDevice>();
		}

		[DataMember]
		public List<LibraryXDevice> XDevices { get; set; }

		public override bool ValidateVersion()
		{
			var result = true;
			foreach (var libraryDevice in XDevices)
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