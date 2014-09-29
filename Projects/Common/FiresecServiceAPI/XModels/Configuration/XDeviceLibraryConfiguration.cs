using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	public class XDeviceLibraryConfiguration : VersionedConfiguration
	{
		public XDeviceLibraryConfiguration()
		{
			XDevices = new List<LibraryXDevice>();
		}

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