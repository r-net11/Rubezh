using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class CardDoor
	{
		[Key]
		public Guid UID { get; set; }

		public Guid? CardUID { get; set; }
		public Card Card { get; set; }

		public Guid? AccessTemplateUID { get; set; }
		public AccessTemplate AccessTemplate { get; set; }

		public Guid DoorUID { get; set; }

		public int EnterScheduleNo { get; set; }

		public int ExitScheduleNo { get; set; }
	}
}
