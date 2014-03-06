using System.Collections.Generic;
using Infrastructure.Common.Validation;

namespace SKDModule.Validation
{
	public partial class Validator
	{
		List<IValidationError> Errors { get; set; }

		public IEnumerable<IValidationError> Validate()
		{
			Errors = new List<IValidationError>();
			ValidateDevices();
			ValidateZones();
			ValidateTimeIntervals();
			ValidateWeklyIntervals();
			ValidateSlideDayIntervals();
			ValidateSlideWeklyIntervals();
			ValidateHolidays();
			return Errors;
		}
	}
}