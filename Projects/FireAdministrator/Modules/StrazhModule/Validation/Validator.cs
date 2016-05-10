using System.Collections.Generic;
using Infrastructure.Common.Validation;
using StrazhAPI.SKD;

namespace StrazhModule.Validation
{
	public partial class Validator
	{
		List<IValidationError> Errors { get; set; }

		public IEnumerable<IValidationError> Validate()
		{
			SKDManager.UpdateConfiguration();
			Errors = new List<IValidationError>();
			ValidateDevices();
			ValidateZones();
			ValidateDoors();
			ValidateTimeIntervals();
			ValidateWeeklyIntervals();
			ValidateSlideDayIntervals();
			ValidateSlideWeklyIntervals();
			ValidateDoorTimeIntervals();
			ValidateDoorWeeklyIntervals();
			ValidateHolidays();
			ValidatePlans();
			return Errors;
		}
	}
}