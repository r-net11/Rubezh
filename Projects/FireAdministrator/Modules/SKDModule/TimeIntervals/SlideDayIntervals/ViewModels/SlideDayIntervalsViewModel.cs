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
using Common;
using SKDModule.Intervals.Base.ViewModels;

namespace SKDModule.ViewModels
{
	public class SlideDayIntervalsViewModel : BaseIntervalsViewModel<SlideDayIntervalViewModel, SlideDayIntervalPartViewModel, SKDSlideDayInterval>
	{
		public ObservableCollection<SKDDayInterval> AvailableTimeIntervals { get; private set; }

		public override void Initialize()
		{
			BuildIntervals();
			var map = SKDManager.TimeIntervalsConfiguration.SlideDayIntervals.ToDictionary(item => item.ID);
			Intervals = new ObservableCollection<SlideDayIntervalViewModel>();
			for (int i = 1; i <= 128; i++)
				Intervals.Add(new SlideDayIntervalViewModel(i, map.ContainsKey(i) ? map[i] : null, this));
			SelectedInterval = Intervals.FirstOrDefault();
		}

		protected override void OnEdit()
		{
			var slideDayIntervalDetailsViewModel = new SlideDayIntervalDetailsViewModel(SelectedInterval.Model);
			if (DialogService.ShowModalWindow(slideDayIntervalDetailsViewModel))
			{
				SelectedInterval.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		protected override SKDSlideDayInterval CopyInterval(SKDSlideDayInterval source)
		{
			return new SKDSlideDayInterval()
			{
				Name = source.Name,
				Description = source.Description,
				StartDate = source.StartDate,
				DayIntervalIDs = new List<int>(source.DayIntervalIDs),
			};
		}

		protected override void BuildIntervals()
		{
			AvailableTimeIntervals = new ObservableCollection<SKDDayInterval>(SKDManager.TimeIntervalsConfiguration.DayIntervals.OrderBy(item => item.ID));
			AvailableTimeIntervals.Insert(0, new SKDDayInterval()
			{
				ID = 0,
				Name = "<Никогда>",
			});
			OnPropertyChanged(() => AvailableTimeIntervals);
			if (SelectedInterval != null)
				SelectedInterval.Parts.ForEach(item => item.Update());
		}
	}
}