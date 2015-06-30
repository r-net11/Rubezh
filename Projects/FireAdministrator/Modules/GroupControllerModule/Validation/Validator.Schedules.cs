using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Validation;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateSchedules()
		{
			var schedules = GKScheduleHelper.GetSchedules();
			if (schedules == null)
				return;

			ValidateScheduleNoEquality(schedules);

			foreach (var schedule in schedules)
			{
				ValidateSchedule(schedule);
			}
		}

		void ValidateScheduleNoEquality(List<GKSchedule> schedules)
		{
			var scheduleNos = new HashSet<int>();
			foreach (var schedule in schedules)
			{
				if (!scheduleNos.Add(schedule.No))
					Errors.Add(new ScheduleValidationError(schedule, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateSchedule(GKSchedule schedule)
		{
			if(schedule.DayScheduleUIDs.Count > 250)
				Errors.Add(new ScheduleValidationError(schedule, "Количество интервалов графика не должно превышать 250", ValidationErrorLevel.CannotWrite));
		}
	}
}