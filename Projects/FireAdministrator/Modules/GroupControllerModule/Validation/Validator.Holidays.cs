using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateHolidays()
		{
			ValidateHolidayNoEquality();

			foreach (var holiday in GKManager.DeviceConfiguration.Holidays)
			{
				ValidateHoliday(holiday);
			}
		}

		void ValidateHolidayNoEquality()
		{
			var holidayNos = new HashSet<int>();
			foreach (var holiday in GKManager.DeviceConfiguration.Holidays)
			{
				if (!holidayNos.Add(holiday.No))
					Errors.Add(new HolidayValidationError(holiday, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateHoliday(GKHoliday holiday)
		{
			Errors.Add(new HolidayValidationError(holiday, "Ощибка праздничного дня", ValidationErrorLevel.CannotWrite));
		}
	}
}