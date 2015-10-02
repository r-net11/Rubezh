using FiresecClient.SKDHelpers;

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