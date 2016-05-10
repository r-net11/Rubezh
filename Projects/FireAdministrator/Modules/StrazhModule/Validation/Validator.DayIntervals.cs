using System;
using System.Collections.Generic;
using StrazhAPI.SKD;
using Infrastructure.Common.Validation;

namespace StrazhModule.Validation
{
	public partial class Validator
	{
		void ValidateTimeIntervals()
		{
			ValidateTimeIntervalEquality();

			foreach (var dayInterval in SKDManager.TimeIntervalsConfiguration.DayIntervals)
			{
				// Пропускаем предустановленные дневные графики доступа
				if (dayInterval.Name == TimeIntervalsConfiguration.PredefinedIntervalNameNever || dayInterval.Name == TimeIntervalsConfiguration.PredefinedIntervalNameAlways)
				{
					continue;
				}

				if (string.IsNullOrEmpty(dayInterval.Name))
					Errors.Add(new DayIntervalValidationError(dayInterval, "Отсутствует название дневного графика", ValidationErrorLevel.CannotWrite));
				if (dayInterval.DayIntervalParts.Count == 0)
					Errors.Add(new DayIntervalValidationError(dayInterval, "Отсутствуют составляющие части дневного графика", ValidationErrorLevel.CannotWrite));
				foreach (var dayIntervalPart in dayInterval.DayIntervalParts)
					if (dayIntervalPart.EndMilliseconds < dayIntervalPart.StartMilliseconds)
					{
						Errors.Add(new DayIntervalValidationError(dayInterval, "Начало интервала меньше конца интервала", ValidationErrorLevel.CannotWrite));
						break;
					}

				var currentDateTime = DateTime.MinValue.TimeOfDay.TotalMilliseconds;
				foreach (var dayIntervalPart in dayInterval.DayIntervalParts)
				{
					if (dayIntervalPart.StartMilliseconds < currentDateTime)
					{
						Errors.Add(new DayIntervalValidationError(dayInterval, "Последовательность интервалов не должна быть пересекающейся", ValidationErrorLevel.CannotWrite));
						break;
					}
					currentDateTime = dayIntervalPart.EndMilliseconds;
				}
			}
		}

		void ValidateTimeIntervalEquality()
		{
			var dayIntervalNames = new HashSet<string>();
			foreach (var dayInterval in SKDManager.TimeIntervalsConfiguration.DayIntervals)
				if (!dayIntervalNames.Add(dayInterval.Name))
					Errors.Add(new DayIntervalValidationError(dayInterval, "Дублируется название дневного графика", ValidationErrorLevel.CannotWrite));
		}
	}
}