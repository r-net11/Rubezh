using System;
using System.Collections.Generic;
using FiresecAPI;
using Infrastructure.Common.Validation;

namespace SKDModule.Validation
{
	public partial class Validator
	{
		void ValidateWeklyIntervals()
		{
			ValidateWeeklyIntervalEquality();
			foreach (var weeklyInterval in SKDManager.SKDConfiguration.WeeklyIntervals)
			{
				if (string.IsNullOrEmpty(weeklyInterval.Name))
				{
					Errors.Add(new WeeklyIntervalValidationError(weeklyInterval, "Отсутствует название интервала", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidateWeeklyIntervalEquality()
		{
			var weeklyIntervals = new HashSet<string>();
			foreach (var weeklyInterval in SKDManager.SKDConfiguration.WeeklyIntervals)
			{
				if (!weeklyIntervals.Add(weeklyInterval.Name))
				{
					Errors.Add(new WeeklyIntervalValidationError(weeklyInterval, "Дублируется название интервала", ValidationErrorLevel.CannotWrite));
				}
			}
		}
	}
}