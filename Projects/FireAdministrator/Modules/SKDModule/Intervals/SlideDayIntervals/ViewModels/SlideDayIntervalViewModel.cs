using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SlideDayIntervalViewModel : BaseViewModel
	{
		public SKDSlideDayInterval SlideDayInterval { get; private set; }

		public SlideDayIntervalViewModel(SKDSlideDayInterval slideDayInterval)
		{
			SlideDayInterval = slideDayInterval;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			Initialize();
		}

		public void Initialize()
		{
			TimeIntervals = new ObservableCollection<SlideDayIntervalPartViewModel>();
			foreach (var timeIntervalID in SlideDayInterval.TimeIntervalIDs)
			{
				var timeInterval = SKDManager.TimeIntervalsConfiguration.TimeIntervals.FirstOrDefault(x => x.ID == timeIntervalID);
				if (timeInterval != null)
				{
					var slideDayIntervalPartViewModel = new SlideDayIntervalPartViewModel(this, timeInterval);
					TimeIntervals.Add(slideDayIntervalPartViewModel);
				}
			}
		}

		ObservableCollection<SlideDayIntervalPartViewModel> _timeIntervals;
		public ObservableCollection<SlideDayIntervalPartViewModel> TimeIntervals
		{
			get { return _timeIntervals; }
			set
			{
				_timeIntervals = value;
				OnPropertyChanged("TimeIntervals");
			}
		}

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

			SlideDayInterval.TimeIntervalIDs = new List<int>();
			foreach (var timeInterval in TimeIntervals)
			{
				SlideDayInterval.TimeIntervalIDs.Add(timeInterval.SelectedTimeInterval.ID);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var timeInterval = SKDManager.TimeIntervalsConfiguration.TimeIntervals.FirstOrDefault();
			SlideDayInterval.TimeIntervalIDs.Add(timeInterval.ID);
			var slideDayIntervalPartViewModel = new SlideDayIntervalPartViewModel(this, timeInterval);
			TimeIntervals.Add(slideDayIntervalPartViewModel);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanAdd()
		{
			return TimeIntervals.Count < 31;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			SlideDayInterval.TimeIntervalIDs.Add(SelectedTimeInterval.TimeInterval.ID);
			TimeIntervals.Remove(SelectedTimeInterval);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanRemove()
		{
			return SelectedTimeInterval != null && TimeIntervals.Count > 1;
		}

		public bool IsEnabled
		{
			get { return !SlideDayInterval.IsDefault; }
		}
	}
}