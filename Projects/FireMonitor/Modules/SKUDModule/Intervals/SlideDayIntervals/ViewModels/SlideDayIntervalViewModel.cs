using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SlideDayIntervalViewModel : BaseViewModel
	{
		public ScheduleScheme SlideDayInterval { get; private set; }

		public SlideDayIntervalViewModel(ScheduleScheme slideDayInterval)
		{
			SlideDayInterval = slideDayInterval;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			TimeIntervals = new ObservableCollection<SlideDayIntervalPartViewModel>();
			foreach (var timeIntervalUID in slideDayInterval.DayIntervals)
			{
				var timeInterval = (DayInterval)null; // SKDManager.SKDConfiguration.TimeIntervals.FirstOrDefault(x => x.UID == timeIntervalUID);
				if (timeInterval != null)
				{
					var slideDayIntervalPartViewModel = new SlideDayIntervalPartViewModel(this, timeInterval);
					TimeIntervals.Add(slideDayIntervalPartViewModel);
				}
			}
		}

		public ObservableCollection<SlideDayIntervalPartViewModel> TimeIntervals { get; private set; }

		SlideDayIntervalPartViewModel _selectedTimeInterval;
		public SlideDayIntervalPartViewModel SelectedTimeInterval
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
			OnPropertyChanged("SlideDayInterval");

			SlideDayInterval.DayIntervals = new List<DayInterval>();
			foreach (var timeInterval in TimeIntervals)
			{
				SlideDayInterval.DayIntervals.Add(timeInterval.SelectedTimeInterval);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var timeInterval = new DayInterval(); //SKDManager.SKDConfiguration.TimeIntervals.FirstOrDefault();
			if (timeInterval != null)
			{
				SlideDayInterval.DayIntervals.Add(timeInterval);
				var slideDayIntervalPartViewModel = new SlideDayIntervalPartViewModel(this, timeInterval);
				TimeIntervals.Add(slideDayIntervalPartViewModel);
			}
		}
		bool CanAdd()
		{
			return TimeIntervals.Count < 31;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			SlideDayInterval.DayIntervals.Remove(SelectedTimeInterval.TimeInterval);
			TimeIntervals.Remove(SelectedTimeInterval);
		}
		bool CanRemove()
		{
			return SelectedTimeInterval != null && TimeIntervals.Count > 1;
		}
	}
}