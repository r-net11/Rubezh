using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class WeeklyIntervalPartViewModel : BaseViewModel
	{
		WeeklyIntervalViewModel WeeklyIntervalViewModel;
		public SKDWeeklyIntervalPart WeeklyIntervalPart { get; private set; }

		public WeeklyIntervalPartViewModel(WeeklyIntervalViewModel weeklyIntervalViewModel, SKDWeeklyIntervalPart weeklyIntervalPart)
		{
			WeeklyIntervalViewModel = weeklyIntervalViewModel;
			WeeklyIntervalPart = weeklyIntervalPart;

			AvailableTimeIntervals = new ObservableCollection<SKDTimeInterval>();
			foreach (var namedTimeInterval in SKDManager.SKDConfiguration.TimeIntervals)
			{
				AvailableTimeIntervals.Add(namedTimeInterval);
			}
			SelectedTimeInterval = AvailableTimeIntervals.FirstOrDefault(x => x.UID == WeeklyIntervalPart.TimeIntervalUID);

			if (weeklyIntervalPart.IsHolliday)
			{
				Name = "Тип праздника " + weeklyIntervalPart.No;
			}
			else
			{
				Name = IntToWeekDay(weeklyIntervalPart.No);
			}
		}

		public string Name { get; private set; }

		public ObservableCollection<SKDTimeInterval> AvailableTimeIntervals { get; private set; }

		SKDTimeInterval _selectedTimeInterval;
		public SKDTimeInterval SelectedTimeInterval
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