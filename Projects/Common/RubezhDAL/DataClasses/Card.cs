using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubezhDAL.DataClasses
{
	public class Card : IExternalKey
	{
		public Card()
		{
			PendingCards = new List<PendingCard>();
			CardDoors = new List<CardDoor>();
			GKControllerUIDs = new List<CardGKControllerUID>();
		}
		
		[Key]
		public Guid UID { get; set; }
		[Index]
		public Guid? EmployeeUID { get; set; }
		public Employee Employee { get; set; }
		[Index]
		public Guid? AccessTemplateUID { get; set; }
		public AccessTemplate AccessTemplate { get; set; }
		[Index]
		public Guid? PassCardTemplateUID { get; set; }
		public PassCardTemplate PassCardTemplate { get; set; }

		public ICollection<PendingCard> PendingCards { get; set; }

		public ICollection<CardDoor> CardDoors { get; set; }

		public ICollection<CardGKControllerUID> GKControllerUIDs { get; set; }
		
		public int Number { get; set; }

		public DateTime EndDate { get; set; }

		public bool IsInStopList { get; set; }
		[MaxLength(4000)]
		public string StopReason { get; set; }

		public int GKLevel { get; set; }

		public int GKLevelSchedule { get; set; }

		public int GKCardType { get; set; }

		[MaxLength(50)]
		public string ExternalKey { get; set; }

		[NotMapped]
		public bool IsDeleted
		{
			get { return false; }
			set { return; }
		}

		[NotMapped]
		public DateTime? RemovalDate
		{
			get { return null; }
			set { return; }
		}
	}
}
