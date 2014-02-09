using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SlideWeekIntervalViewModel : BaseViewModel
	{
		public EmployeeSlideWeeklyInterval SlideWeekInterval { get; private set; }

		public SlideWeekIntervalViewModel(EmployeeSlideWeeklyInterval slideWeekInterval)
		{
			SlideWeekInterval = slideWeekInterval;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			WeeklyIntervals = new ObservableCollection<SlideWeekIntervalPartViewModel>();
			foreach (var weeklyIntervalUID in slideWeekInterval.WeeklyIntervalUIDs)
			{
				var weeklyInterval = (EmployeeWeeklyInterval)null;// SKDManager.SKDConfiguration.WeeklyIntervals.FirstOrDefault(x => x.UID == weeklyIntervalUID);
				if (weeklyInterval != null)
				{
					var slideWeekIntervalPartViewModel = new SlideWeekIntervalPartViewModel(this, weeklyInterval);
					WeeklyIntervals.Add(slideWeekIntervalPartViewModel);
				}
			}
		}

		public ObservableCollection<SlideWeekIntervalPartViewModel> WeeklyIntervals { get; private set; }

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

			SlideWeekInterval.WeeklyIntervalUIDs = new List<Guid>();
			foreach (var timeInterval in WeeklyIntervals)
			{
				SlideWeekInterval.WeeklyIntervalUIDs.Add(timeInterval.SelectedWeeklyInterval.UID);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var weeklyInterval = (EmployeeWeeklyInterval)null;
			if (weeklyInterval != null)
			{
				SlideWeekInterval.WeeklyIntervalUIDs.Add(weeklyInterval.UID);
				var slideWeekIntervalPartViewModel = new SlideWeekIntervalPartViewModel(this, weeklyInterval);
				WeeklyIntervals.Add(slideWeekIntervalPartViewModel);
			}
		}
		bool CanAdd()
		{
			return WeeklyIntervals.Count < 31;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			SlideWeekInterval.WeeklyIntervalUIDs.Add(SelectedWeeklyInterval.WeeklyInterval.UID);
			WeeklyIntervals.Remove(SelectedWeeklyInterval);
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