using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class GKCardDoor : SKDModelBase
	{
		[DataMember]
		public Guid DoorUID { get; set; }

		[DataMember]
		public int EnterIntervalID { get; set; }

		[DataMember]
		public IntervalType ExitIntervalType { get; set; }

		[DataMember]
		public int ExitIntervalID { get; set; }

		[DataMember]
		public Guid? CardUID { get; set; }

		[DataMember]
		public Guid? AccessTemplateUID { get; set; }
	}
}