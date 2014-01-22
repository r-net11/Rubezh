using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class SlideWeekIntervalPartViewModel : BaseViewModel
	{
		SlideWeekIntervalViewModel SlideWeekIntervalViewModel;
		public SKDWeeklyInterval WeeklyInterval { get; private set; }

		public SlideWeekIntervalPartViewModel(SlideWeekIntervalViewModel slideWeekIntervalViewModel, SKDWeeklyInterval weeklyInterval)
		{
			SlideWeekIntervalViewModel = slideWeekIntervalViewModel;
			WeeklyInterval = weeklyInterval;

			AvailableWeeklyIntervals = new ObservableCollection<SKDWeeklyInterval>();
			foreach (var skdWeeklyInterval in SKDManager.SKDConfiguration.WeeklyIntervals)
			{
				AvailableWeeklyIntervals.Add(skdWeeklyInterval);
			}
			SelectedWeeklyInterval = WeeklyInterval;
		}

		public ObservableCollection<SKDWeeklyInterval> AvailableWeeklyIntervals { get; private set; }

		SKDWeeklyInterval _selectedWeeklyInterval;
		public SKDWeeklyInterval SelectedWeeklyInterval
		{
			get { return _selectedWeeklyInterval; }
			set
			{
				_selectedWeeklyInterval = value;
				OnPropertyChanged("SelectedWeeklyInterval");
				SlideWeekIntervalViewModel.Update();
			}
		}
	}
}