using System.Collections.Generic;
using System.Linq;
using StrazhAPI.SKD;
using Infrastructure.Common.Validation;

namespace StrazhModule.Validation
{
	public partial class Validator
	{
		void ValidateSlideWeklyIntervals()
		{
			ValidateSlideWeeklyIntervalEquality();
			foreach (var slideWeeklyInterval in SKDManager.TimeIntervalsConfiguration.SlideWeeklyIntervals)
			{
				if (string.IsNullOrEmpty(slideWeeklyInterval.Name))
					Errors.Add(new SlideWeeklyIntervalValidationError(slideWeeklyInterval, "Отсутствует название скользящего понедельного графика", ValidationErrorLevel.CannotWrite));
				if (slideWeeklyInterval.WeeklyIntervalIDs.Count == 0)
					Errors.Add(new SlideWeeklyIntervalValidationError(slideWeeklyInterval, "Отсутствуют составляющие части скользящего понедельного графика", ValidationErrorLevel.CannotWrite));
				else if (slideWeeklyInterval.WeeklyIntervalIDs.All(item => item == 0))
					Errors.Add(new SlideWeeklyIntervalValidationError(slideWeeklyInterval, "Все составляющие части скользящего понедельного графика пустые", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateSlideWeeklyIntervalEquality()
		{
			var slideWeeklyIntervals = new HashSet<string>();
			foreach (var slideWeeklyInterval in SKDManager.TimeIntervalsConfiguration.SlideWeeklyIntervals)
				if (!slideWeeklyIntervals.Add(slideWeeklyInterval.Name))
					Errors.Add(new SlideWeeklyIntervalValidationError(slideWeeklyInterval, "Дублируется название скользящего понедельного графика", ValidationErrorLevel.CannotWrite));
		}
	}
}