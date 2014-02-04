using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class CardZoneLink
	{
		[DataMember]
		public Guid Uid { get; set; }
		[DataMember]
		public Guid? CardUid { get; set; }
		[DataMember]
		public Guid? ZoneUid { get; set; }
	}
}
