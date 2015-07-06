using FiresecAPI.SKD;
using System.Runtime.Serialization;

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