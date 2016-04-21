using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using RubezhAPI.GK;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class SKDCard : SKDIsDeletedModel, IHRListItem
	{
		public SKDCard()
		{
			CardDoors = new List<CardDoor>();
			GKCardType = GK.GKCardType.Employee;
			GKControllerUIDs = new List<Guid>();
		}

		[DataMember]
		public uint Number { get; set; }

		[DataMember]
		public Guid? EmployeeUID { get; set; }

		[DataMember]
		public DateTime EndDate { get; set; }

		[DataMember]
		public List<CardDoor> CardDoors { get; set; }

		[DataMember]
		public Guid? AccessTemplateUID { get; set; }

		[DataMember]
		public GKCardType GKCardType { get; set; }

		[DataMember]
		public bool IsInStopList { get; set; }

		[DataMember]
		public string StopReason { get; set; }

		[DataMember]
		public string EmployeeName { get; set; }

		[DataMember]
		public Guid OrganisationUID { get; set; }

		[DataMember]
		public int GKLevel { get; set; }

		[DataMember]
		public int GKLevelSchedule { get; set; }

		[DataMember]
		public List<Guid> GKControllerUIDs { get; set; }

		public int NumberInt { set { Number = (uint)value; } }

		public string Name { get { return Number.ToString(); } }

		public string ImageSource { get { return "/Controls;component/Images/Card.png"; } }
	}

	public class CardAccessTemplateDoors
	{
		public CardAccessTemplateDoors()
		{
			CardDoors = new List<CardDoor>();
		}
		public Guid CardUID { get; set; }

		public List<CardDoor> CardDoors { get; set; }
	}
}