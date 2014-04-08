using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.EmployeeTimeIntervals;

namespace SKDModule.ViewModels
{
	public class SlideDayIntervalPartViewModel : BaseViewModel
	{
		SlideDayIntervalViewModel SlideDayIntervalViewModel;
		public DayInterval TimeInterval { get; private set; }

		public SlideDayIntervalPartViewModel(SlideDayIntervalViewModel slideDayIntervalViewModel, DayInterval timeInterval)
		{
			SlideDayIntervalViewModel = slideDayIntervalViewModel;
			TimeInterval = timeInterval;

			AvailableTimeIntervals = new ObservableCollection<DayInterval>();
			//foreach (var interval in SKDManager.SKDConfiguration.TimeIntervals)
			//{
			//	AvailableTimeIntervals.Add(interval);
			//}
			_selectedTimeInterval = TimeInterval;
		}

		public ObservableCollection<DayInterval> AvailableTimeIntervals { get; private set; }

		DayInterval _selectedTimeInterval;
		public DayInterval SelectedTimeInterval
		{
			get { return _selectedTimeInterval; }
			set
			{
				_selectedTimeInterval = value;
				OnPropertyChanged("SelectedTimeInterval");
				SlideDayIntervalViewModel.Update();
			}
		}
	}
}
