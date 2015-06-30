using System.Runtime.Serialization;
using FiresecAPI.SKD;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ServerTask
	{
		[DataMember]
		public string DeviceName { get; set; }

		[DataMember]
		public string DeviceAddress { get; set; }

		[DataMember]
		public int CardNumber { get; set; }

		[DataMember]
		public PendingCardAction PendingCardAction { get; set; }
	}
}