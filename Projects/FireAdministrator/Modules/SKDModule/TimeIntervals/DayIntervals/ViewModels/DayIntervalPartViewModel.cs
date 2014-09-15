using System;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Intervals.Base.ViewModels;

namespace SKDModule.ViewModels
{
	public class DayIntervalPartViewModel : BaseIntervalPartViewModel
	{
		public SKDDayIntervalPart DayIntervalPart { get; set; }

		public DayIntervalPartViewModel(SKDDayIntervalPart dayIntervalPart)
		{
			DayIntervalPart = dayIntervalPart;
		}

		public TimeSpan StartTime
		{
			get { return DayIntervalPart.StartTime; }
		}

		public TimeSpan EndTime
		{
			get { return DayIntervalPart.EndTime; }
		}

		public override void Update()
		{
			OnPropertyChanged(() => DayIntervalPart);
			OnPropertyChanged(() => StartTime);
			OnPropertyChanged(() => EndTime);
		}
	}
}