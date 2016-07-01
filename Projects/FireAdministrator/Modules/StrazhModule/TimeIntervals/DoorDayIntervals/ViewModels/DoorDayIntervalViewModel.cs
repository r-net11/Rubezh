using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using Localization.Strazh.ViewModels;
using StrazhAPI;
using StrazhAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Events;
using StrazhModule.Events;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class DoorDayIntervalViewModel : BaseViewModel
	{
		public SKDDoorDayInterval DayInterval { get; private set; }
		public DoorDayIntervalPartsViewModel DayIntervalPartsViewModel { get; private set; }

		public DoorDayIntervalViewModel(SKDDoorDayInterval dayInterval)
		{
			DayInterval = dayInterval;
			DayIntervalPartsViewModel = new DoorDayIntervalPartsViewModel(dayInterval);
			Update(DayInterval);
		}

		public void Update(SKDDoorDayInterval dayInterval)
		{
			DayInterval = dayInterval;
			OnPropertyChanged(() => DayInterval);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
		}

		public string Name
		{
			get { return DayInterval.Name; }
		}

		public string Description
		{
			get { return DayInterval.Description; }
		}

		public bool IsEnabled
		{
			get
			{
				return Name != TimeIntervalsConfiguration.PredefinedIntervalNameCard
					&& Name != TimeIntervalsConfiguration.PredefinedIntervalNamePassword
					&& Name != TimeIntervalsConfiguration.PredefinedIntervalNameCardAndPassword;
			}
		}

		public bool ConfirmRemoval()
		{
			var hasReference = SKDManager.TimeIntervalsConfiguration.DoorWeeklyIntervals.Any(item => item.WeeklyIntervalParts.Any(part => part.DayIntervalUID == DayInterval.UID));
			return hasReference
				? MessageBoxService.ShowQuestion(String.Format(CommonViewModels.DayLockSchedule_DeleteForWeekConfirm, Name), null, MessageBoxImage.Warning)
				: MessageBoxService.ShowQuestion(String.Format(CommonViewModels.DayLockSchedule_DeleteConfirm,Name));
		}

		public IEnumerable<SKDDoorWeeklyInterval> GetLinkedWeeklyIntervals()
		{
			return SKDManager.TimeIntervalsConfiguration.DoorWeeklyIntervals.Where(item => item.WeeklyIntervalParts.Any(part => part.DayIntervalUID == DayInterval.UID)).ToList();
		}
	}
}