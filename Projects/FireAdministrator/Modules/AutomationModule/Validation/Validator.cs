using System.Collections.Generic;
using RubezhClient;
using Infrastructure.Common.Windows.Validation;

namespace AutomationModule.Validation
{
	public partial class Validator
	{
		List<IValidationError> Errors { get; set; }

		public IEnumerable<IValidationError> Validate()
		{
			ClientManager.SystemConfiguration.AutomationConfiguration.UpdateConfiguration();
			Errors = new List<IValidationError>();
			ValidateGlobalVariableName();
			ValidateProcedure();
			ValidateSchedule();
			ValidateSoundName();
			return Errors;
		}
	}
}