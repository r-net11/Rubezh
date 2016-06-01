using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Common;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using StrazhModule.Intervals.Base.ViewModels;
using System;

namespace StrazhModule.ViewModels
{
	public class DoorWeeklyIntervalsViewModel : BaseIntervalsViewModel<DoorWeeklyIntervalViewModel, DoorWeeklyIntervalPartViewModel, SKDDoorWeeklyInterval>
	{
		public ObservableCollection<SKDDoorDayInterval> AvailableDayIntervals { get; private set; }

		public override void Initialize()
		{
			base.Initialize();
			var map = SKDManager.TimeIntervalsConfiguration.DoorWeeklyIntervals.ToDictionary(item => item.ID);
			Intervals = new ObservableCollection<DoorWeeklyIntervalViewModel>();
			for (int i = 0; i <= 127; i++)
				Intervals.Add(new DoorWeeklyIntervalViewModel(i, map.ContainsKey(i) ? map[i] : null, this));
			SelectedInterval = Intervals.FirstOrDefault();
		}

		protected override void OnEdit()
		{
			var weeklyIntervalDetailsViewModel = new DoorWeeklyIntervalDetailsViewModel(SelectedInterval.Model);
			if (DialogService.ShowModalWindow(weeklyIntervalDetailsViewModel))
			{
				SelectedInterval.Update();
				ServiceFactory.SaveService.SKDChanged = true;
				ServiceFactory.SaveService.TimeIntervalChanged();
			}
		}

		protected override SKDDoorWeeklyInterval CopyInterval(SKDDoorWeeklyInterval source)
		{
			var copy = new SKDDoorWeeklyInterval
			{
				Name = source.Name,
				Description = source.Description,
			};
			for (int i = 0; i < source.WeeklyIntervalParts.Count; i++)
			{
				copy.WeeklyIntervalParts[i].DayIntervalUID = source.WeeklyIntervalParts[i].DayIntervalUID;
			}
			return copy;
		}

		protected override void BuildIntervals()
		{
			AvailableDayIntervals = new ObservableCollection<SKDDoorDayInterval>(SKDManager.TimeIntervalsConfiguration.DoorDayIntervals);
			OnPropertyChanged(() => AvailableDayIntervals);
			if (SelectedInterval != null)
				SelectedInterval.Parts.ForEach(item => item.Update());
		}
	}
}