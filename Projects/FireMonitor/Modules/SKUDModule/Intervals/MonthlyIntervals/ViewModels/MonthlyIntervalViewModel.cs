using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Organisation = FiresecAPI.Organisation;

namespace SKDModule.ViewModels
{
	public class MonthlyIntervalViewModel : BaseViewModel
	{
		public ScheduleScheme MonthlyInterval { get; private set; }

		public MonthlyIntervalViewModel(ScheduleScheme monthlyInterval)
		{
			MonthlyInterval = monthlyInterval;
			MonthlyIntervals = new ObservableCollection<MonthlyIntervalPartViewModel>();
			foreach (var monthlyIntervalPart in monthlyInterval.DayIntervals)
			{
				var timeInterval = new NamedInterval();// SKDManager.SKDConfiguration.TimeIntervals.FirstOrDefault(x => x.UID == timeIntervalUID);
				if (timeInterval != null)
				{
					var monthlyIntervalPartViewModel = new MonthlyIntervalPartViewModel(this, monthlyIntervalPart);
					MonthlyIntervals.Add(monthlyIntervalPartViewModel);
				}
			}
		}

		public ObservableCollection<MonthlyIntervalPartViewModel> MonthlyIntervals { get; private set; }

		MonthlyIntervalPartViewModel _selectedMonthlyInterval;
		public MonthlyIntervalPartViewModel SelectedMonthlyInterval
		{
			get { return _selectedMonthlyInterval; }
			set
			{
				_selectedMonthlyInterval = value;
				OnPropertyChanged("SelectedMonthlyInterval");
			}
		}

		public void Update()
		{
			OnPropertyChanged("MonthlyInterval");

			MonthlyInterval.DayIntervals = new List<DayInterval>();
			foreach (var monthlyInterval in MonthlyIntervals)
			{
				if (monthlyInterval.SelectedTimeInterval != null)
				{
					monthlyInterval.MonthlyIntervalPart.NamedIntervalUID = monthlyInterval.SelectedTimeInterval.UID;
					MonthlyInterval.DayIntervals.Add(monthlyInterval.MonthlyIntervalPart);
				}
			}
		}
	}
}