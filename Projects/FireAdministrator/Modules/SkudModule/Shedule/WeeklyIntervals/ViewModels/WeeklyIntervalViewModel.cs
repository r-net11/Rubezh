using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class WeeklyIntervalViewModel : BaseViewModel
	{
		public SKDWeeklyInterval WeeklyInterval { get; private set; }

		public WeeklyIntervalViewModel(SKDWeeklyInterval weeklyInterval)
		{
			WeeklyInterval = weeklyInterval;
			Initialize();
		}

		public void Initialize()
		{
			TimeIntervals = new ObservableCollection<WeeklyIntervalPartViewModel>();
			foreach (var weeklyIntervalPart in WeeklyInterval.WeeklyIntervalParts)
			{
				var weeklyIntervalPartViewModel = new WeeklyIntervalPartViewModel(this, weeklyIntervalPart);
				TimeIntervals.Add(weeklyIntervalPartViewModel);
			}
		}

		ObservableCollection<WeeklyIntervalPartViewModel> _timeIntervals;
		public ObservableCollection<WeeklyIntervalPartViewModel> TimeIntervals
		{
			get { return _timeIntervals; }
			set
			{
				_timeIntervals = value;
				OnPropertyChanged("TimeIntervals");
			}
		}

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

			WeeklyInterval.WeeklyIntervalParts = new List<SKDWeeklyIntervalPart>();
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