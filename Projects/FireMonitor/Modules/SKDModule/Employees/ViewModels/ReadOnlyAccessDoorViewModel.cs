using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;

namespace SKDModule.ViewModels
{
	public class ReadOnlyAccessDoorViewModel : BaseViewModel
	{
		public SKDDoor Door { get; private set; }
		public string EnerScheduleName { get; private set; }
		public string ExitScheduleName { get; private set; }

		public ReadOnlyAccessDoorViewModel(SKDDoor door, CardDoor cardDoor)
		{
			Door = door;

			var enterWeeklyInterval = SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.FirstOrDefault(x => x.ID == cardDoor.EnterIntervalID);
			if (enterWeeklyInterval != null)
			{
				EnerScheduleName = enterWeeklyInterval.Name;
			}
			var exitWeeklyInterval = SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.FirstOrDefault(x => x.ID == cardDoor.ExitIntervalID);
			if (exitWeeklyInterval != null)
			{
				ExitScheduleName = exitWeeklyInterval.Name;
			}
		}

		public bool HasEnter
		{
			get { return Door.InDeviceUID != Guid.Empty; }
		}

		public bool HasExit
		{
			get { return Door.OutDeviceUID != Guid.Empty && Door.DoorType == DoorType.TwoWay; }
		}
	}
}