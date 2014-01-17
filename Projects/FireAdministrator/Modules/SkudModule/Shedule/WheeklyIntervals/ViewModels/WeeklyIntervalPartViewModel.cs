using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using System.Collections.ObjectModel;

namespace SkudModule.ViewModels
{
	public class WeeklyIntervalPartViewModel : BaseViewModel
	{
		WeeklyIntervalViewModel WeeklyIntervalViewModel;
		public SKDWeeklyIntervalPart WeeklyIntervalPart { get; private set; }

		public WeeklyIntervalPartViewModel(WeeklyIntervalViewModel weeklyIntervalViewModel, SKDWeeklyIntervalPart weeklyIntervalPart)
		{
			WeeklyIntervalViewModel = weeklyIntervalViewModel;
			WeeklyIntervalPart = weeklyIntervalPart;

			AvailableTimeIntervals = new ObservableCollection<NamedSKDTimeInterval>();
			foreach (var namedTimeInterval in SKDManager.SKDConfiguration.NamedTimeIntervals)
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

		public ObservableCollection<NamedSKDTimeInterval> AvailableTimeIntervals { get; private set; }

		NamedSKDTimeInterval _selectedTimeInterval;
		public NamedSKDTimeInterval SelectedTimeInterval
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