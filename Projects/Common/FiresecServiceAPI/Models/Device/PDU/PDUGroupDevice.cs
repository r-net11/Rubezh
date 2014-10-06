using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class PDUGroupDevice
	{
		[XmlIgnore]
		public Device Device { get; set; }

		[DataMember]
		public Guid DeviceUID { get; set; }

		[DataMember]
		public bool IsInversion { get; set; }

		[DataMember]
		public int OnDelay { get; set; }

		[DataMember]
		public int OffDelay { get; set; }
	}
}