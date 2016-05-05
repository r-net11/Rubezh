using System;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using StrazhModule.Intervals.Base.ViewModels;

namespace StrazhModule.ViewModels
{
	public class DoorDayIntervalPartViewModel : BaseIntervalPartViewModel
	{
		public SKDDoorDayIntervalPart DayIntervalPart { get; set; }

		public DoorDayIntervalPartViewModel(SKDDoorDayIntervalPart dayIntervalPart)
		{
			DayIntervalPart = dayIntervalPart;
		}

		public TimeSpan StartTime
		{
			get { return TimeSpan.FromMilliseconds(DayIntervalPart.StartMilliseconds); }
		}

		public TimeSpan EndTime
		{
			get { return TimeSpan.FromMilliseconds(DayIntervalPart.EndMilliseconds); }
		}

		public SKDDoorConfiguration_DoorOpenMethod DoorOpenMethod
		{
			get { return DayIntervalPart.DoorOpenMethod; }
		}

		public override void Update()
		{
			OnPropertyChanged(() => DayIntervalPart);
			OnPropertyChanged(() => StartTime);
			OnPropertyChanged(() => EndTime);
			OnPropertyChanged(() => DoorOpenMethod);
		}
	}
}