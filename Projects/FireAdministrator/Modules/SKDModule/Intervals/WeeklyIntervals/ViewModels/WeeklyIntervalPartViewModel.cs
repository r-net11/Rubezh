using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure;
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

			if (weeklyIntervalPart.IsHolliday)
			{
				Name = "Тип праздника " + weeklyIntervalPart.No;
			}
			else
			{
				Name = IntToWeekDay(weeklyIntervalPart.No);
			}

			Update();
		}

		public string Name { get; private set; }

		public void Update()
		{
			AvailableTimeIntervals = new ObservableCollection<SKDTimeInterval>();
			foreach (var namedTimeInterval in SKDManager.TimeIntervalsConfiguration.TimeIntervals)
			{
				AvailableTimeIntervals.Add(namedTimeInterval);
			}
			_selectedTimeInterval = AvailableTimeIntervals.FirstOrDefault(x => x.ID == WeeklyIntervalPart.TimeIntervalID);
			if (_selectedTimeInterval == null)
			{
				_selectedTimeInterval = AvailableTimeIntervals.FirstOrDefault();
			}
		}

		ObservableCollection<SKDTimeInterval> _availableTimeIntervals;
		public ObservableCollection<SKDTimeInterval> AvailableTimeIntervals
		{
			get { return _availableTimeIntervals; }
			set
			{
				_availableTimeIntervals = value;
				OnPropertyChanged("AvailableTimeIntervals");
			}
		}

		SKDTimeInterval _selectedTimeInterval;
		public SKDTimeInterval SelectedTimeInterval
		{
			get { return _selectedTimeInterval; }
			set
			{
				_selectedTimeInterval = value;
				OnPropertyChanged("SelectedTimeInterval");
				WeeklyIntervalViewModel.Update();
				ServiceFactory.SaveService.SKDChanged = true;
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