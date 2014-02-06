using System;
using System.Collections.Generic;
using FiresecAPI;
using Infrastructure.Common.Validation;

namespace SKDModule.Validation
{
	public static partial class Validator
	{
		static void ValidateWeklyIntervals()
		{
			ValidateWeeklyIntervalEquality();
		}

		static void ValidateWeeklyIntervalEquality()
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