using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure;

namespace SKDModule.ViewModels
{
	public class WeeklyIntervalPartViewModel : BaseViewModel
	{
		WeeklyIntervalViewModel WeeklyIntervalViewModel;
		public DayInterval WeeklyIntervalPart { get; private set; }

		public WeeklyIntervalPartViewModel(WeeklyIntervalViewModel weeklyIntervalViewModel, DayInterval weeklyIntervalPart)
		{
			WeeklyIntervalViewModel = weeklyIntervalViewModel;
			WeeklyIntervalPart = weeklyIntervalPart;
			Name = IntToWeekDay(weeklyIntervalPart.Number);
			Update();
		}

		public string Name { get; private set; }

		public void Update()
		{
			AvailableTimeIntervals = new ObservableCollection<NamedInterval>();
			//foreach (var namedTimeInterval in SKDManager.SKDConfiguration.TimeIntervals)
			//{
			//	AvailableTimeIntervals.Add(namedTimeInterval);
			//}
			_selectedTimeInterval = AvailableTimeIntervals.FirstOrDefault(x => x.UID == WeeklyIntervalPart.NamedIntervalUID);
		}

		ObservableCollection<NamedInterval> _availableTimeIntervals;
		public ObservableCollection<NamedInterval> AvailableTimeIntervals
		{
			get { return _availableTimeIntervals; }
			set
			{
				_availableTimeIntervals = value;
				OnPropertyChanged("AvailableTimeIntervals");
			}
		}

		NamedInterval _selectedTimeInterval;
		public NamedInterval SelectedTimeInterval
		{
			get { return _selectedTimeInterval; }
			set
			{
				_selectedTimeInterval = value;
				OnPropertyChanged("SelectedTimeInterval");
				WeeklyIntervalViewModel.Update();
			}
		}

		string IntToWeekDay(int dayNo)
		{
			switch(dayNo)
			{
				case 1:
					return "Понедельник";
				case 2:
					return "Вторник";
				case 3:
					return "Среда";
				case 4:
					return "Четверг";
				case 5:
					return "Пятница";
				case 6:
					return "Суббота";
				case 7:
					return "Воскресенье";
			}
			return "Неизвестный день";
		}
	}
}