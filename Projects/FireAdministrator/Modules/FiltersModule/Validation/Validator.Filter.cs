using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Validation;
using Localization.Filter.Errors;

namespace FilterModule.Validation
{
	public partial class Validator
	{
		private void ValidateFilterName()
		{
			var nameList = new List<string>();
			foreach (var filter in FiresecManager.SystemConfiguration.JournalFilters)
			{
				if (nameList.Contains(filter.Name))
					Errors.Add(new FilterValidationError(filter, string.Format(CommonErrors.ValidatorFilter_Error, filter.Name), ValidationErrorLevel.CannotWrite));
				nameList.Add(filter.Name);
			}
		}
	}
}