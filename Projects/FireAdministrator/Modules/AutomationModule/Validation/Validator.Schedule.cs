using System.Collections.Generic;
using RubezhAPI.Automation;
using RubezhClient;
using Infrastructure.Common.Validation;
using System.Linq;

namespace AutomationModule.Validation
{
	public partial class Validator
	{
		private void ValidateSchedule()
		{
			var nameList = new List<string>();
			foreach (var schedule in ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSchedules)
			{
				foreach (var scheduleProcedure in schedule.ScheduleProcedures)
					ValidateScheduleProcedure(schedule, scheduleProcedure);
				nameList.Add(schedule.Name);
			}
		}

		bool ValidateScheduleProcedure(AutomationSchedule schedule, ScheduleProcedure scheduleProcedure)
		{
			return true;
		}
	}
}