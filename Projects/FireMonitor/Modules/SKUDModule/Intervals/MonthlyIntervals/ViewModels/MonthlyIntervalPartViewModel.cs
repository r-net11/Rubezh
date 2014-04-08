using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class MonthlyIntervalPartViewModel : BaseViewModel
	{
		MonthlyIntervalViewModel MonthlyIntervalViewModel;
		public DayInterval MonthlyIntervalPart { get; private set; }

		public MonthlyIntervalPartViewModel(MonthlyIntervalViewModel monthlyIntervalViewModel, DayInterval timeInterval)
		{
			MonthlyIntervalViewModel = monthlyIntervalViewModel;
			MonthlyIntervalPart = timeInterval;

			AvailableTimeIntervals = new ObservableCollection<NamedInterval>();
			//foreach (var skdWeeklyInterval in SKDManager.SKDConfiguration.WeeklyIntervals)
			//{
			//	AvailableWeeklyIntervals.Add(skdWeeklyInterval);
			//}
			_selectedTimeInterval = AvailableTimeIntervals.FirstOrDefault(x => x.UID == MonthlyIntervalPart.NamedIntervalUID);
		}

		public ObservableCollection<NamedInterval> AvailableTimeIntervals { get; private set; }

		NamedInterval _selectedTimeInterval;
		public NamedInterval SelectedTimeInterval
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