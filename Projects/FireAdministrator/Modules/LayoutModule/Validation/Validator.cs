using Infrastructure.Common.Validation;
using RubezhClient;
using System.Collections.Generic;

namespace LayoutModule.Validation
{
	public partial class Validator
	{
		List<IValidationError> Errors { get; set; }

		public IEnumerable<IValidationError> Validate()
		{
			ClientManager.LayoutsConfiguration.Update();
			Errors = new List<IValidationError>();
			ValidateLicense();
			return Errors;
		}
	}
}