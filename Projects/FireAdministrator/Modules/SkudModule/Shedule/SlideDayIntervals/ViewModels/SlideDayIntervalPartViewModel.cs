using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class SlideDayIntervalPartViewModel : BaseViewModel
	{
		SlideDayIntervalViewModel SlideDayIntervalViewModel;
		public NamedSKDTimeInterval TimeInterval { get; private set; }

		public SlideDayIntervalPartViewModel(SlideDayIntervalViewModel slideDayIntervalViewModel, NamedSKDTimeInterval timeInterval)
		{
			SlideDayIntervalViewModel = slideDayIntervalViewModel;
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
				SlideDayIntervalViewModel.Update();
			}
		}
	}
}
