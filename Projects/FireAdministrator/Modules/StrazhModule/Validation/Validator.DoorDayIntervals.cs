using System;
using System.Collections.Generic;
using StrazhAPI.SKD;
using Infrastructure.Common.Validation;

namespace StrazhModule.Validation
{
	public partial class Validator
	{
		void ValidateDoorTimeIntervals()
		{
			ValidateDoorTimeIntervalEquality();

			foreach (var dayInterval in SKDManager.TimeIntervalsConfiguration.DoorDayIntervals)
			{
				// Пропускаем предустановленные дневные графики замка
				if (dayInterval.Name == TimeIntervalsConfiguration.PredefinedIntervalNameCard
					|| dayInterval.Name == TimeIntervalsConfiguration.PredefinedIntervalNamePassword
					|| dayInterval.Name == TimeIntervalsConfiguration.PredefinedIntervalNameCardAndPassword)
				{
					continue;
				}

				if (string.IsNullOrEmpty(dayInterval.Name))
					Errors.Add(new DoorDayIntervalValidationError(dayInterval, "Отсутствует название дневного графика", ValidationErrorLevel.CannotWrite));
				if (dayInterval.DayIntervalParts.Count == 0)
					Errors.Add(new DoorDayIntervalValidationError(dayInterval, "Отсутствуют составляющие части дневного графика", ValidationErrorLevel.CannotWrite));
				foreach (var dayIntervalPart in dayInterval.DayIntervalParts)
					if (dayIntervalPart.EndMilliseconds < dayIntervalPart.StartMilliseconds)
					{
						Errors.Add(new DoorDayIntervalValidationError(dayInterval, "Начало интервала меньше конца интервала", ValidationErrorLevel.CannotWrite));
						break;
					}

				var currentDateTime = DateTime.MinValue.TimeOfDay.TotalMilliseconds;
				foreach (var dayIntervalPart in dayInterval.DayIntervalParts)
				{
					if (dayIntervalPart.StartMilliseconds < currentDateTime)
					{
						Errors.Add(new DoorDayIntervalValidationError(dayInterval, "Последовательность интервалов не должна быть пересекающейся", ValidationErrorLevel.CannotWrite));
						break;
					}
					currentDateTime = dayIntervalPart.EndMilliseconds;
				}
			}
		}

		void ValidateDoorTimeIntervalEquality()
		{
			var dayIntervalNames = new HashSet<string>();
			foreach (var dayInterval in SKDManager.TimeIntervalsConfiguration.DoorDayIntervals)
				if (!dayIntervalNames.Add(dayInterval.Name))
					Errors.Add(new DoorDayIntervalValidationError(dayInterval, "Дублируется название дневного графика", ValidationErrorLevel.CannotWrite));
		}
	}
}