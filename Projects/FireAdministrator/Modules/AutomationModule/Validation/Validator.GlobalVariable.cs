using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Validation;
using Localization.Automation.Errors;

namespace AutomationModule.Validation
{
	public partial class Validator
	{
		private void ValidateGlobalVariableName()
		{
			var nameList = new List<string>();
			foreach (var globalVariable in FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables)
			{
				if (nameList.Contains(globalVariable.Name))
					Errors.Add(new VariableValidationError(globalVariable, string.Format(CommonErrors.ValidatorGlobalVariable_Error, globalVariable.Name), ValidationErrorLevel.CannotSave));
				nameList.Add(globalVariable.Name);
			}
		}
	}
}