using System.Collections.Generic;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure.Common.Validation;
using System.Linq;

namespace AutomationModule.Validation
{
	public partial class Validator
	{
		private void ValidateSchedule()
		{
			var nameList = new List<string>();
			foreach (var schedule in FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSchedules)
			{
				foreach (var scheduleProcedure in schedule.ScheduleProcedures)
					ValidateScheduleProcedure(schedule, scheduleProcedure);
				if (nameList.Contains(schedule.Name))
                    Errors.Add(new ScheduleValidationError(schedule, string.Format(Resources.Language.Validation.ValidatorSchedule.SameName_Error, schedule.Name), ValidationErrorLevel.CannotSave));
				nameList.Add(schedule.Name);
				if ((schedule.IsPeriodSelected)&&
					((schedule.Year == -1) || (schedule.Month == -1) || (schedule.Day == -1) || (schedule.Hour == -1) ||
					(schedule.Minute == -1) || (schedule.Second == -1)))
					Errors.Add(new ScheduleValidationError(schedule, string.Format(Resources.Language.Validation.ValidatorSchedule.BeginDate_Error,schedule.Name), ValidationErrorLevel.CannotSave));
				if ((schedule.IsPeriodSelected)&&(schedule.PeriodDay == 0)&&(schedule.PeriodHour == 0)&&
					(schedule.PeriodMinute == 0)&&(schedule.PeriodSecond == 0))
					Errors.Add(new ScheduleValidationError(schedule, string.Format(Resources.Language.Validation.ValidatorSchedule.NullPeriod_Error,schedule.Name), ValidationErrorLevel.CannotSave));
			}
		}

		bool ValidateScheduleProcedure(AutomationSchedule schedule, ScheduleProcedure scheduleProcedure)
		{
			foreach (var argument in scheduleProcedure.Arguments)
			{
				//if (argument.VariableScope != VariableScope.ExplicitValue)
					//if (FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables.All(x => x.Uid != argument.VariableUid)) TODO: validate Global Variables
					//{
					//	Errors.Add(new ScheduleValidationError(schedule, "Все переменные должны быть инициализированы " + schedule.Name, ValidationErrorLevel.CannotSave));
					//	return false;
					//}
			}
			return true;
		}
	}
}