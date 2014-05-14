using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class XParameterTemplate
	{
		public XParameterTemplate()
		{
			UID = Guid.NewGuid();
			DeviceParameterTemplates = new List<XDeviceParameterTemplate>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public int No { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public List<XDeviceParameterTemplate> DeviceParameterTemplates { get; set; }
	}
}