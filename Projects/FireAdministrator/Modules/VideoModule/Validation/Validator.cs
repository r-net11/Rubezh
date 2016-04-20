using System.Collections.Generic;
using RubezhClient;
using Infrastructure.Common.Validation;

namespace VideoModule.Validation
{
	public partial class Validator
	{
		List<IValidationError> Errors { get; set; }

		public IEnumerable<IValidationError> Validate()
		{
			Errors = new List<IValidationError>();
			ValidateAddress();
			ValidateLicense();
			return Errors;
		}
	}
}