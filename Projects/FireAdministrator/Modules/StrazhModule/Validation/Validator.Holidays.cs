using System;
using System.Collections.Generic;
using StrazhAPI.SKD;
using Infrastructure.Common.Validation;

namespace StrazhModule.Validation
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
					Errors.Add(new HolidayValidationError(holiday, "Отсутствует название праздника", ValidationErrorLevel.CannotSave));
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
					Errors.Add(new HolidayValidationError(holiday, "Дублируется дата праздника", ValidationErrorLevel.CannotSave));
				}
			}
		}
	}
}