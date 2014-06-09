using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class CardDoor : SKDIsDeletedModel
	{
		[DataMember]
		public Guid DoorUID { get; set; }

		[DataMember]
		public bool IsAntiPassback { get; set; }

		[DataMember]
		public bool IsComission { get; set; }

		[DataMember]
		public IntervalType IntervalType { get; set; }

		[DataMember]
		public Guid? IntervalUID { get; set; }

		[DataMember]
		public Guid? ParentUID { get; set; }
	}
}