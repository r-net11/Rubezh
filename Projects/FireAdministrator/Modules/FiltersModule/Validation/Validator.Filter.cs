using System.Collections.Generic;
using RubezhClient;
using Infrastructure.Common.Validation;

namespace FilterModule.Validation
{
	public partial class Validator
	{
		private void ValidateFilterName()
		{
			var nameList = new List<string>();
			foreach (var filter in ClientManager.SystemConfiguration.JournalFilters)
			{
				if (nameList.Contains(filter.Name))
					Errors.Add(new FilterValidationError(filter, "Фильтр с таким именем уже существует " + filter.Name, ValidationErrorLevel.CannotWrite));
				nameList.Add(filter.Name);
			}
		}
	}
}