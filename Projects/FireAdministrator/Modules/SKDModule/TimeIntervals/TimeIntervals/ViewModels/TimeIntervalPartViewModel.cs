using System;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Intervals.Base.ViewModels;

namespace SKDModule.ViewModels
{
	public class TimeIntervalPartViewModel : BaseIntervalPartViewModel
	{
		public SKDTimeIntervalPart TimeIntervalPart { get; set; }

		public TimeIntervalPartViewModel(SKDTimeIntervalPart timeIntervalPart)
		{
			TimeIntervalPart = timeIntervalPart;
		}

		public TimeSpan StartTime
		{
			get { return TimeIntervalPart.StartTime; }
		}

		public TimeSpan EndTime
		{
			get { return TimeIntervalPart.EndTime; }
		}

		public override void Update()
		{
			OnPropertyChanged(() => TimeIntervalPart);
			OnPropertyChanged(() => StartTime);
			OnPropertyChanged(() => EndTime);
		}
	}
}