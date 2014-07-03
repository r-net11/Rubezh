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
	public class SlideWeekIntervalViewModel : BaseViewModel
	{
		public SKDSlideWeeklyInterval SlideWeekInterval { get; private set; }

		public SlideWeekIntervalViewModel(SKDSlideWeeklyInterval slideWeekInterval)
		{
			SlideWeekInterval = slideWeekInterval;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			Initialize();
		}

		public void Initialize()
		{
			WeeklyIntervals = new ObservableCollection<SlideWeekIntervalPartViewModel>();
			foreach (var weeklyIntervalID in SlideWeekInterval.WeeklyIntervalIDs)
			{
				var weeklyInterval = SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.FirstOrDefault(x => x.ID == weeklyIntervalID);
				if (weeklyInterval != null)
				{
					var slideWeekIntervalPartViewModel = new SlideWeekIntervalPartViewModel(this, weeklyInterval);
					WeeklyIntervals.Add(slideWeekIntervalPartViewModel);
				}
			}
		}

		ObservableCollection<SlideWeekIntervalPartViewModel> _weeklyIntervals;
		public ObservableCollection<SlideWeekIntervalPartViewModel> WeeklyIntervals
		{
			get { return _weeklyIntervals; }
			set
			{
				_weeklyIntervals = value;
				OnPropertyChanged("WeeklyIntervals");
			}
		}

		SlideWeekIntervalPartViewModel _selectedWeeklyInterval;
		public SlideWeekIntervalPartViewModel SelectedWeeklyInterval
		{
			get { return _selectedWeeklyInterval; }
			set
			{
				_selectedWeeklyInterval = value;
				OnPropertyChanged("SelectedWeeklyInterval");
			}
		}

		public void Update()
		{
			OnPropertyChanged("SlideWeekInterval");

			SlideWeekInterval.WeeklyIntervalIDs = new List<int>();
			foreach (var timeInterval in WeeklyIntervals)
			{
				SlideWeekInterval.WeeklyIntervalIDs.Add(timeInterval.SelectedWeeklyInterval.ID);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var weeklyInterval = SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.FirstOrDefault();
			SlideWeekInterval.WeeklyIntervalIDs.Add(weeklyInterval.ID);
			var slideWeekIntervalPartViewModel = new SlideWeekIntervalPartViewModel(this, weeklyInterval);
			WeeklyIntervals.Add(slideWeekIntervalPartViewModel);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanAdd()
		{
			return WeeklyIntervals.Count < 31;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			SlideWeekInterval.WeeklyIntervalIDs.Add(SelectedWeeklyInterval.WeeklyInterval.ID);
			WeeklyIntervals.Remove(SelectedWeeklyInterval);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanRemove()
		{
			return SelectedWeeklyInterval != null;
		}

		public bool IsEnabled
		{
			get { return !SlideWeekInterval.IsDefault; }
		}
	}
}