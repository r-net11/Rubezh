using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class CardDoor : SKDModelBase
	{
		[DataMember]
		public Guid DoorUID { get; set; }

		[DataMember]
		public int EnterIntervalID { get; set; }

		[DataMember]
		public int ExitIntervalID { get; set; }

		[DataMember]
		public Guid? CardUID { get; set; }

		[DataMember]
		public Guid? AccessTemplateUID { get; set; }

		[DataMember]
		public CardSubsystemType CardSubsystemType { get; set; }
	}
}