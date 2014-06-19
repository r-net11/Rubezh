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
		public IntervalType EnterIntervalType { get; set; }

		[DataMember]
		public Guid? EnterIntervalUID { get; set; }

		[DataMember]
		public IntervalType ExitIntervalType { get; set; }

		[DataMember]
		public Guid? ExitIntervalUID { get; set; }

		[DataMember]
		public Guid? ParentUID { get; set; }
	}
}