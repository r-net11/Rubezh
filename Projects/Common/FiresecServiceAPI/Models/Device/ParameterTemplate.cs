using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ParameterTemplate
	{
		public ParameterTemplate()
		{
			UID = Guid.NewGuid();
			DeviceParameterTemplates = new List<DeviceParameterTemplate>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public List<DeviceParameterTemplate> DeviceParameterTemplates { get; set; }
	}
}