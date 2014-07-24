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
		private WeeklyIntervalsViewModel _weeklyIntervalsViewModel;
		public SKDWeeklyIntervalPart WeeklyIntervalPart { get; private set; }

		public WeeklyIntervalPartViewModel(WeeklyIntervalsViewModel weeklyIntervalsViewModel, SKDWeeklyIntervalPart weeklyIntervalPart)
		{
			_weeklyIntervalsViewModel = weeklyIntervalsViewModel;
			_weeklyIntervalsViewModel.PropertyChanged += (s, e) =>
				{
					if (e.PropertyName == "AvailableHolidays" || e.PropertyName == "AvailableTimeIntervals")
						OnPropertyChanged(() => AvailableTimeIntervals);
				};
			WeeklyIntervalPart = weeklyIntervalPart;
			if (weeklyIntervalPart.IsHolliday)
				Name = "Тип праздника " + weeklyIntervalPart.No;
			else
				Name = IntToWeekDay(weeklyIntervalPart.No);
			Update();
		}

		public string Name { get; private set; }

		public ObservableCollection<object> AvailableTimeIntervals
		{
			get { return WeeklyIntervalPart.IsHolliday ? new ObservableCollection<object>(_weeklyIntervalsViewModel.AvailableHolidays) : new ObservableCollection<object>(_weeklyIntervalsViewModel.AvailableTimeIntervals); }
		}
		private object _selectedTimeInterval;
		public object SelectedTimeInterval
		{
			get { return _selectedTimeInterval; }
			set
			{
				if (value == null)
					SelectedTimeInterval = AvailableTimeIntervals.FirstOrDefault();
				else
				{
					_selectedTimeInterval = value;
					OnPropertyChanged(() => SelectedTimeInterval);
					if (WeeklyIntervalPart.IsHolliday)
					{
						WeeklyIntervalPart.TimeIntervalID = 0;
						WeeklyIntervalPart.HolidayUID = ((SKDHoliday)SelectedTimeInterval).UID;
					}
					else
					{
						WeeklyIntervalPart.TimeIntervalID = ((SKDTimeInterval)SelectedTimeInterval).ID;
						WeeklyIntervalPart.HolidayUID = Guid.Empty;
					}
					ServiceFactory.SaveService.SKDChanged = true;
				}
			}
		}

		public override void Update()
		{
			if (WeeklyIntervalPart.IsHolliday)
				_selectedTimeInterval = _weeklyIntervalsViewModel.AvailableHolidays.FirstOrDefault(x => x.UID == WeeklyIntervalPart.HolidayUID);
			else
				_selectedTimeInterval = _weeklyIntervalsViewModel.AvailableTimeIntervals.FirstOrDefault(x => x.ID == WeeklyIntervalPart.TimeIntervalID);
			if (_selectedTimeInterval == null)
				_selectedTimeInterval = AvailableTimeIntervals.FirstOrDefault();
			OnPropertyChanged(() => SelectedTimeInterval);
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