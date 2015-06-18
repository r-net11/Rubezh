using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class CardDoor
	{
		public CardDoor(FiresecAPI.SKD.CardDoor apiItem)
		{
			UID = apiItem.UID;
			TranslateBack(apiItem);
		}
		
		[Key]
		public Guid UID { get; set; }

		public Guid? CardUID { get; set; }
		public Card Card { get; set; }

		public Guid? AccessTemplateUID { get; set; }
		public AccessTemplate AccessTemplate { get; set; }

		public Guid DoorUID { get; set; }

		public int EnterScheduleNo { get; set; }

		public int ExitScheduleNo { get; set; }

		public FiresecAPI.SKD.CardDoor Translate()
		{
			return new FiresecAPI.SKD.CardDoor
			{
				UID = UID,
				CardUID = CardUID,
				DoorUID = DoorUID,
				AccessTemplateUID = AccessTemplateUID,
				EnterScheduleNo = ExitScheduleNo,
				ExitScheduleNo = EnterScheduleNo
			};
		}

		public void TranslateBack(FiresecAPI.SKD.CardDoor apiItem)
		{
			CardUID = apiItem.CardUID;
			DoorUID = apiItem.DoorUID;
			AccessTemplateUID = apiItem.AccessTemplateUID;
			EnterScheduleNo = apiItem.EnterScheduleNo;
			ExitScheduleNo = apiItem.ExitScheduleNo;
		}
	}
}
