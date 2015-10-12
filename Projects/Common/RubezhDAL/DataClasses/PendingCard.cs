using System;
using System.ComponentModel.DataAnnotations;

namespace RubezhDAL.DataClasses
{
	public class PendingCard
	{
		[Key]
		public Guid UID { get; set; }

		public Guid? CardUID { get; set; }
		public Card Card { get; set; }

		public Guid ControllerUID { get; set; }

		public int Action { get; set; }
	}
}
