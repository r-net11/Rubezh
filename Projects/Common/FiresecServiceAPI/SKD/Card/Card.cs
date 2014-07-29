using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDCard : SKDIsDeletedModel
	{
		public SKDCard()
		{
			CardDoors = new List<CardDoor>();
			CardType = CardType.Constant;
		}

		[DataMember]
		public int Number { get; set; }

		[DataMember]
		public Guid? HolderUID { get; set; }

		[DataMember]
		public DateTime StartDate { get; set; }

		[DataMember]
		public DateTime EndDate { get; set; }

		[DataMember]
		public List<CardDoor> CardDoors { get; set; }

		[DataMember]
		public Guid? CardTemplateUID { get; set; }

		[DataMember]
		public Guid? AccessTemplateUID { get; set; }

		[DataMember]
		public CardType CardType { get; set; }

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public bool IsInStopList { get; set; }

		[DataMember]
		public string StopReason { get; set; }

		[DataMember]
		public string EmployeeName { get; set; }
	}
}