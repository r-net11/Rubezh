﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure;
using StrazhModule.Intervals.Base.ViewModels;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class WeeklyIntervalPartViewModel : BaseIntervalPartViewModel
	{
		WeeklyIntervalsViewModel _weeklyIntervalsViewModel;
		public SKDWeeklyIntervalPart WeeklyIntervalPart { get; private set; }

		public WeeklyIntervalPartViewModel(WeeklyIntervalsViewModel weeklyIntervalsViewModel, SKDWeeklyIntervalPart weeklyIntervalPart)
		{
			_weeklyIntervalsViewModel = weeklyIntervalsViewModel;
			WeeklyIntervalPart = weeklyIntervalPart;
			Update();
		}

		public ObservableCollection<SelectableDayInterval> AvailableDayIntervals { get; private set; }

		SelectableDayInterval _selectedDayInterval;
		public SelectableDayInterval SelectedDayInterval
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
			AvailableDayIntervals = new ObservableCollection<SelectableDayInterval>();
			foreach (var dayInterval in _weeklyIntervalsViewModel.AvailableDayIntervals)
			{
				var selectableDayInterval = new SelectableDayInterval(dayInterval);
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