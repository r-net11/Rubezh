﻿using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace AutomationModule.Validation
{
	public partial class Validator
	{
		List<IValidationError> Errors { get; set; }

		public IEnumerable<IValidationError> Validate()
		{
			XManager.UpdateConfiguration();
			Errors = new List<IValidationError>();
			ValidateGlobalVariableName();
			ValidateFilterName();
			ValidateProcedureName();
			ValidateScheduleName();
			ValidateSoundName();
			return Errors;
		}
	}
}