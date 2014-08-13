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
					if (e.PropertyName == "AvailableHolidays" || e.PropertyName == "AvailableTimeIntervals")
						OnPropertyChanged(() => AvailableTimeIntervals);
				};
			WeeklyIntervalPart = weeklyIntervalPart;
			Name = IntToWeekDay(weeklyIntervalPart.No);
			Update();
		}

		public string Name { get; private set; }

		public ObservableCollection<object> AvailableTimeIntervals
		{
			get { return new ObservableCollection<object>(_weeklyIntervalsViewModel.AvailableTimeIntervals); }
		}
		object _selectedTimeInterval;
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
					WeeklyIntervalPart.TimeIntervalID = ((SKDTimeInterval)SelectedTimeInterval).ID;
					ServiceFactory.SaveService.SKDChanged = true;
					ServiceFactory.SaveService.TimeIntervalChanged();
				}
			}
		}

		public override void Update()
		{
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