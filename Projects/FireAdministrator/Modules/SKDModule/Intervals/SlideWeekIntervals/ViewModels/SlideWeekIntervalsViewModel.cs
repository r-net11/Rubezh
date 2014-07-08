using System;
using Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using SKDModule.Intervals.Base.ViewModels;

namespace SKDModule.ViewModels
{
	public class SlideWeekIntervalsViewModel : BaseIntervalsViewModel<SlideWeekIntervalViewModel, SlideWeekIntervalPartViewModel, SKDSlideWeeklyInterval>
	{
		public ObservableCollection<SKDWeeklyInterval> AvailableWeekIntervals { get; private set; }

		public SlideWeekIntervalsViewModel()
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			var map = SKDManager.TimeIntervalsConfiguration.SlideWeeklyIntervals.ToDictionary(item => item.ID);
			Intervals = new ObservableCollection<SlideWeekIntervalViewModel>();
			for (int i = 1; i <= 128; i++)
				Intervals.Add(new SlideWeekIntervalViewModel(i, map.ContainsKey(i) ? map[i] : null, this));
			SelectedInterval = Intervals.First();
		}

		protected override void OnEdit()
		{
			var slideWeekIntervalDetailsViewModel = new SlideWeekIntervalDetailsViewModel(SelectedInterval.Model);
			if (DialogService.ShowModalWindow(slideWeekIntervalDetailsViewModel))
			{
				SelectedInterval.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		protected override SKDSlideWeeklyInterval CopyInterval(SKDSlideWeeklyInterval source)
		{
			return new SKDSlideWeeklyInterval()
			{
				Name = source.Name,
				Description = source.Description,
				StartDate = source.StartDate,
				WeeklyIntervalIDs = new List<int>(source.WeeklyIntervalIDs),
			};
		}

		protected override void BuildIntervals()
		{
			AvailableWeekIntervals = new ObservableCollection<SKDWeeklyInterval>(SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.OrderBy(item => item.ID));
			AvailableWeekIntervals.Insert(0, new SKDWeeklyInterval()
			{
				ID = 0,
				Name = "Никогда",
			});
			OnPropertyChanged(() => AvailableWeekIntervals);
			if (SelectedInterval != null)
				SelectedInterval.Parts.ForEach(item => item.Update());
		}
	}
}