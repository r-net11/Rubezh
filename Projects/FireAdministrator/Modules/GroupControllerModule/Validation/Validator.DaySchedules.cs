using System;
using System.Collections.Generic;
using FiresecAPI.GK;
using Infrastructure.Common.Validation;
using FiresecClient;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateDaySchedules()
		{
			ValidateDaySchedulesEquality();

			foreach (var daySchedule in GKManager.DeviceConfiguration.DaySchedules)
			{
				if (daySchedule.Name == "<Никогда>" || daySchedule.Name == "<Всегда>")
				{
					continue;
				}

				if (string.IsNullOrEmpty(daySchedule.Name))
					Errors.Add(new DayScheduleValidationError(daySchedule, "Отсутствует название дневного графика", ValidationErrorLevel.CannotWrite));
				if (daySchedule.DayScheduleParts.Count == 0)
					Errors.Add(new DayScheduleValidationError(daySchedule, "Отсутствуют составляющие части дневного графика", ValidationErrorLevel.CannotWrite));
				foreach (var daySchedulePart in daySchedule.DayScheduleParts)
					if (daySchedulePart.EndMilliseconds < daySchedulePart.StartMilliseconds)
					{
						Errors.Add(new DayScheduleValidationError(daySchedule, "Начало интервала меньше конца интервала", ValidationErrorLevel.CannotWrite));
						break;
					}

				var currentDateTime = DateTime.MinValue.TimeOfDay.TotalMilliseconds;
				foreach (var daySchedulePart in daySchedule.DayScheduleParts)
				{
					if (daySchedulePart.StartMilliseconds < currentDateTime)
					{
						Errors.Add(new DayScheduleValidationError(daySchedule, "Последовательность интервалов не должна быть пересекающейся", ValidationErrorLevel.CannotWrite));
						break;
					}
					currentDateTime = daySchedulePart.EndMilliseconds;
				}
			}
		}

		void ValidateDaySchedulesEquality()
		{
			var daySchedulelNames = new HashSet<string>();
			foreach (var daySchedule in GKManager.DeviceConfiguration.DaySchedules)
				if (!daySchedulelNames.Add(daySchedule.Name))
					Errors.Add(new DayScheduleValidationError(daySchedule, "Дублируется название дневного графика", ValidationErrorLevel.CannotWrite));
		}
	}
}