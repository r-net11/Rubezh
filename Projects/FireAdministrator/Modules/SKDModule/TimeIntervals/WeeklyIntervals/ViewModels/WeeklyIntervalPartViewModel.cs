using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure;
using SKDModule.Intervals.Base.ViewModels;

namespace SKDModule.ViewModels
{
	public class WeeklyIntervalPartViewModel : BaseIntervalPartViewModel
	{
		WeeklyIntervalsViewModel _weeklyIntervalsViewModel;
		public SKDWeeklyIntervalPart WeeklyIntervalPart { get; private set; }

		public WeeklyIntervalPartViewModel(WeeklyIntervalsViewModel weeklyIntervalsViewModel, SKDWeeklyIntervalPart weeklyIntervalPart)
		{
			_weeklyIntervalsViewModel = weeklyIntervalsViewModel;
			_weeklyIntervalsViewModel.PropertyChanged += (s, e) =>
				{
					if (e.PropertyName == "AvailableDayIntervals")
						OnPropertyChanged(() => AvailableDayIntervals);
				};
			WeeklyIntervalPart = weeklyIntervalPart;
			Name = IntToWeekDay(weeklyIntervalPart.No);
			Update();
		}

		public string Name { get; private set; }

		public ObservableCollection<SKDDayInterval> AvailableDayIntervals
		{
			get { return new ObservableCollection<SKDDayInterval>(_weeklyIntervalsViewModel.AvailableDayIntervals); }
		}

		SKDDayInterval _selectedDayInterval;
		public SKDDayInterval SelectedDayInterval
		{
			get { return _selectedDayInterval; }
			set
			{
				if (value == null)
					SelectedDayInterval = AvailableDayIntervals.FirstOrDefault();
				else
				{
					_selectedDayInterval = value;
					OnPropertyChanged(() => SelectedDayInterval);
					WeeklyIntervalPart.DayIntervalID = ((SKDDayInterval)SelectedDayInterval).No;
					ServiceFactory.SaveService.SKDChanged = true;
					ServiceFactory.SaveService.TimeIntervalChanged();
				}
			}
		}

		public override void Update()
		{
			_selectedDayInterval = _weeklyIntervalsViewModel.AvailableDayIntervals.FirstOrDefault(x => x.No == WeeklyIntervalPart.DayIntervalID);
			if (_selectedDayInterval == null)
				_selectedDayInterval = AvailableDayIntervals.FirstOrDefault();
			OnPropertyChanged(() => SelectedDayInterval);
		}

		public static string IntToWeekDay(int dayNo)
		{
			switch (dayNo)
			{
				case 1:
					return "Воскресенье";
				case 2:
					return "Понедельник";
				case 3:
					return "Вторник";
				case 4:
					return "Среда";
				case 5:
					return "Четверг";
				case 6:
					return "Пятница";
				case 7:
					return "Суббота";
			}
			return "Неизвестный день";
		}
	}
}