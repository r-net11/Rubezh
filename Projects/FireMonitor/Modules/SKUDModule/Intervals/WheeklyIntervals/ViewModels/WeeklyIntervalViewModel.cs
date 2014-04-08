using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class WeeklyIntervalViewModel : BaseViewModel
	{
		public ScheduleScheme WeeklyInterval { get; private set; }

		public WeeklyIntervalViewModel(ScheduleScheme weeklyInterval)
		{
			WeeklyInterval = weeklyInterval;
			TimeIntervals = new ObservableCollection<WeeklyIntervalPartViewModel>();
			foreach (var weeklyIntervalPart in weeklyInterval.DayIntervals)
			{
				var weeklyIntervalPartViewModel = new WeeklyIntervalPartViewModel(this, weeklyIntervalPart);
				TimeIntervals.Add(weeklyIntervalPartViewModel);
			}
		}

		public ObservableCollection<WeeklyIntervalPartViewModel> TimeIntervals { get; private set; }

		WeeklyIntervalPartViewModel _selectedTimeInterval;
		public WeeklyIntervalPartViewModel SelectedTimeInterval
		{
			get { return _selectedTimeInterval; }
			set
			{
				_selectedTimeInterval = value;
				OnPropertyChanged("SelectedTimeInterval");
			}
		}

		public void Update()
		{
			OnPropertyChanged("WeeklyInterval");

			WeeklyInterval.DayIntervals = new List<DayInterval>();
			foreach (var timeInterval in TimeIntervals)
			{
				if (timeInterval.SelectedTimeInterval != null)
				{
					timeInterval.WeeklyIntervalPart.NamedIntervalUID = timeInterval.SelectedTimeInterval.UID;
					WeeklyInterval.DayIntervals.Add(timeInterval.WeeklyIntervalPart);
				}
			}
		}
	}
}