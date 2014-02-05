using System;
using System.Collections.Generic;
using FiresecAPI;
using Infrastructure.Common.Validation;

namespace SKDModule.Validation
{
	public static partial class Validator
	{
		static void ValidateTimeIntervals()
		{
			ValidateTimeIntervalEquality();

			foreach (var timeInterval in SKDManager.SKDConfiguration.TimeIntervals)
			{
				if (timeInterval.TimeIntervalParts.Count == 0)
				{
					Errors.Add(new TimeIntervalValidationError(timeInterval, "Пустой интервал", ValidationErrorLevel.CannotWrite));
				}

				foreach (var timeIntervalPart in timeInterval.TimeIntervalParts)
				{
					if (timeIntervalPart.EndTime < timeIntervalPart.StartTime)
					{
						Errors.Add(new TimeIntervalValidationError(timeInterval, "Начало интервала меньше конца интервала", ValidationErrorLevel.CannotWrite));
						break;
					}
				}

				var currentDateTime = DateTime.MinValue;
				foreach (var timeIntervalPart in timeInterval.TimeIntervalParts)
				{
					if (timeIntervalPart.StartTime < currentDateTime)
					{
						Errors.Add(new TimeIntervalValidationError(timeInterval, "Последовательность интервалов не должна быть пересекающейся", ValidationErrorLevel.CannotWrite));
						break;
					}
					currentDateTime = timeIntervalPart.EndTime;
				}
			}
		}

		static void ValidateTimeIntervalEquality()
		{
			var timeIntervals = new HashSet<string>();
			foreach (var timeInterval in SKDManager.SKDConfiguration.TimeIntervals)
			{
				if (!timeIntervals.Add(timeInterval.Name))
				{
					Errors.Add(new TimeIntervalValidationError(timeInterval, "Дублируется название интервала", ValidationErrorLevel.CannotWrite));
				}
			}
		}
	}
}