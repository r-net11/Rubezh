using System;
using System.ComponentModel.DataAnnotations;

namespace RubezhDAL.DataClasses
{
	public class CardGKControllerUID
	{
		[Key]
		public Guid UID { get; set; }

		public Guid? CardUID { get; set; }
		public Card Card { get; set; }

		public Guid GKControllerUID { get; set; }
	}
}