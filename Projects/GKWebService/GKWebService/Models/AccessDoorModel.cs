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
		public List<CardScheduleItemModel> EnterSchedules { get; set; }
		public CardScheduleItemModel SelectedEnterSchedule { get; set; }
		public int? SelectedEnterScheduleNo { get; set; }
		public List<CardScheduleItemModel> ExitSchedules { get; set; }
		public CardScheduleItemModel SelectedExitSchedule { get; set; }
		public int? SelectedExitScheduleNo { get; set; }

		public AccessDoorModel()
		{
		}

		public AccessDoorModel(GKDoor door, List<CardDoor> cardDoors, List<GKSchedule> schedules)
		{
			DoorUID = door.UID;
			PresentationName = door.PresentationName;
			HasExit = door.DoorType != GKDoorType.OneWay;

			EnterSchedules = new List<CardScheduleItemModel>();
			ExitSchedules = new List<CardScheduleItemModel>();
			if (schedules != null)
			{
				EnterSchedules = new List<CardScheduleItemModel>(from o in schedules
																			orderby o.No ascending
																			select new CardScheduleItemModel(o.No, o.Name));
				ExitSchedules = new List<CardScheduleItemModel>(from o in schedules
																		   orderby o.No ascending
																		   select new CardScheduleItemModel(o.No, o.Name));
			}

			var cardDoor = cardDoors.FirstOrDefault(x => x.DoorUID == DoorUID);
			CardScheduleItemModel selectedEnterSchedule;
			CardScheduleItemModel selectedExitSchedule;
			if (cardDoor != null)
			{
				IsChecked = true;
				selectedEnterSchedule = EnterSchedules.FirstOrDefault(x => x.ScheduleNo == cardDoor.EnterScheduleNo);
				if (selectedEnterSchedule == null)
					selectedEnterSchedule = EnterSchedules.FirstOrDefault();
				selectedExitSchedule = ExitSchedules.FirstOrDefault(x => x.ScheduleNo == cardDoor.ExitScheduleNo);
				if (selectedExitSchedule == null)
					selectedExitSchedule = ExitSchedules.FirstOrDefault();
			}
			else
			{
				selectedEnterSchedule = EnterSchedules.FirstOrDefault();
				selectedExitSchedule = ExitSchedules.FirstOrDefault();
			}
			SelectedEnterSchedule = selectedEnterSchedule;
			SelectedExitSchedule = selectedExitSchedule;
		}
	}

	public class CardScheduleItemModel
	{
		public CardScheduleItemModel()
		{
		}

		public CardScheduleItemModel(int scheduleNo, string name)
		{
			ScheduleNo = scheduleNo;
			Name = name;
		}

		public int ScheduleNo { get; set; }
		public string Name { get; set; }
	}
}