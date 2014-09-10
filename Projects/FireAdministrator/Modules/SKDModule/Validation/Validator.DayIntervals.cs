using System;
using System.Collections.Generic;
using FiresecAPI.SKD;
using Infrastructure.Common.Validation;

namespace SKDModule.Validation
{
	public partial class Validator
	{
		void ValidateTimeIntervals()
		{
			ValidateTimeIntervalEquality();

			foreach (var dayInterval in SKDManager.TimeIntervalsConfiguration.DayIntervals)
			{
				if (string.IsNullOrEmpty(dayInterval.Name))
					Errors.Add(new DayIntervalValidationError(dayInterval, "Отсутствует название дневного графика", ValidationErrorLevel.CannotWrite));
				if (dayInterval.DayIntervalParts.Count == 0)
					Errors.Add(new DayIntervalValidationError(dayInterval, "Отсутствуют составляющие части дневного графика", ValidationErrorLevel.CannotWrite));
				foreach (var dayIntervalPart in dayInterval.DayIntervalParts)
					if (dayIntervalPart.EndTime < dayIntervalPart.StartTime)
					{
						Errors.Add(new DayIntervalValidationError(dayInterval, "Начало интервала меньше конца интервала", ValidationErrorLevel.CannotWrite));
						break;
					}

				var currentDateTime = DateTime.MinValue.TimeOfDay;
				foreach (var dayIntervalPart in dayInterval.DayIntervalParts)
				{
					if (dayIntervalPart.StartTime < currentDateTime)
					{
						Errors.Add(new DayIntervalValidationError(dayInterval, "Последовательность интервалов не должна быть пересекающейся", ValidationErrorLevel.CannotWrite));
						break;
					}
					currentDateTime = dayIntervalPart.EndTime;
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