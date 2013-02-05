using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XPumpStationProperty
	{
		public XPumpStationProperty()
		{
			DeviceUIDs = new List<Guid>();
		}

		[DataMember]
		public List<Guid> DeviceUIDs { get; set; }
	}
}