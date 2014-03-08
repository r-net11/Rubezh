using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class MonthlyIntervalPartViewModel : BaseViewModel
	{
		MonthlyIntervalViewModel MonthlyIntervalViewModel;
		public EmployeeMonthlyIntervalPart MonthlyIntervalPart { get; private set; }

		public MonthlyIntervalPartViewModel(MonthlyIntervalViewModel monthlyIntervalViewModel, EmployeeMonthlyIntervalPart timeInterval)
		{
			MonthlyIntervalViewModel = monthlyIntervalViewModel;
			MonthlyIntervalPart = timeInterval;

			AvailableTimeIntervals = new ObservableCollection<EmployeeTimeInterval>();
			//foreach (var skdWeeklyInterval in SKDManager.SKDConfiguration.WeeklyIntervals)
			//{
			//	AvailableWeeklyIntervals.Add(skdWeeklyInterval);
			//}
			_selectedTimeInterval = AvailableTimeIntervals.FirstOrDefault(x => x.UID == MonthlyIntervalPart.TimeIntervalUID);
		}

		public ObservableCollection<EmployeeTimeInterval> AvailableTimeIntervals { get; private set; }

		EmployeeTimeInterval _selectedTimeInterval;
		public EmployeeTimeInterval SelectedTimeInterval
		{
			get { return _selectedTimeInterval; }
			set
			{
				_selectedTimeInterval = value;
				OnPropertyChanged("SelectedTimeInterval");
				MonthlyIntervalViewModel.Update();
			}
		}
	}
}