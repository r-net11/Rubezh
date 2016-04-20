using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhClient.SKDHelpers;

namespace GKModule.ViewModels
{
	public class SchedulePartViewModel : BaseViewModel
	{
		public int DayNo { get; private set; }
		GKSchedule Schedule;

		public SchedulePartViewModel(GKSchedule schedule, GKSchedulePart schedulePart)
		{
			Schedule = schedule;
			DayNo = schedulePart.DayNo;
			AvailableDaySchedules = new ObservableCollection<GKDaySchedule>();
			foreach (var dayInterval in GKModuleLoader.DaySchedulesViewModel.GetDaySchedules())
			{
				AvailableDaySchedules.Add(dayInterval);
			}
			_selectedDaySchedule = AvailableDaySchedules.FirstOrDefault(x => x.UID == schedulePart.DayScheduleUID);
			if (_selectedDaySchedule == null)
				_selectedDaySchedule = AvailableDaySchedules.FirstOrDefault();
			Update();
		}

		public string Name { get; private set; }
		public ObservableCollection<GKDaySchedule> AvailableDaySchedules { get; private set; }

		GKDaySchedule _selectedDaySchedule;
		public GKDaySchedule SelectedDaySchedule
		{
			get { return _selectedDaySchedule; }
			set
			{
				if (value == null)
					SelectedDaySchedule = AvailableDaySchedules.FirstOrDefault();
				else
				{
					if (Schedule.ScheduleParts.Count > DayNo)
					{
						Schedule.ScheduleParts[DayNo].DayScheduleUID = value.UID;
					}
				}
				if (GKScheduleHelper.SaveSchedule(Schedule, false))
				{
					_selectedDaySchedule = value;
				}
				else
				{
					Schedule.ScheduleParts[DayNo].DayScheduleUID = _selectedDaySchedule.UID;
				}
				OnPropertyChanged(() => SelectedDaySchedule);
			}
		}

		public void Update()
		{
			if (Schedule.SchedulePeriodType == GKSchedulePeriodType.Weekly)
			{
				var dayOfWeekNo = DayNo % 7;
				Name = IntToWeekDay(dayOfWeekNo);
			}
			else
			{
				Name = string.Format("{0}", DayNo + 1);
			}
			_selectedDaySchedule = AvailableDaySchedules.FirstOrDefault(x => x.UID == Schedule.ScheduleParts[DayNo].DayScheduleUID);
			if (_selectedDaySchedule == null)
				_selectedDaySchedule = AvailableDaySchedules.FirstOrDefault();
			OnPropertyChanged(() => SelectedDaySchedule);
			OnPropertyChanged(() => Name);
		}

		public static string IntToWeekDay(int dayNo)
		{
			switch (dayNo)
			{
				case 0:
					return "Понедельник";
				case 1:
					return "Вторник";
				case 2:
					return "Среда";
				case 3:
					return "Четверг";
				case 4:
					return "Пятница";
				case 5:
					return "Суббота";
				case 6:
					return "Воскресенье";
			}
			return "Неизвестный день";
		}
	}
}