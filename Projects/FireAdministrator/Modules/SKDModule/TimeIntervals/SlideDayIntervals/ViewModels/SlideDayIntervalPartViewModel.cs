using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq;
using SKDModule.Intervals.Base.ViewModels;

namespace SKDModule.ViewModels
{
	public class SlideDayIntervalPartViewModel : BaseIntervalPartViewModel
	{
		private SlideDayIntervalsViewModel _slideDayIntervalsViewModel;
		private SKDSlideDayInterval _slideDayInterval;
		public int Index { get; private set; }

		public SlideDayIntervalPartViewModel(SlideDayIntervalsViewModel slideDayIntervalsViewModel, SKDSlideDayInterval slideDayInterval, int index)
		{
			_slideDayIntervalsViewModel = slideDayIntervalsViewModel;
			_slideDayInterval = slideDayInterval;
			Index = index;
			Update();
		}

		public string Name { get; private set; }

		private SKDDayInterval _selectedTimeInterval;
		public SKDDayInterval SelectedTimeInterval
		{
			get { return _selectedTimeInterval; }
			set
			{
				if (value == null)
					SelectedTimeInterval = _slideDayIntervalsViewModel.AvailableTimeIntervals.FirstOrDefault();
				else
				{
					_selectedTimeInterval = value;
					OnPropertyChanged(() => SelectedTimeInterval);
					_slideDayInterval.DayIntervalIDs[Index] = SelectedTimeInterval.ID;
					ServiceFactory.SaveService.SKDChanged = true;
				}
			}
		}

		public override void Update()
		{
			Name = string.Format("{0}", Index + 1);
			_selectedTimeInterval = _slideDayIntervalsViewModel.AvailableTimeIntervals.FirstOrDefault(x => x.ID == _slideDayInterval.DayIntervalIDs[Index]);
			if (_selectedTimeInterval == null)
				_selectedTimeInterval = _slideDayIntervalsViewModel.AvailableTimeIntervals.FirstOrDefault();
			OnPropertyChanged(() => SelectedTimeInterval);
			OnPropertyChanged(() => Name);
		}
	}
}