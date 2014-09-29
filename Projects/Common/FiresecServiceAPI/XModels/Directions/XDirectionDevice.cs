using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	public class XDirectionDevice
	{
		[XmlIgnore]
		public XDevice Device { get; set; }

		public Guid DeviceUID { get; set; }
		public XStateBit StateBit { get; set; }
	}
}