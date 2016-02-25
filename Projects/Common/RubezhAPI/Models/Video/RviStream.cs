using System;
using System.Runtime.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class RviStream
	{
		[DataMember]
		public int Number { get; set; }
		[DataMember]
		public Guid RviDeviceUid { get; set; }
		[DataMember]
		public int RviChannelNumber { get; set; }
	}
}