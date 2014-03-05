using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SlideDayIntervalPartViewModel : BaseViewModel
	{
		SlideDayIntervalViewModel SlideDayIntervalViewModel;
		public EmployeeTimeInterval TimeInterval { get; private set; }

		public SlideDayIntervalPartViewModel(SlideDayIntervalViewModel slideDayIntervalViewModel, EmployeeTimeInterval timeInterval)
		{
			SlideDayIntervalViewModel = slideDayIntervalViewModel;
			TimeInterval = timeInterval;

			AvailableTimeIntervals = new ObservableCollection<EmployeeTimeInterval>();
			//foreach (var interval in SKDManager.SKDConfiguration.TimeIntervals)
			//{
			//	AvailableTimeIntervals.Add(interval);
			//}
			_selectedTimeInterval = TimeInterval;
		}

		public ObservableCollection<EmployeeTimeInterval> AvailableTimeIntervals { get; private set; }

		EmployeeTimeInterval _selectedTimeInterval;
		public EmployeeTimeInterval SelectedTimeInterval
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
