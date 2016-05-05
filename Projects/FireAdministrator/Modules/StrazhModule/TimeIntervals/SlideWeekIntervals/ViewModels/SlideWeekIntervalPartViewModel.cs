using System.Collections.ObjectModel;
using System.Linq;
using Common;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using StrazhModule.Intervals.Base.ViewModels;

namespace StrazhModule.ViewModels
{
	public class SlideWeekIntervalPartViewModel : BaseIntervalPartViewModel
	{
		private SlideWeekIntervalsViewModel _slideWeekIntervalsViewModel;
		private SKDSlideWeeklyInterval _slideWeekInterval;
		public int Index { get; private set; }

		public SlideWeekIntervalPartViewModel(SlideWeekIntervalsViewModel slideWeekIntervalsViewModel, SKDSlideWeeklyInterval slideWeekInterval, int index)
		{
			_slideWeekIntervalsViewModel = slideWeekIntervalsViewModel;
			_slideWeekInterval = slideWeekInterval;
			Index = index;
			Update();
		}

		public string Name { get; private set; }

		private SKDWeeklyInterval _selectedWeekInterval;
		public SKDWeeklyInterval SelectedWeekInterval
		{
			get { return _selectedWeekInterval; }
			set
			{
				if (value == null)
					SelectedWeekInterval = _slideWeekIntervalsViewModel.AvailableWeekIntervals.FirstOrDefault();
				else
				{
					_selectedWeekInterval = value;
					OnPropertyChanged(() => SelectedWeekInterval);
					_slideWeekInterval.WeeklyIntervalIDs[Index] = SelectedWeekInterval.ID;
					ServiceFactory.SaveService.SKDChanged = true;
				}
			}
		}

		public override void Update()
		{
			Name = string.Format("{0}", Index + 1);
			_selectedWeekInterval = _slideWeekIntervalsViewModel.AvailableWeekIntervals.FirstOrDefault(x => x.ID == _slideWeekInterval.WeeklyIntervalIDs[Index]);
			if (_selectedWeekInterval == null)
				_selectedWeekInterval = _slideWeekIntervalsViewModel.AvailableWeekIntervals.FirstOrDefault();
			OnPropertyChanged(() => SelectedWeekInterval);
			OnPropertyChanged(() => Name);
		}
	}
}