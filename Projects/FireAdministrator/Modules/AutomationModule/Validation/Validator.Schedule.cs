using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace AutomationModule.Validation
{
	public partial class Validator
	{
		private void ValidateScheduleName()
		{
			var nameList = new List<string>();
			foreach (var schedule in FiresecManager.SystemConfiguration.AutomationSchedules)
			{
				if (nameList.Contains(schedule.Name))
					Errors.Add(new ScheduleValidationError(schedule, "Расписание с таким именем уже существует " + schedule.Name, ValidationErrorLevel.CannotSave));
				nameList.Add(schedule.Name);
			}
		}
	}
}