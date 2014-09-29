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

		public Guid UID { get; set; }
		public int No { get; set; }
		public string Name { get; set; }
		public List<XDeviceParameterTemplate> DeviceParameterTemplates { get; set; }
	}
}