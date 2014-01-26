using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDStates
	{
		public SKDStates()
		{
			DeviceStates = new List<SKDDeviceState>();
		}

		[DataMember]
		public List<SKDDeviceState> DeviceStates { get; set; }
	}
}