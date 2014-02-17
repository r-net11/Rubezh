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
	public class MonthlyIntervalViewModel : BaseViewModel
	{
		public EmployeeMonthlyInterval MonthlyInterval { get; private set; }

		public MonthlyIntervalViewModel(EmployeeMonthlyInterval monthlyInterval)
		{
			MonthlyInterval = monthlyInterval;
			MonthlyIntervals = new ObservableCollection<MonthlyIntervalPartViewModel>();
			foreach (var monthlyIntervalPart in monthlyInterval.MonthlyIntervalParts)
			{
				var timeInterval = new EmployeeTimeInterval();// SKDManager.SKDConfiguration.TimeIntervals.FirstOrDefault(x => x.UID == timeIntervalUID);
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

			MonthlyInterval.MonthlyIntervalParts = new List<EmployeeMonthlyIntervalPart>();
			foreach (var monthlyInterval in MonthlyIntervals)
			{
				if (monthlyInterval.SelectedTimeInterval != null)
				{
					monthlyInterval.MonthlyIntervalPart.TimeIntervalUID = monthlyInterval.SelectedTimeInterval.UID;
					MonthlyInterval.MonthlyIntervalParts.Add(monthlyInterval.MonthlyIntervalPart);
				}
			}
		}

		public bool IsEnabled
		{
			get { return !MonthlyInterval.IsDefault; }
		}
	}
}