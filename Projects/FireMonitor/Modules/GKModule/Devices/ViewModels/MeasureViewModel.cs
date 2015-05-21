using System;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class MeasureViewModel : BaseViewModel
	{
		public DateTime MeasureTime { get; private set; }
		public int MeasureValue { get; private set; }

		public MeasureViewModel(DateTime measureTime, int measureValue)
		{
			MeasureTime = measureTime;
			MeasureValue = measureValue;
			OnPropertyChanged(() => MeasureValue);
			OnPropertyChanged(() => MeasureTime);
		}
	}
}
