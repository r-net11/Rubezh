using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.GK
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
			//TODO: mde
			return result;
		}
	}
}