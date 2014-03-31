using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Validation;
using XFiresecAPI;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateDelays()
		{
			ValidateDelayNoEquality();

			foreach (var delay in XManager.Delays)
			{
				ValidateDelay(delay);
				ValidateDelaySelfLogic(delay);
			}
		}

		void ValidateDelayNoEquality()
		{
			var delayNos = new HashSet<int>();
			foreach (var delay in XManager.Delays)
			{
				if (!delayNos.Add(delay.No))
					Errors.Add(new DelayValidationError(delay, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateDelay(XDelay delay)
		{
			if (string.IsNullOrEmpty(delay.Name))
				Errors.Add(new DelayValidationError(delay, "Пустое название", ValidationErrorLevel.CannotWrite));
		}

		void ValidateDelaySelfLogic(XDelay delay)
		{
			if(delay.ClauseInputDelays.Contains(delay))
				Errors.Add(new DelayValidationError(delay, "Задержка зависит от самой себя", ValidationErrorLevel.CannotWrite));
		}
	}
}