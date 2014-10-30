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
		public int EnterScheduleNo { get; set; }

		[DataMember]
		public int ExitScheduleNo { get; set; }

		[DataMember]
		public Guid? CardUID { get; set; }

		[DataMember]
		public Guid? AccessTemplateUID { get; set; }
	}
}