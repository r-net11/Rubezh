using System;
using RubezhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DayIntervalPartViewModel : BaseViewModel
	{
		public DayIntervalPart DayIntervalPart { get; private set; }

		public DayIntervalPartViewModel(DayIntervalPart dayIntervalPart)
		{
			DayIntervalPart = dayIntervalPart;
		}
		public TimeSpan BeginTime
		{
			get { return DayIntervalPart.BeginTime; }
		}
		public TimeSpan EndTime
		{
			get { return DayIntervalPart.EndTime; }
		}
		public DayIntervalPartTransitionType IntervalTransitionType
		{
			get { return DayIntervalPart.TransitionType; }
		}

		public void Update()
		{
			OnPropertyChanged(() => DayIntervalPart);
			OnPropertyChanged(() => BeginTime);
			OnPropertyChanged(() => EndTime);
			OnPropertyChanged(() => IntervalTransitionType);
		}
	}
}