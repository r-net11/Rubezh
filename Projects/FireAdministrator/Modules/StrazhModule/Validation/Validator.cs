﻿using System.Collections.Generic;
using Infrastructure.Common.Validation;
using FiresecAPI.SKD;

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
			ValidateWeklyIntervals();
			ValidateSlideDayIntervals();
			ValidateSlideWeklyIntervals();
			ValidateHolidays();
			ValidatePlans();
			return Errors;
		}
	}
}