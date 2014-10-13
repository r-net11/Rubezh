using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using FiresecAPI.GK;

namespace GKModule.ViewModels
{
	public class ScheduleIntervalPartDetailsViewModel : SaveCancelDialogViewModel
	{
		bool IsNew;
		GKSchedule Schedule;
		public GKIntervalPart GKIntervalPart { get; private set; }

		public ScheduleIntervalPartDetailsViewModel(GKSchedule schedule, GKIntervalPart gkIntervalPart = null)
		{
			Schedule = schedule;
			if (gkIntervalPart == null)
			{
				Title = "Новый интервал";
				IsNew = true;
				gkIntervalPart = new GKIntervalPart()
				{
					DayIntervalUID = schedule.UID,
				};
			}
			else
			{
				Title = "Редактирование интервала";
				IsNew = false;
			}
			GKIntervalPart = gkIntervalPart;

			BeginTime = gkIntervalPart.BeginTime;
			EndTime = gkIntervalPart.EndTime;
		}

		TimeSpan _beginTime;
		public TimeSpan BeginTime
		{
			get { return _beginTime; }
			set
			{
				_beginTime = value;
				OnPropertyChanged(() => BeginTime);
			}
		}

		TimeSpan _endTime;
		public TimeSpan EndTime
		{
			get { return _endTime; }
			set
			{
				_endTime = value;
				OnPropertyChanged(() => EndTime);
			}
		}

		protected override bool Save()
		{
			if (!Validate())
				return false;
			GKIntervalPart.BeginTime = BeginTime;
			GKIntervalPart.EndTime = EndTime;
			return true;
		}

		bool Validate()
		{
			var dayIntervalParts = CloneGKIntervalPart();

			var currentDateTime = TimeSpan.Zero;
			foreach (var dayIntervalPart in dayIntervalParts)
			{
				if (dayIntervalPart.BeginTime < currentDateTime)
				{
					MessageBoxService.ShowWarning("Интервалы должны идти последовательно");
					return false;
				}
				currentDateTime = dayIntervalPart.BeginTime;
			}

			currentDateTime = TimeSpan.Zero;
			foreach (var dayIntervalPart in dayIntervalParts)
			{
				var beginTime = dayIntervalPart.BeginTime;
				var endTime = dayIntervalPart.EndTime;
				if (beginTime > endTime)
				{
					MessageBoxService.ShowWarning("Время окончания интервала должно быть позже времени начала");
					return false;
				}
				if (beginTime < currentDateTime)
				{
					MessageBoxService.ShowWarning("Последовательность интервалов не должна быть пересекающейся");
					return false;
				}
				if (beginTime == currentDateTime)
				{
					MessageBoxService.ShowWarning("Пауза между интервалами не должна быть нулевой");
					return false;
				}
				currentDateTime = beginTime;
				if (endTime < currentDateTime)
				{
					MessageBoxService.ShowWarning("Начало интервала не может быть раньше его окончания");
					return false;
				}
				if (endTime == currentDateTime)
				{
					MessageBoxService.ShowWarning("Интервал не может иметь нулевую длительность");
					return false;
				}
				currentDateTime = endTime;
			}
			return true;
		}
		List<GKIntervalPart> CloneGKIntervalPart()
		{
			var gkIntervalParts = new List<GKIntervalPart>();
			foreach (var intervalPart in Schedule.DayIntervalParts)
			{
				var clonedDayIntervalPart = new GKIntervalPart()
				{
					UID = intervalPart.UID,
					BeginTime = intervalPart.BeginTime,
					EndTime = intervalPart.EndTime,
					DayIntervalUID = intervalPart.DayIntervalUID,
				};
				gkIntervalParts.Add(clonedDayIntervalPart);
			}
			if (IsNew)
			{
				var newEmployeeDayIntervalPart = new GKIntervalPart()
				{
					BeginTime = BeginTime,
					EndTime = EndTime,
					DayIntervalUID = GKIntervalPart.UID,
				};
				gkIntervalParts.Add(newEmployeeDayIntervalPart);
			}
			else
			{
				var deitingDayIntervalPart = gkIntervalParts.FirstOrDefault(x => x.UID == GKIntervalPart.UID);
				if (deitingDayIntervalPart != null)
				{
					deitingDayIntervalPart.BeginTime = BeginTime;
					deitingDayIntervalPart.EndTime = EndTime;
				}
			}
			return gkIntervalParts;
		}
	}
}