using System;
using System.Linq;
using Localization.SKD.ViewModels;
using StrazhAPI.GK;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ReadOnlyAccessDoorViewModel : BaseViewModel
	{
		public int No { get; set; }
		public string Name { get; set; }
		public string EnerScheduleName { get; private set; }
		public bool HasEnter { get; private set; }
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
				EnerScheduleName = string.Format(CommonViewModels.ScheduleDisable, cardDoor.EnterScheduleNo);
			}

			HasEnter = door.InDeviceUID != Guid.Empty;
		}
	}
}