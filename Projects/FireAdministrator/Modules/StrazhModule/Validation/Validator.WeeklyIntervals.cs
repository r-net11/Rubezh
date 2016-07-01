using System.Collections.Generic;
using System.Linq;
using Localization.Strazh.Errors;
using StrazhAPI.SKD;
using Infrastructure.Common.Validation;
using System;

namespace StrazhModule.Validation
{
	public partial class Validator
	{
		void ValidateWeeklyIntervals()
		{
			ValidateWeeklyIntervalEquality();
			foreach (var weeklyInterval in SKDManager.TimeIntervalsConfiguration.WeeklyIntervals)
			{
				// Пропускаем предустановленные недельные графики доступа
				if (weeklyInterval.Name == TimeIntervalsConfiguration.PredefinedIntervalNameNever || weeklyInterval.Name == TimeIntervalsConfiguration.PredefinedIntervalNameAlways)
				{
					continue;
				}

				if (string.IsNullOrEmpty(weeklyInterval.Name))
					Errors.Add(new WeeklyIntervalValidationError(weeklyInterval, CommonErrors.ValidateWeeklyIntervals_EmptyNameError, ValidationErrorLevel.CannotWrite));
				if (weeklyInterval.WeeklyIntervalParts.All(item => item.DayIntervalUID == Guid.Empty))
					Errors.Add(new WeeklyIntervalValidationError(weeklyInterval, CommonErrors.ValidateDoorWeeklyIntervals_EmptyError, ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateWeeklyIntervalEquality()
		{
			var weeklyIntervals = new HashSet<string>();
			foreach (var weeklyInterval in SKDManager.TimeIntervalsConfiguration.WeeklyIntervals)
				if (!weeklyIntervals.Add(weeklyInterval.Name))
                    Errors.Add(new WeeklyIntervalValidationError(weeklyInterval, CommonErrors.ValidateDoorWeeklyIntervals_DublicateError, ValidationErrorLevel.CannotWrite));
		}
	}
}