using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
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
		public uint? Number { get; set; }

		[DataMember]
		public Guid? HolderUID { get; set; }

		[DataMember]
		public DateTime StartDate { get; set; }

		[DataMember]
		public DateTime EndDate { get; set; }

		[DataMember]
		public int UserTime { get; set; }

		[DataMember]
		public Guid DeactivationControllerUID { get; set; }

		[DataMember]
		public List<CardDoor> CardDoors { get; set; }

		[DataMember]
		public Guid? PassCardTemplateUID { get; set; }

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

		[DataMember]
		public Guid EmployeeUID { get; set; }

		[DataMember]
		public Guid OrganisationUID { get; set; }

		[DataMember]
		public bool IsHandicappedCard { get; set; }

		[DataMember]
		public int? AllowedPassCount { get; set; }
	}
}