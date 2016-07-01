using System;
using System.Collections.Generic;
using Localization.Strazh.Errors;
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
                    Errors.Add(new DayIntervalValidationError(dayInterval, CommonErrors.ValidateIntervals_EmptyNameError, ValidationErrorLevel.CannotWrite));
				if (dayInterval.DayIntervalParts.Count == 0)
                    Errors.Add(new DayIntervalValidationError(dayInterval, CommonErrors.ValidateTimeIntervals_EmptyPartError, ValidationErrorLevel.CannotWrite));
				foreach (var dayIntervalPart in dayInterval.DayIntervalParts)
					if (dayIntervalPart.EndMilliseconds < dayIntervalPart.StartMilliseconds)
					{
                        Errors.Add(new DayIntervalValidationError(dayInterval, CommonErrors.ValidateIntervals_StartLessEndError, ValidationErrorLevel.CannotWrite));
						break;
					}

				var currentDateTime = DateTime.MinValue.TimeOfDay.TotalMilliseconds;
				foreach (var dayIntervalPart in dayInterval.DayIntervalParts)
				{
					if (dayIntervalPart.StartMilliseconds < currentDateTime)
					{
                        Errors.Add(new DayIntervalValidationError(dayInterval, CommonErrors.ValidateTimeIntervals_IntersectError, ValidationErrorLevel.CannotWrite));
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
                    Errors.Add(new DayIntervalValidationError(dayInterval, CommonErrors.ValidateDoorTimeIntervals_DublicateError, ValidationErrorLevel.CannotWrite));
		}
	}
}