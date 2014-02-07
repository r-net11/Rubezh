using System;
using System.Collections.Generic;
using FiresecAPI;
using Infrastructure.Common.Validation;

namespace SKDModule.Validation
{
	public static partial class Validator
	{
		static void ValidateSlideDayIntervals()
		{
			ValidateSlideDayIntervalEquality();
		}

		static void ValidateSlideDayIntervalEquality()
		{
			var slideDayIntervals = new HashSet<string>();
			foreach (var slideDayInterval in SKDManager.SKDConfiguration.SlideDayIntervals)
			{
				if (!slideDayIntervals.Add(slideDayInterval.Name))
				{
					Errors.Add(new SlideDayIntervalValidationError(slideDayInterval, "Дублируется название интервала", ValidationErrorLevel.CannotWrite));
				}
			}
		}
	}
}