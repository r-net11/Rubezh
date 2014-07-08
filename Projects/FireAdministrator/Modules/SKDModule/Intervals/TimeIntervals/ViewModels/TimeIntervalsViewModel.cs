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
	public class TimeIntervalsViewModel : BaseIntervalsViewModel<TimeIntervalViewModel, TimeIntervalPartViewModel, SKDTimeInterval>
	{
		public TimeIntervalsViewModel()
		{
		}

		public override void Initialize()
		{
			Intervals = new ObservableCollection<TimeIntervalViewModel>();
			var map = SKDManager.TimeIntervalsConfiguration.TimeIntervals.ToDictionary(item => item.ID);
			//Intervals.Add(new TimeIntervalViewModel(0, null));
			for (int i = 1; i <= 128; i++)
				Intervals.Add(new TimeIntervalViewModel(i, map.ContainsKey(i) ? map[i] : null));
			SelectedInterval = Intervals.First();
		}

		protected override void OnEdit()
		{
			var timeInrervalDetailsViewModel = new TimeIntervalDetailsViewModel(SelectedInterval.Model);
			if (DialogService.ShowModalWindow(timeInrervalDetailsViewModel))
			{
				SelectedInterval.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		protected override SKDTimeInterval CopyInterval(SKDTimeInterval source)
		{
			var copy = new SKDTimeInterval()
			{
				Name = source.Name,
				Description = source.Description,
			};
			foreach (var timeIntervalPart in source.TimeIntervalParts)
			{
				var copyTimeIntervalPart = new SKDTimeIntervalPart()
				{
					StartTime = timeIntervalPart.StartTime,
					EndTime = timeIntervalPart.EndTime
				};
				copy.TimeIntervalParts.Add(copyTimeIntervalPart);
			}
			return copy;
		}

		protected override void BuildIntervals()
		{
			base.BuildIntervals();
		}
	}
}