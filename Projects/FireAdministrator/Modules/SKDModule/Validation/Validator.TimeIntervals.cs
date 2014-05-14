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

			foreach (var timeInterval in SKDManager.TimeIntervalsConfiguration.TimeIntervals)
			{
				if (string.IsNullOrEmpty(timeInterval.Name))
				{
					Errors.Add(new TimeIntervalValidationError(timeInterval, "Отсутствует название интервала", ValidationErrorLevel.CannotWrite));
				}
				if (timeInterval.TimeIntervalParts.Count == 0)
				{
					Errors.Add(new TimeIntervalValidationError(timeInterval, "Отсутствуют составляющие части интервала", ValidationErrorLevel.CannotWrite));
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

		void ValidateTimeIntervalEquality()
		{
			var timeIntervals = new HashSet<string>();
			foreach (var timeInterval in SKDManager.TimeIntervalsConfiguration.TimeIntervals)
			{
				if (!timeIntervals.Add(timeInterval.Name))
				{
					Errors.Add(new TimeIntervalValidationError(timeInterval, "Дублируется название интервала", ValidationErrorLevel.CannotWrite));
				}
			}
		}
	}
}