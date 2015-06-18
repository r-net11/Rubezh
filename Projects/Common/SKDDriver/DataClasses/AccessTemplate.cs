using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class AccessTemplate : IOrganisationItem
	{
		public AccessTemplate()
		{
			Cards = new List<Card>();
			PendingCards = new List<PendingCard>();
			CardDoors = new List<CardDoor>();
			CardGKControllerUIDs = new List<CardGKControllerUID>();
		}
		
		#region IOrganisationItemMembers
		[Key]
		public Guid UID { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime? RemovalDate { get; set; }

		public Guid? OrganisationUID { get; set; }
		public Organisation Organisation { get; set; }
		#endregion

		public ICollection<Card> Cards { get; set; }

		public ICollection<PendingCard> PendingCards { get; set; }

		public ICollection<CardDoor> CardDoors { get; set; }

		public ICollection<CardGKControllerUID> CardGKControllerUIDs { get; set; }
	}
}

