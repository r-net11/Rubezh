using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace FilterModule.Validation
{
	public partial class Validator
	{
		List<IValidationError> Errors { get; set; }

		public IEnumerable<IValidationError> Validate()
		{
			XManager.UpdateConfiguration();
			Errors = new List<IValidationError>();
			ValidateFilterName();
			return Errors;
		}
	}
}