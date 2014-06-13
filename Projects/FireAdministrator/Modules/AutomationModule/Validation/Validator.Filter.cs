using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace AutomationModule.Validation
{
	public partial class Validator
	{
		private void ValidateFilterName()
		{
			var nameList = new List<string>();
			foreach (var filter in FiresecManager.SystemConfiguration.Filters)
			{
				if (nameList.Contains(filter.Name))
					Errors.Add(new FilterValidationError(filter, "Фильтр с таким именем уже существует " + filter.Name, ValidationErrorLevel.CannotSave));
				nameList.Add(filter.Name);
			}
		}
	}
}