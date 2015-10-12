using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace RubezhDAL.DataClasses
{
	public class CardDoor
	{
		public CardDoor() { }

		public CardDoor(RubezhAPI.SKD.CardDoor apiItem)
		{
			UID = apiItem.UID;
			TranslateBack(apiItem);
		}

		[Key]
		public Guid UID { get; set; }
		[Index]
		public Guid? CardUID { get; set; }
		public Card Card { get; set; }
		[Index]
		public Guid? AccessTemplateUID { get; set; }
		public AccessTemplate AccessTemplate { get; set; }

		public Guid DoorUID { get; set; }

		public int EnterScheduleNo { get; set; }

		public int ExitScheduleNo { get; set; }

		public RubezhAPI.SKD.CardDoor Translate()
		{
			return new RubezhAPI.SKD.CardDoor
			{
				UID = UID,
				CardUID = CardUID,
				DoorUID = DoorUID,
				AccessTemplateUID = AccessTemplateUID,
				EnterScheduleNo = ExitScheduleNo,
				ExitScheduleNo = EnterScheduleNo
			};
		}

		public static IEnumerable<RubezhAPI.SKD.CardDoor> GetAPIItems(ICollection<CardDoor> tableItems)
		{
			return tableItems.Select(x => new RubezhAPI.SKD.CardDoor
			{
				UID = x.UID,
				CardUID = x.CardUID,
				DoorUID = x.DoorUID,
				AccessTemplateUID = x.AccessTemplateUID,
				EnterScheduleNo = x.ExitScheduleNo,
				ExitScheduleNo = x.EnterScheduleNo
			});
		}

		public void TranslateBack(RubezhAPI.SKD.CardDoor apiItem)
		{
			CardUID = apiItem.CardUID;
			DoorUID = apiItem.DoorUID;
			AccessTemplateUID = apiItem.AccessTemplateUID;
			EnterScheduleNo = apiItem.EnterScheduleNo;
			ExitScheduleNo = apiItem.ExitScheduleNo;
		}
	}
}