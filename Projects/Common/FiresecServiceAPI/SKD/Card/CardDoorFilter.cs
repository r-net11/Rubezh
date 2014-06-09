using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class CardDoorFilter : IsDeletedFilter
	{
		[DataMember]
		public List<Guid> CardUIDs { get; set; }

		[DataMember]
		public List<Guid> DoorUIDs { get; set; }

		[DataMember]
		public List<Guid> IntervalUIDs { get; set; }

		public CardDoorFilter():base()
		{
			CardUIDs = new List<Guid>();
			DoorUIDs = new List<Guid>();
			IntervalUIDs = new List<Guid>();
		}
	}
}