using System.Collections.Generic;
using Infrastructure.Common.Validation;
using StrazhAPI.SKD;

namespace SKDModule.Validation
{
	public partial class Validator
	{
		List<IValidationError> Errors { get; set; }

		public IEnumerable<IValidationError> Validate()
		{
			SKDManager.UpdateConfiguration();
			Errors = new List<IValidationError>();
			return Errors;
		}
	}
}