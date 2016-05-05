using System.Collections.Generic;
using System.Linq;
using StrazhAPI.SKD;
using Infrastructure.Common.Validation;
using System;

namespace StrazhModule.Validation
{
	public partial class Validator
	{
		void ValidateDoorWeeklyIntervals()
		{
			ValidateDoorWeeklyIntervalEquality();
			foreach (var weeklyInterval in SKDManager.TimeIntervalsConfiguration.DoorWeeklyIntervals)
			{
				// Пропускаем предустановленные недельные графики замка
				if (weeklyInterval.Name == TimeIntervalsConfiguration.PredefinedIntervalNameCard
					|| weeklyInterval.Name == TimeIntervalsConfiguration.PredefinedIntervalNamePassword
					|| weeklyInterval.Name == TimeIntervalsConfiguration.PredefinedIntervalNameCardAndPassword)
				{
					continue;
				}

				if (string.IsNullOrEmpty(weeklyInterval.Name))
					Errors.Add(new DoorWeeklyIntervalValidationError(weeklyInterval, "Отсутствует название недельного графика", ValidationErrorLevel.CannotWrite));
				if (weeklyInterval.WeeklyIntervalParts.All(item => item.DayIntervalUID == Guid.Empty))
					Errors.Add(new DoorWeeklyIntervalValidationError(weeklyInterval, "Все составляющие части недельного графика пустые", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateDoorWeeklyIntervalEquality()
		{
			var weeklyIntervals = new HashSet<string>();
			foreach (var weeklyInterval in SKDManager.TimeIntervalsConfiguration.DoorWeeklyIntervals)
				if (!weeklyIntervals.Add(weeklyInterval.Name))
					Errors.Add(new DoorWeeklyIntervalValidationError(weeklyInterval, "Дублируется название недельного графика", ValidationErrorLevel.CannotWrite));
		}
	}
}