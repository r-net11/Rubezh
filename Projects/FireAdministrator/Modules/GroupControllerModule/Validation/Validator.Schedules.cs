using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateSchedules()
		{
			ValidateScheduleNoEquality();

			foreach (var schedule in GKManager.DeviceConfiguration.Schedules)
			{
				ValidateSchedule(schedule);
			}
		}

		void ValidateScheduleNoEquality()
		{
			var scheduleNos = new HashSet<int>();
			foreach (var schedule in GKManager.DeviceConfiguration.Schedules)
			{
				if (!scheduleNos.Add(schedule.No))
					Errors.Add(new ScheduleValidationError(schedule, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateSchedule(GKSchedule schedule)
		{
		}
	}
}