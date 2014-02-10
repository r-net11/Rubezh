using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SlideWeekIntervalPartViewModel : BaseViewModel
	{
		SlideWeekIntervalViewModel SlideWeekIntervalViewModel;
		public EmployeeWeeklyInterval WeeklyInterval { get; private set; }

		public SlideWeekIntervalPartViewModel(SlideWeekIntervalViewModel slideWeekIntervalViewModel, EmployeeWeeklyInterval weeklyInterval)
		{
			SlideWeekIntervalViewModel = slideWeekIntervalViewModel;
			WeeklyInterval = weeklyInterval;

			AvailableWeeklyIntervals = new ObservableCollection<EmployeeWeeklyInterval>();
			//foreach (var skdWeeklyInterval in SKDManager.SKDConfiguration.WeeklyIntervals)
			//{
			//    AvailableWeeklyIntervals.Add(skdWeeklyInterval);
			//}
			_selectedWeeklyInterval = WeeklyInterval;
		}

		public ObservableCollection<EmployeeWeeklyInterval> AvailableWeeklyIntervals { get; private set; }

		EmployeeWeeklyInterval _selectedWeeklyInterval;
		public EmployeeWeeklyInterval SelectedWeeklyInterval
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