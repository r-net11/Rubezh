using System.Collections.Generic;
using RubezhClient;
using Infrastructure.Common.Windows.Validation;

namespace FilterModule.Validation
{
	public partial class Validator
	{
		List<IValidationError> Errors { get; set; }

		public IEnumerable<IValidationError> Validate()
		{
			Errors = new List<IValidationError>();
			ValidateFilterName();
			return Errors;
		}
	}
}