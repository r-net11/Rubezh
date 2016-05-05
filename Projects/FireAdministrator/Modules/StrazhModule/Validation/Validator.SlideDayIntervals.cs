using System.Collections.Generic;
using System.Linq;
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
					Errors.Add(new SlideDayIntervalValidationError(slideDayInterval, "Отсутствует название скользящего посуточного графика", ValidationErrorLevel.CannotWrite));
				if (slideDayInterval.DayIntervalIDs.Count == 0 )
					Errors.Add(new SlideDayIntervalValidationError(slideDayInterval, "Отсутствуют составляющие части скользящего посуточного графика", ValidationErrorLevel.CannotWrite));
				else if (slideDayInterval.DayIntervalIDs.All(item => item == 0))
					Errors.Add(new SlideDayIntervalValidationError(slideDayInterval, "Все составляющие части скользящего посуточного графика пустые", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateSlideDayIntervalEquality()
		{
			var slideDayIntervals = new HashSet<string>();
			foreach (var slideDayInterval in SKDManager.TimeIntervalsConfiguration.SlideDayIntervals)
				if (!slideDayIntervals.Add(slideDayInterval.Name))
					Errors.Add(new SlideDayIntervalValidationError(slideDayInterval, "Дублируется название скользящего посуточного графика", ValidationErrorLevel.CannotWrite));
		}
	}
}