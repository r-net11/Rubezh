using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class ReadOnlyAccessDoorViewModel : BaseViewModel
	{
		public string PresentationName { get; private set; }
		public string EnerScheduleName { get; private set; }
		public string ExitScheduleName { get; private set; }

		public ReadOnlyAccessDoorViewModel(SKDDoor door, CardDoor cardDoor)
		{
			PresentationName = door.PresentationName;

			var enterSchedule = SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.FirstOrDefault(x => x.ID == cardDoor.EnterIntervalID);
			if (enterSchedule != null)
			{
				EnerScheduleName = enterSchedule.Name;
			}
			var exitSchedule = SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.FirstOrDefault(x => x.ID == cardDoor.ExitIntervalID);
			if (exitSchedule != null)
			{
				ExitScheduleName = exitSchedule.Name;
			}

			HasEnter = door.InDeviceUID != Guid.Empty;
			HasExit = door.OutDeviceUID != Guid.Empty && door.DoorType == DoorType.TwoWay;
		}

		public ReadOnlyAccessDoorViewModel(GKDoor door, CardDoor cardDoor)
		{
			PresentationName = door.PresentationName;

			var enterSchedule = GKManager.DeviceConfiguration.Schedules.FirstOrDefault(x => x.No == cardDoor.EnterIntervalID);
			if (enterSchedule != null)
			{
				EnerScheduleName = enterSchedule.Name;
			}
			var exitSchedule = GKManager.DeviceConfiguration.Schedules.FirstOrDefault(x => x.No == cardDoor.ExitIntervalID);
			if (exitSchedule != null)
			{
				ExitScheduleName = exitSchedule.Name;
			}

			HasEnter = door.EnterDeviceUID != Guid.Empty;
			HasExit = door.ExitDeviceUID != Guid.Empty && door.DoorType == GKDoorType.TwoWay;
		}

		public bool HasEnter { get; private set; }
		public bool HasExit { get; private set; }
	}
}