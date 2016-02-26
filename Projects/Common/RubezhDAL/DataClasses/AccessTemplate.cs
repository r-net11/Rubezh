using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubezhDAL.DataClasses
{
	public class AccessTemplate : IOrganisationItem
	{
		public AccessTemplate()
		{
			Cards = new List<Card>();
			//PendingCards = new List<PendingCard>();
			CardDoors = new List<CardDoor>();
			//CardGKControllerUIDs = new List<CardGKControllerUID>();
		}

		#region IOrganisationItemMembers
		[Key]
		public Guid UID { get; set; }
		[MaxLength(50)]
		public string Name { get; set; }
		[MaxLength(4000)]
		public string Description { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime? RemovalDate { get; set; }
		[Index]
		public Guid? OrganisationUID { get; set; }
		public Organisation Organisation { get; set; }
		#endregion

		[InverseProperty("AccessTemplate")]
		public ICollection<Card> Cards { get; set; }

		//public ICollection<PendingCard> PendingCards { get; set; }

		[InverseProperty("AccessTemplate")]
		public ICollection<CardDoor> CardDoors { get; set; }

		//public ICollection<CardGKControllerUID> CardGKControllerUIDs { get; set; }
	}
}

