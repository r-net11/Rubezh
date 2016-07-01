using System.Collections.Generic;
using System.Linq;
using Localization.Strazh.Errors;
using StrazhAPI.SKD;
using Infrastructure.Common.Validation;

namespace StrazhModule.Validation
{
	public partial class Validator
	{
		void ValidateSlideDayIntervals()
		{
			ValidateSlideDayIntervalEquality();
			foreach (var slideDayInterval in SKDManager.TimeIntervalsConfiguration.SlideDayIntervals)
			{
				if (string.IsNullOrEmpty(slideDayInterval.Name))
					Errors.Add(new SlideDayIntervalValidationError(slideDayInterval, CommonErrors.ValidateSlideDayIntervals_EmptyNameError, ValidationErrorLevel.CannotWrite));
				if (slideDayInterval.DayIntervalIDs.Count == 0 )
					Errors.Add(new SlideDayIntervalValidationError(slideDayInterval, CommonErrors.ValidateSlideDayIntervals_EmptyPartError, ValidationErrorLevel.CannotWrite));
				else if (slideDayInterval.DayIntervalIDs.All(item => item == 0))
					Errors.Add(new SlideDayIntervalValidationError(slideDayInterval, CommonErrors.ValidateSlideDayIntervals_EmptyError, ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateSlideDayIntervalEquality()
		{
			var slideDayIntervals = new HashSet<string>();
			foreach (var slideDayInterval in SKDManager.TimeIntervalsConfiguration.SlideDayIntervals)
				if (!slideDayIntervals.Add(slideDayInterval.Name))
					Errors.Add(new SlideDayIntervalValidationError(slideDayInterval, CommonErrors.ValidateSlideDayIntervals_DublicateError, ValidationErrorLevel.CannotWrite));
		}
	}
}