using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI.GK;
using RubezhAPI.SKD;

namespace GKWebService.Models
{
	public class ReadOnlyAccessDoorModel
	{
		public string PresentationName { get; set; }
		public string EnerScheduleName { get; set; }
		public string ExitScheduleName { get; set; }
		public bool HasEnter { get; set; }
		public bool HasExit { get; set; }
		public CardDoor CardDoor { get; set; }

		public ReadOnlyAccessDoorModel(GKDoor door, CardDoor cardDoor, List<GKSchedule> schedules)
		{
			PresentationName = door.PresentationName;
			CardDoor = cardDoor;

			if (schedules != null)
			{
				var enterSchedule = schedules.FirstOrDefault(x => x.No == cardDoor.EnterScheduleNo);
				if (enterSchedule != null)
				{
					EnerScheduleName = enterSchedule.Name;
				}
				var exitSchedule = schedules.FirstOrDefault(x => x.No == cardDoor.ExitScheduleNo);
				if (exitSchedule != null && door.DoorType != GKDoorType.OneWay)
				{
					ExitScheduleName = exitSchedule.Name;
				}
			}
			HasEnter = door.EnterDeviceUID != Guid.Empty;
			HasExit = door.ExitDeviceUID != Guid.Empty && door.DoorType == GKDoorType.TwoWay;
		}

		public ReadOnlyAccessDoorModel()
		{
		}
	}
}