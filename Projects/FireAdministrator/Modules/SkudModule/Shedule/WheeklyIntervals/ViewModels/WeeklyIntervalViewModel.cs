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
	public class WeeklyIntervalViewModel : BaseViewModel
	{
		public SKDWeeklyInterval WeeklyInterval { get; private set; }

		public WeeklyIntervalViewModel(SKDWeeklyInterval weeklyInterval)
		{
			WeeklyInterval = weeklyInterval;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			TimeIntervals = new ObservableCollection<WeeklyIntervalPartViewModel>();
			foreach (var timeIntervalUID in weeklyInterval.TimeIntervalUIDs)
			{
				var timeInterval = SKDManager.SKDConfiguration.NamedTimeIntervals.FirstOrDefault(x => x.UID == timeIntervalUID);
				if (timeInterval != null)
				{
					var weeklyIntervalPartViewModel = new WeeklyIntervalPartViewModel(this, timeInterval);
					TimeIntervals.Add(weeklyIntervalPartViewModel);
				}
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

			WeeklyInterval.TimeIntervalUIDs = new List<Guid>();
			foreach (var timeInterval in TimeIntervals)
			{
				WeeklyInterval.TimeIntervalUIDs.Add(timeInterval.SelectedTimeInterval.UID);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var timeInterval = SKDManager.SKDConfiguration.NamedTimeIntervals.FirstOrDefault();
			WeeklyInterval.TimeIntervalUIDs.Add(timeInterval.UID);
			var slideDayIntervalPartViewModel = new WeeklyIntervalPartViewModel(this, timeInterval);
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
			WeeklyInterval.TimeIntervalUIDs.Add(SelectedTimeInterval.TimeInterval.UID);
			TimeIntervals.Remove(SelectedTimeInterval);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanRemove()
		{
			return SelectedTimeInterval != null;
		}
	}
}