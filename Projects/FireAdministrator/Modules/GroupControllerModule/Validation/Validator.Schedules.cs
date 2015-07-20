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
		}
	}
}