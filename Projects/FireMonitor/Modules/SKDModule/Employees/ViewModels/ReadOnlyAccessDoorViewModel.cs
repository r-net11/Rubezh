using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ReadOnlyAccessDoorViewModel : BaseViewModel
	{
		public string PresentationName { get; private set; }
		public string EnerScheduleName { get; private set; }
		public string ExitScheduleName { get; private set; }
		public bool HasExit { get; private set; }
		public CardDoor CardDoor { get; private set; } 

		public ReadOnlyAccessDoorViewModel(GKDoor door, CardDoor cardDoor, List<GKSchedule> schedules)
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
			HasExit = door.DoorType == GKDoorType.TwoWay;
		}
	}
}