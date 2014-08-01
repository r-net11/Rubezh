using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class CardDoorFilter : IsDeletedFilter
	{
		public CardDoorFilter()
			: base()
		{
			CardUIDs = new List<Guid>();
			DoorUIDs = new List<Guid>();
			IntervalIDs = new List<int>();
		}

		[DataMember]
		public List<Guid> CardUIDs { get; set; }

		[DataMember]
		public List<Guid> DoorUIDs { get; set; }

		[DataMember]
		public List<int> IntervalIDs { get; set; }
	}
}