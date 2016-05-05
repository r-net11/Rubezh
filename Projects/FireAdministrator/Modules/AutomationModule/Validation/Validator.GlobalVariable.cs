using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace AutomationModule.Validation
{
	public partial class Validator
	{
		private void ValidateGlobalVariableName()
		{
			var nameList = new List<string>();
			//foreach (var globalVariable in FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables) TODO: Validate global Variable
			//{
			//	if (nameList.Contains(globalVariable.Name))
			//		Errors.Add(new VariableValidationError(globalVariable, "Глобальная переменная с таким именем уже существует " + globalVariable.Name, ValidationErrorLevel.CannotSave));
			//	nameList.Add(globalVariable.Name);
			//}
		}
	}
}