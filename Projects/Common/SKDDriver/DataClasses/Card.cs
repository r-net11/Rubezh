using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class Card
	{
		[Key]
		public Guid UID { get; set; }

		public Guid? EmployeeUID { get; set; }
		public Employee Employee { get; set; }

		public Guid? AccessTemplateUID { get; set; }
		public AccessTemplate AccessTemplate { get; set; }

		public Guid? PassCardTemplateUID { get; set; }
		public PassCardTemplate PassCardTemplate { get; set; }

		public ICollection<PendingCard> PendingCards { get; set; }

		public ICollection<CardDoor> CardDoors { get; set; }

		public int Number { get; set; }

		public int CardType { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public bool InInStopList { get; set; }

		public string StopReason { get; set; }

		public string Password { get; set; }

		public Guid? DeactivationControllerUID { get; set; }

		public int UserTime { get; set; }

		public int GKLevel { get; set; }

		public int GKLevelSchedule { get; set; }

		public int GKCardType { get; set; }

		public string ExternalKey { get; set; }
	}
}
