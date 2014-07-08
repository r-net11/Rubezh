using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
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
			WeeklyIntervalPart = weeklyIntervalPart;
			if (weeklyIntervalPart.IsHolliday)
				Name = "Тип праздника " + weeklyIntervalPart.No;
			else
				Name = IntToWeekDay(weeklyIntervalPart.No);
			Update();
		}

		public string Name { get; private set; }

		private SKDTimeInterval _selectedTimeInterval;
		public SKDTimeInterval SelectedTimeInterval
		{
			get { return _selectedTimeInterval; }
			set
			{
				if (value == null)
					SelectedTimeInterval = _weeklyIntervalsViewModel.AvailableTimeIntervals.First();
				else
				{
					_selectedTimeInterval = value;
					OnPropertyChanged(() => SelectedTimeInterval);
					WeeklyIntervalPart.TimeIntervalID = SelectedTimeInterval.ID;
					ServiceFactory.SaveService.SKDChanged = true;
				}
			}
		}

		public override void Update()
		{
			_selectedTimeInterval = _weeklyIntervalsViewModel.AvailableTimeIntervals.FirstOrDefault(x => x.ID == WeeklyIntervalPart.TimeIntervalID);
			if (_selectedTimeInterval == null)
				_selectedTimeInterval = _weeklyIntervalsViewModel.AvailableTimeIntervals.First();
			OnPropertyChanged(() => SelectedTimeInterval);
		}

		private string IntToWeekDay(int dayNo)
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