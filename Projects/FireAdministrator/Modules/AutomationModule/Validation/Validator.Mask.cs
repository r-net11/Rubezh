using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace AutomationModule.Validation
{
	public partial class Validator
	{
		private void ValidateMaskName()
		{
			var nameList = new List<string>();
			foreach (var mask in FiresecManager.SystemConfiguration.Masks)
			{
				if (nameList.Contains(mask.Name))
					Errors.Add(new MaskValidationError(mask, "Маска с таким именем уже существует " + mask.Name, ValidationErrorLevel.CannotSave));
				nameList.Add(mask.Name);
			}
		}
	}
}
