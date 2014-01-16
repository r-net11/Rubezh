using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using System.Collections.ObjectModel;

namespace SkudModule.ViewModels
{
	public class WeeklyIntervalPartViewModel : BaseViewModel
	{
		WeeklyIntervalViewModel WeeklyIntervalViewModel;
		public NamedSKDTimeInterval TimeInterval { get; private set; }

		public WeeklyIntervalPartViewModel(WeeklyIntervalViewModel weeklyIntervalViewModel, NamedSKDTimeInterval timeInterval)
		{
			WeeklyIntervalViewModel = weeklyIntervalViewModel;
			TimeInterval = timeInterval;

			AvailableTimeIntervals = new ObservableCollection<NamedSKDTimeInterval>();
			foreach (var namedTimeInterval in SKDManager.SKDConfiguration.NamedTimeIntervals)
			{
				AvailableTimeIntervals.Add(namedTimeInterval);
			}
			SelectedTimeInterval = TimeInterval;
		}

		public ObservableCollection<NamedSKDTimeInterval> AvailableTimeIntervals { get; private set; }

		NamedSKDTimeInterval _selectedTimeInterval;
		public NamedSKDTimeInterval SelectedTimeInterval
		{
			get { return _selectedTimeInterval; }
			set
			{
				_selectedTimeInterval = value;
				OnPropertyChanged("SelectedTimeInterval");
				WeeklyIntervalViewModel.Update();
			}
		}
	}
}
