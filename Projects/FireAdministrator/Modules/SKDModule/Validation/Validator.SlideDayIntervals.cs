using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Validation;

namespace SKDModule.Validation
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
				if (slideDayInterval.TimeIntervalIDs.Count == 0 )
					Errors.Add(new SlideDayIntervalValidationError(slideDayInterval, "Отсутствуют составляющие части скользящего посуточного графика", ValidationErrorLevel.CannotWrite));
				else if (slideDayInterval.TimeIntervalIDs.All(item => item == 0))
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