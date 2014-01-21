using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace SkudModule.ViewModels
{
	public class SlideWeekIntervalViewModel : BaseViewModel
	{
		public SKDSlideWeekInterval SlideWeekInterval { get; private set; }

		public SlideWeekIntervalViewModel(SKDSlideWeekInterval slideWeekInterval)
		{
			SlideWeekInterval = slideWeekInterval;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			WeeklyIntervals = new ObservableCollection<SlideWeekIntervalPartViewModel>();
			foreach (var weeklyIntervalUID in slideWeekInterval.WeeklyIntervalUIDs)
			{
				var weeklyInterval = SKDManager.SKDConfiguration.WeeklyIntervals.FirstOrDefault(x => x.UID == weeklyIntervalUID);
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
			var weeklyInterval = SKDManager.SKDConfiguration.WeeklyIntervals.FirstOrDefault();
			SlideWeekInterval.WeeklyIntervalUIDs.Add(weeklyInterval.UID);
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
			SlideWeekInterval.WeeklyIntervalUIDs.Add(SelectedWeeklyInterval.WeeklyInterval.UID);
			WeeklyIntervals.Remove(SelectedWeeklyInterval);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanRemove()
		{
			return SelectedWeeklyInterval != null;
		}
	}
}