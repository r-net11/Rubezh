using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class DeviceParameterTemplate
	{
		[DataMember]
		public Device Device { get; set; }
	}
}
