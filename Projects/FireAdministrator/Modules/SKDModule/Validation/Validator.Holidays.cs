using System;
using System.Collections.Generic;
using FiresecAPI;
using Infrastructure.Common.Validation;

namespace SKDModule.Validation
{
	public partial class Validator
	{
		void ValidateHolidays()
		{
			ValidateHolidayEquality();
			foreach (var holiday in SKDManager.TimeIntervalsConfiguration.Holidays)
			{
				if (string.IsNullOrEmpty(holiday.Name))
				{
					Errors.Add(new HolidayValidationError(holiday, "Отсутствует название праздника", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidateHolidayEquality()
		{
			var holidays = new HashSet<DateTime>();
			foreach (var holiday in SKDManager.TimeIntervalsConfiguration.Holidays)
			{
				if (!holidays.Add(new DateTime(2000, holiday.DateTime.Month, holiday.DateTime.Day)))
				{
					Errors.Add(new HolidayValidationError(holiday, "Дублируется дата праздника", ValidationErrorLevel.CannotWrite));
				}
			}
		}
	}
}