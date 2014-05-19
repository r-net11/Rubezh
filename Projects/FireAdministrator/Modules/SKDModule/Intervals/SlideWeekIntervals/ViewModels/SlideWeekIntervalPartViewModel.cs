using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

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
			foreach (var skdWeeklyInterval in SKDManager.TimeIntervalsConfiguration.WeeklyIntervals)
			{
				AvailableWeeklyIntervals.Add(skdWeeklyInterval);
			}
			_selectedWeeklyInterval = WeeklyInterval;
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
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
	}
}