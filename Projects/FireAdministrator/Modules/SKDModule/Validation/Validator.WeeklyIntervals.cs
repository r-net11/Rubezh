using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Validation;
using System;

namespace SKDModule.Validation
{
	public partial class Validator
	{
		void ValidateWeklyIntervals()
		{
			ValidateWeeklyIntervalEquality();
			foreach (var weeklyInterval in SKDManager.TimeIntervalsConfiguration.WeeklyIntervals)
			{
				if (weeklyInterval.Name == "<Никогда>" || weeklyInterval.Name == "<Всегда>")
				{
					continue;
				}

				if (string.IsNullOrEmpty(weeklyInterval.Name))
					Errors.Add(new WeeklyIntervalValidationError(weeklyInterval, "Отсутствует название недельного графика", ValidationErrorLevel.CannotWrite));
				if (weeklyInterval.WeeklyIntervalParts.All(item => item.DayIntervalUID == Guid.Empty))
					Errors.Add(new WeeklyIntervalValidationError(weeklyInterval, "Все составляющие части недельного графика пустые", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateWeeklyIntervalEquality()
		{
			var weeklyIntervals = new HashSet<string>();
			foreach (var weeklyInterval in SKDManager.TimeIntervalsConfiguration.WeeklyIntervals)
				if (!weeklyIntervals.Add(weeklyInterval.Name))
					Errors.Add(new WeeklyIntervalValidationError(weeklyInterval, "Дублируется название недельного графика", ValidationErrorLevel.CannotWrite));
		}
	}
}