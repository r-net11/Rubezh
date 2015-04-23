using System;
using System.Linq;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Events;
using GKModule.Events;
using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class DayScheduleViewModel : BaseViewModel
	{
		public GKDaySchedule DaySchedule { get; private set; }
		public DaySchedulePartsViewModel DaySchedulePartsViewModel { get; private set; }

		public DayScheduleViewModel(GKDaySchedule daySchedule)
		{
			DaySchedule = daySchedule;
			DaySchedulePartsViewModel = new DaySchedulePartsViewModel(daySchedule);
			Update(DaySchedule);
		}

		public void Update(GKDaySchedule daySchedule)
		{
			DaySchedule = daySchedule;
			OnPropertyChanged(() => DaySchedule);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
		}

		public string Name
		{
			get { return DaySchedule.Name; }
		}

		public string Description
		{
			get { return DaySchedule.Description; }
		}

		public bool IsEnabled
		{
			get
			{
				return Name != "<Никогда>" && Name != "<Всегда>";
			}
		}

		public bool ConfirmDeactivation()
		{
			return true;
			//var hasReference = GKManager.DeviceConfiguration.WeeklyIntervals.Any(item => item.WeeklyIntervalParts.Any(part => part.DayIntervalUID == DayInterval.UID));
			//return !hasReference || MessageBoxService.ShowConfirmation("Данный дневной график используется в одном или нескольких недельных графиках, Вы уверены что хотите его деактивировать?");
		}
	}
}