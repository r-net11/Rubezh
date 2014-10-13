using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;

namespace GKModule.ViewModels
{
	public class ScheduleIntervalPartViewModel : BaseViewModel
	{
		public GKIntervalPart IntervalPart { get; private set; }

		public ScheduleIntervalPartViewModel(GKIntervalPart intervalPart)
		{
			IntervalPart = intervalPart;
		}

		public TimeSpan BeginTime
		{
			get { return IntervalPart.BeginTime; }
		}
		public TimeSpan EndTime
		{
			get { return IntervalPart.EndTime; }
		}

		public void Update()
		{
			OnPropertyChanged(() => IntervalPart);
			OnPropertyChanged(() => BeginTime);
			OnPropertyChanged(() => EndTime);
		}
	}
}