using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class WeeklyIntervalViewModel : BaseViewModel
	{
		public EmployeeWeeklyInterval WeeklyInterval { get; private set; }

		public WeeklyIntervalViewModel(EmployeeWeeklyInterval weeklyInterval)
		{
			WeeklyInterval = weeklyInterval;
			TimeIntervals = new ObservableCollection<WeeklyIntervalPartViewModel>();
			foreach (var weeklyIntervalPart in weeklyInterval.WeeklyIntervalParts)
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

			WeeklyInterval.WeeklyIntervalParts = new List<EmployeeWeeklyIntervalPart>();
			foreach (var timeInterval in TimeIntervals)
			{
				if (timeInterval.SelectedTimeInterval != null)
				{
					timeInterval.WeeklyIntervalPart.TimeIntervalUID = timeInterval.SelectedTimeInterval.UID;
					WeeklyInterval.WeeklyIntervalParts.Add(timeInterval.WeeklyIntervalPart);
				}
			}
		}

		public bool IsEnabled
		{
			get { return !WeeklyInterval.IsDefault; }
		}
	}
}