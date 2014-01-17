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
			//AddCommand = new RelayCommand(OnAdd, CanAdd);
			//RemoveCommand = new RelayCommand(OnRemove, CanRemove);
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

			WeeklyInterval.WeeklyIntervalParts = new List<SKDWeeklyIntervalPart>();
			foreach (var timeInterval in TimeIntervals)
			{
				timeInterval.WeeklyIntervalPart.TimeIntervalUID = timeInterval.SelectedTimeInterval.UID;
				WeeklyInterval.WeeklyIntervalParts.Add(timeInterval.WeeklyIntervalPart);
			}
		}

		//public RelayCommand AddCommand { get; private set; }
		//void OnAdd()
		//{
		//    var weeklyIntervalPart = new SKDWeeklyIntervalPart();
		//    weeklyIntervalPart.TimeIntervalUID = SKDManager.SKDConfiguration.NamedTimeIntervals.FirstOrDefault().UID;
		//    WeeklyInterval.WeeklyIntervalParts.Add(weeklyIntervalPart);
		//    var slideDayIntervalPartViewModel = new WeeklyIntervalPartViewModel(this, weeklyIntervalPart);
		//    TimeIntervals.Add(slideDayIntervalPartViewModel);
		//    ServiceFactory.SaveService.SKDChanged = true;
		//}
		//bool CanAdd()
		//{
		//    return TimeIntervals.Count < 31;
		//}

		//public RelayCommand RemoveCommand { get; private set; }
		//void OnRemove()
		//{
		//    WeeklyInterval.WeeklyIntervalParts.Remove(SelectedTimeInterval.WeeklyIntervalPart);
		//    TimeIntervals.Remove(SelectedTimeInterval);
		//    ServiceFactory.SaveService.SKDChanged = true;
		//}
		//bool CanRemove()
		//{
		//    return SelectedTimeInterval != null;
		//}
	}
}