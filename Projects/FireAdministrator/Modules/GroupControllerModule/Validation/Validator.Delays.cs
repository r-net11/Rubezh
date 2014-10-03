using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateDelays()
		{
			ValidateDelayNoEquality();

			foreach (var delay in GKManager.Delays)
			{
				ValidateDelay(delay);
				ValidateDelaySelfLogic(delay);
			}
		}

		void ValidateDelayNoEquality()
		{
			var delayNos = new HashSet<int>();
			foreach (var delay in GKManager.Delays)
			{
				if (!delayNos.Add(delay.No))
					Errors.Add(new DelayValidationError(delay, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateDelay(GKDelay delay)
		{
			if (string.IsNullOrEmpty(delay.Name))
				Errors.Add(new DelayValidationError(delay, "Пустое название", ValidationErrorLevel.CannotWrite));
		}

		void ValidateDelaySelfLogic(GKDelay delay)
		{
			if(delay.ClauseInputDelays.Contains(delay))
				Errors.Add(new DelayValidationError(delay, "Задержка зависит от самой себя", ValidationErrorLevel.CannotWrite));
		}
	}
}