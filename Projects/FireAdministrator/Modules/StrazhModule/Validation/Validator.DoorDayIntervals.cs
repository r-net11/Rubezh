using System;
using System.Collections.Generic;
using Localization.Strazh.Errors;
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
                    Errors.Add(new DoorDayIntervalValidationError(dayInterval, CommonErrors.ValidateIntervals_EmptyNameError, ValidationErrorLevel.CannotWrite));
				if (dayInterval.DayIntervalParts.Count == 0)
                    Errors.Add(new DoorDayIntervalValidationError(dayInterval, CommonErrors.ValidateTimeIntervals_EmptyPartError, ValidationErrorLevel.CannotWrite));
				foreach (var dayIntervalPart in dayInterval.DayIntervalParts)
					if (dayIntervalPart.EndMilliseconds < dayIntervalPart.StartMilliseconds)
					{
                        Errors.Add(new DoorDayIntervalValidationError(dayInterval, CommonErrors.ValidateIntervals_StartLessEndError, ValidationErrorLevel.CannotWrite));
						break;
					}

				var currentDateTime = DateTime.MinValue.TimeOfDay.TotalMilliseconds;
				foreach (var dayIntervalPart in dayInterval.DayIntervalParts)
				{
					if (dayIntervalPart.StartMilliseconds < currentDateTime)
					{
                        Errors.Add(new DoorDayIntervalValidationError(dayInterval, CommonErrors.ValidateTimeIntervals_IntersectError, ValidationErrorLevel.CannotWrite));
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
					Errors.Add(new DoorDayIntervalValidationError(dayInterval, CommonErrors.ValidateDoorTimeIntervals_DublicateError, ValidationErrorLevel.CannotWrite));
		}
	}
}