using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKParameterTemplate
	{
		public GKParameterTemplate()
		{
			UID = Guid.NewGuid();
			DeviceParameterTemplates = new List<GKDeviceParameterTemplate>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public int No { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public List<GKDeviceParameterTemplate> DeviceParameterTemplates { get; set; }
	}
}