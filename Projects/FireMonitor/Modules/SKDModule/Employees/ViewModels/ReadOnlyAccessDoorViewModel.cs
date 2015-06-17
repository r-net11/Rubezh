using System;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ReadOnlyAccessDoorViewModel : BaseViewModel
	{
		public int No { get; set; }
		public string Name { get; set; }
		public string EnerScheduleName { get; private set; }
		public string ExitScheduleName { get; private set; }
		public bool HasEnter { get; private set; }
		public bool HasExit { get; private set; }
		public CardDoor CardDoor { get; private set; }

		public ReadOnlyAccessDoorViewModel(SKDDoor door, CardDoor cardDoor)
		{
			No = door.No;
			Name = door.Name;
			CardDoor = cardDoor;

			var enterSchedule = SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.FirstOrDefault(x => x.ID == cardDoor.EnterScheduleNo);
			if (enterSchedule != null)
			{
				EnerScheduleName = enterSchedule.Name;
			}
			else
			{
				EnerScheduleName = "График " + cardDoor.EnterScheduleNo + " деактивирован";
			}

			HasEnter = door.InDeviceUID != Guid.Empty;
			HasExit = false;
			ExitScheduleName = "";
		}

		public ReadOnlyAccessDoorViewModel(GKDoor door, CardDoor cardDoor)
		{
			No = door.No;
			Name = door.Name;
			CardDoor = cardDoor;

			var schedules = GKScheduleHelper.GetSchedules();
			if (schedules != null)
			{
				var enterSchedule = schedules.FirstOrDefault(x => x.No == cardDoor.EnterScheduleNo);
				if (enterSchedule != null)
				{
					EnerScheduleName = enterSchedule.Name;
				}
				var exitSchedule = schedules.FirstOrDefault(x => x.No == cardDoor.ExitScheduleNo);
				if (exitSchedule != null)
				{
					ExitScheduleName = exitSchedule.Name;
				}
			}
			HasEnter = door.EnterDeviceUID != Guid.Empty;
			HasExit = door.ExitDeviceUID != Guid.Empty && door.DoorType == GKDoorType.TwoWay;
		}
	}
}