using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace VideoModule.Validation
{
	public partial class Validator
	{
		List<IValidationError> Errors { get; set; }

		public IEnumerable<IValidationError> Validate()
		{
			XManager.UpdateConfiguration();
			Errors = new List<IValidationError>();
			ValidateAddress();
			return Errors;
		}
	}
}
