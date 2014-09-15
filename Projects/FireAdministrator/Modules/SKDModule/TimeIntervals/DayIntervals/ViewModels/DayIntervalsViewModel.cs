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
	public class DayIntervalsViewModel : BaseIntervalsViewModel<DayIntervalViewModel, DayIntervalPartViewModel, SKDDayInterval>
	{
		public override void Initialize()
		{
			Intervals = new ObservableCollection<DayIntervalViewModel>();
			var map = SKDManager.TimeIntervalsConfiguration.DayIntervals.ToDictionary(item => item.ID);
			for (int i = 1; i <= 128; i++)
				Intervals.Add(new DayIntervalViewModel(i, map.ContainsKey(i) ? map[i] : null));
			SelectedInterval = Intervals.FirstOrDefault();
		}

		protected override void OnEdit()
		{
			var dayInrervalDetailsViewModel = new DayIntervalDetailsViewModel(SelectedInterval.Model);
			if (DialogService.ShowModalWindow(dayInrervalDetailsViewModel))
			{
				SelectedInterval.Update();
				ServiceFactory.SaveService.SKDChanged = true;
				ServiceFactory.SaveService.TimeIntervalChanged();
			}
		}

		protected override SKDDayInterval CopyInterval(SKDDayInterval source)
		{
			var copy = new SKDDayInterval()
			{
				Name = source.Name,
				Description = source.Description,
			};
			foreach (var dayIntervalPart in source.DayIntervalParts)
			{
				var copyDayIntervalPart = new SKDDayIntervalPart()
				{
					StartTime = dayIntervalPart.StartTime,
					EndTime = dayIntervalPart.EndTime
				};
				copy.DayIntervalParts.Add(copyDayIntervalPart);
			}
			return copy;
		}

		protected override void BuildIntervals()
		{
			base.BuildIntervals();
		}
	}
}