using System.Collections.Generic;
using RubezhClient;
using Infrastructure.Common.Windows.Validation;

namespace AutomationModule.Validation
{
	public partial class Validator
	{
		private void ValidateGlobalVariableName()
		{
			var nameList = new List<string>();
			foreach (var globalVariable in ClientManager.SystemConfiguration.AutomationConfiguration.GlobalVariables)
			{
				if (nameList.Contains(globalVariable.Name))
					Errors.Add(new VariableValidationError(globalVariable, "Глобальная переменная с таким именем уже существует " + globalVariable.Name, ValidationErrorLevel.CannotSave));
				nameList.Add(globalVariable.Name);
			}
		}
	}
}