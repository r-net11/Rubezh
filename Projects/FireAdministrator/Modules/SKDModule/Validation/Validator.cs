using System.Collections.Generic;
using Infrastructure.Common.Validation;
using FiresecAPI.SKD;

namespace SKDModule.Validation
{
	public partial class Validator
	{
		List<IValidationError> Errors { get; set; }

		public IEnumerable<IValidationError> Validate()
		{
			Errors = new List<IValidationError>();
			return Errors;
		}
	}
}