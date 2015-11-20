using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI.GK;
using RubezhAPI.SKD;

namespace GKWebService.Models
{
	public class AccessDoorModel
	{
		public Guid DoorUID { get; set; }
		public string PresentationName { get; set; }
		public bool IsChecked { get; set; }
		public bool IsSelected { get; set; }
		public bool HasExit { get; private set; }
		public List<CardScheduleItem> EnterSchedules { get; set; }
		public CardScheduleItem SelectedEnterSchedule { get; set; }
		public List<CardScheduleItem> ExitSchedules { get; set; }
		public CardScheduleItem SelectedExitSchedule { get; set; }

		public AccessDoorModel(GKDoor door, List<CardDoor> cardDoors, List<GKSchedule> schedules)
		{
			DoorUID = door.UID;
			PresentationName = door.PresentationName;
			HasExit = door.DoorType != GKDoorType.OneWay;

			EnterSchedules = new List<CardScheduleItem>();
			ExitSchedules = new List<CardScheduleItem>();
			if (schedules != null)
			{
				EnterSchedules = new List<CardScheduleItem>(from o in schedules
																			orderby o.No ascending
																			select new CardScheduleItem(o.No, o.Name));
				ExitSchedules = new List<CardScheduleItem>(from o in schedules
																		   orderby o.No ascending
																		   select new CardScheduleItem(o.No, o.Name));
			}

			var cardDoor = cardDoors.FirstOrDefault(x => x.DoorUID == DoorUID);
			if (cardDoor != null)
			{
				IsChecked = true;
				SelectedEnterSchedule = EnterSchedules.FirstOrDefault(x => x.ScheduleNo == cardDoor.EnterScheduleNo);
				if (SelectedEnterSchedule == null)
					SelectedEnterSchedule = EnterSchedules.FirstOrDefault();
				SelectedExitSchedule = ExitSchedules.FirstOrDefault(x => x.ScheduleNo == cardDoor.ExitScheduleNo);
				if (SelectedExitSchedule == null)
					SelectedExitSchedule = ExitSchedules.FirstOrDefault();
			}
			else
			{
				SelectedEnterSchedule = EnterSchedules.FirstOrDefault();
				SelectedExitSchedule = ExitSchedules.FirstOrDefault();
			}
		}
	}

	public class CardScheduleItem
	{
		public CardScheduleItem(int scheduleNo, string name)
		{
			ScheduleNo = scheduleNo;
			Name = name;
		}

		public int ScheduleNo { get; private set; }
		public string Name { get; private set; }
	}
}