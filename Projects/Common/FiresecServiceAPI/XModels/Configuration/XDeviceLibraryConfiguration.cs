using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FiresecAPI;

namespace XFiresecAPI
{
    [DataContract]
    public class XDeviceLibraryConfiguration: VersionedConfiguration
	{
		public XDeviceLibraryConfiguration()
		{
			XDevices = new List<LibraryXDevice>();
		}

		[DataMember]
		public List<LibraryXDevice> XDevices { get; set; }
	}
}
