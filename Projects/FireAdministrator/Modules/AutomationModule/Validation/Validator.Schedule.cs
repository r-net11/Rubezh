using System.Collections.Generic;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace AutomationModule.Validation
{
	public partial class Validator
	{
		private void ValidateScheduleName()
		{
			var nameList = new List<string>();
			foreach (var schedule in FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSchedules)
			{
				if (nameList.Contains(schedule.Name))
					Errors.Add(new ScheduleValidationError(schedule, "Расписание с таким именем уже существует " + schedule.Name, ValidationErrorLevel.CannotSave));
				nameList.Add(schedule.Name);
				if ((schedule.IsPeriodSelected)&&
					((schedule.Year == -1) || (schedule.Month == -1) || (schedule.Day == -1) || (schedule.Hour == -1) ||
					(schedule.Minute == -1) || (schedule.Second == -1)))
					Errors.Add(new ScheduleValidationError(schedule, "Должна быть задана конкретная дата начала периода " + schedule.Name, ValidationErrorLevel.CannotSave));
				if ((schedule.IsPeriodSelected)&&(schedule.PeriodDay == 0)&&(schedule.PeriodHour == 0)&&
					(schedule.PeriodMinute == 0)&&(schedule.PeriodSecond == 0))
					Errors.Add(new ScheduleValidationError(schedule, "Должен быть задан не нулевой период " + schedule.Name, ValidationErrorLevel.CannotSave));
			}
		}
	}
}