using System;
using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.SKD;
using Infrastructure;
using StrazhModule.Intervals.Base.ViewModels;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class DoorWeeklyIntervalPartViewModel : BaseIntervalPartViewModel
	{
		DoorWeeklyIntervalsViewModel _weeklyIntervalsViewModel;
		public SKDDoorWeeklyIntervalPart WeeklyIntervalPart { get; private set; }

		public DoorWeeklyIntervalPartViewModel(DoorWeeklyIntervalsViewModel weeklyIntervalsViewModel, SKDDoorWeeklyIntervalPart weeklyIntervalPart)
		{
			_weeklyIntervalsViewModel = weeklyIntervalsViewModel;
			WeeklyIntervalPart = weeklyIntervalPart;
			Update();
		}

		public ObservableCollection<SelectableDoorDayInterval> AvailableDayIntervals { get; private set; }

		SelectableDoorDayInterval _selectedDayInterval;
		public SelectableDoorDayInterval SelectedDayInterval
		{
			get { return _selectedDayInterval; }
			set
			{
				if (value != null)
				{
					_selectedDayInterval = value;
					OnPropertyChanged(() => SelectedDayInterval);
					WeeklyIntervalPart.DayIntervalUID = value.DayInterval.UID;
					ServiceFactory.SaveService.SKDChanged = true;
					ServiceFactory.SaveService.TimeIntervalChanged();
				}
			}
		}

		public override void Update()
		{
			AvailableDayIntervals = new ObservableCollection<SelectableDoorDayInterval>();
			foreach (var dayInterval in _weeklyIntervalsViewModel.AvailableDayIntervals)
			{
				var selectableDayInterval = new SelectableDoorDayInterval(dayInterval);
				AvailableDayIntervals.Add(selectableDayInterval);
			}
			OnPropertyChanged(() => AvailableDayIntervals);

			_selectedDayInterval = AvailableDayIntervals.FirstOrDefault(x => x.DayInterval.UID == WeeklyIntervalPart.DayIntervalUID);
			if (_selectedDayInterval == null)
				_selectedDayInterval = AvailableDayIntervals.FirstOrDefault();
			OnPropertyChanged(() => SelectedDayInterval);
		}

		public static string IntToWeekDay(int dayNo)
		{
			switch (dayNo)
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