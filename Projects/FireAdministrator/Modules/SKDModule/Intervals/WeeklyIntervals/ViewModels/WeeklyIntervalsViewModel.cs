using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Common;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using SKDModule.Intervals.Base.ViewModels;
using System;

namespace SKDModule.ViewModels
{
	public class WeeklyIntervalsViewModel : BaseIntervalsViewModel<WeeklyIntervalViewModel, WeeklyIntervalPartViewModel, SKDWeeklyInterval>
	{
		public ObservableCollection<SKDTimeInterval> AvailableTimeIntervals { get; private set; }
		public ObservableCollection<SKDHoliday> AvailableHolidays { get; private set; }

		public override void Initialize()
		{
			base.Initialize();
			var map = SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.ToDictionary(item => item.ID);
			Intervals = new ObservableCollection<WeeklyIntervalViewModel>();
			for (int i = 2; i <= 127; i++)
				Intervals.Add(new WeeklyIntervalViewModel(i, map.ContainsKey(i) ? map[i] : null, this));
			SelectedInterval = Intervals.FirstOrDefault();
		}

		protected override void OnEdit()
		{
			var weeklyIntervalDetailsViewModel = new WeeklyIntervalDetailsViewModel(SelectedInterval.Model);
			if (DialogService.ShowModalWindow(weeklyIntervalDetailsViewModel))
			{
				SelectedInterval.Update();
				ServiceFactory.SaveService.SKDChanged = true;
				ServiceFactory.SaveService.TimeIntervalChanged();
			}
		}

		protected override SKDWeeklyInterval CopyInterval(SKDWeeklyInterval source)
		{
			var copy = new SKDWeeklyInterval()
			{
				Name = source.Name,
				Description = source.Description,
			};
			for (int i = 0; i < source.WeeklyIntervalParts.Count; i++)
			{
				copy.WeeklyIntervalParts[i].TimeIntervalID = source.WeeklyIntervalParts[i].TimeIntervalID;
				copy.WeeklyIntervalParts[i].HolidayUID = source.WeeklyIntervalParts[i].HolidayUID;
			}
			return copy;
		}

		protected override void BuildIntervals()
		{
			AvailableTimeIntervals = new ObservableCollection<SKDTimeInterval>(SKDManager.TimeIntervalsConfiguration.TimeIntervals.OrderBy(item => item.ID));
			AvailableTimeIntervals.Insert(0, new SKDTimeInterval()
			{
				ID = 0,
				Name = "<Никогда>",
			});
			AvailableHolidays = new ObservableCollection<SKDHoliday>(SKDManager.TimeIntervalsConfiguration.Holidays.OrderBy(item => item.DateTime.DayOfYear));
			AvailableHolidays.Insert(0, new SKDHoliday()
			{
				UID = Guid.Empty,
			});
			OnPropertyChanged(() => AvailableTimeIntervals);
			if (SelectedInterval != null)
				SelectedInterval.Parts.ForEach(item => item.Update());
		}
	}
}