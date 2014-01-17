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
			TimeIntervals = new ObservableCollection<SlideWeekIntervalPartViewModel>();
			foreach (var timeIntervalUID in slideWeekInterval.TimeIntervalUIDs)
			{
				var timeInterval = SKDManager.SKDConfiguration.NamedTimeIntervals.FirstOrDefault(x => x.UID == timeIntervalUID);
				if (timeInterval != null)
				{
					var slideWeekIntervalPartViewModel = new SlideWeekIntervalPartViewModel(this, timeInterval);
					TimeIntervals.Add(slideWeekIntervalPartViewModel);
				}
			}
		}

		public ObservableCollection<SlideWeekIntervalPartViewModel> TimeIntervals { get; private set; }

		SlideWeekIntervalPartViewModel _selectedTimeInterval;
		public SlideWeekIntervalPartViewModel SelectedTimeInterval
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
			OnPropertyChanged("SlideWeekInterval");

			SlideWeekInterval.TimeIntervalUIDs = new List<Guid>();
			foreach (var timeInterval in TimeIntervals)
			{
				SlideWeekInterval.TimeIntervalUIDs.Add(timeInterval.SelectedTimeInterval.UID);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var timeInterval = SKDManager.SKDConfiguration.NamedTimeIntervals.FirstOrDefault();
			SlideWeekInterval.TimeIntervalUIDs.Add(timeInterval.UID);
			var slideWeekIntervalPartViewModel = new SlideWeekIntervalPartViewModel(this, timeInterval);
			TimeIntervals.Add(slideWeekIntervalPartViewModel);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanAdd()
		{
			return TimeIntervals.Count < 31;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			SlideWeekInterval.TimeIntervalUIDs.Add(SelectedTimeInterval.TimeInterval.UID);
			TimeIntervals.Remove(SelectedTimeInterval);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanRemove()
		{
			return SelectedTimeInterval != null;
		}
	}
}