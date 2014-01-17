using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using System.Collections.ObjectModel;

namespace SkudModule.ViewModels
{
	public class SlideWeekIntervalPartViewModel : BaseViewModel
	{
		SlideWeekIntervalViewModel SlideWeekIntervalViewModel;
		public NamedSKDTimeInterval TimeInterval { get; private set; }

		public SlideWeekIntervalPartViewModel(SlideWeekIntervalViewModel slideWeekIntervalViewModel, NamedSKDTimeInterval timeInterval)
		{
			SlideWeekIntervalViewModel = slideWeekIntervalViewModel;
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
				SlideWeekIntervalViewModel.Update();
			}
		}
	}
}