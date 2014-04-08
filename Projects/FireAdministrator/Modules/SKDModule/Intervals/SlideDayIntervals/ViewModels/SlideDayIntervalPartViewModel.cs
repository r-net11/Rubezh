using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure;

namespace SKDModule.ViewModels
{
	public class SlideDayIntervalPartViewModel : BaseViewModel
	{
		SlideDayIntervalViewModel SlideDayIntervalViewModel;
		public SKDTimeInterval TimeInterval { get; private set; }

		public SlideDayIntervalPartViewModel(SlideDayIntervalViewModel slideDayIntervalViewModel, SKDTimeInterval timeInterval)
		{
			SlideDayIntervalViewModel = slideDayIntervalViewModel;
			TimeInterval = timeInterval;

			AvailableTimeIntervals = new ObservableCollection<SKDTimeInterval>();
			foreach (var interval in SKDManager.TimeIntervalsConfiguration.TimeIntervals)
			{
				AvailableTimeIntervals.Add(interval);
			}
			_selectedTimeInterval = TimeInterval;
		}

		public ObservableCollection<SKDTimeInterval> AvailableTimeIntervals { get; private set; }

		SKDTimeInterval _selectedTimeInterval;
		public SKDTimeInterval SelectedTimeInterval
		{
			get { return _selectedTimeInterval; }
			set
			{
				_selectedTimeInterval = value;
				OnPropertyChanged("SelectedTimeInterval");
				SlideDayIntervalViewModel.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
	}
}
