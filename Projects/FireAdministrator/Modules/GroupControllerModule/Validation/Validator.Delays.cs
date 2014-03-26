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
			}
		}

		void ValidateDelayNoEquality()
		{
			var delayNames = new HashSet<string>();
			foreach (var delay in XManager.Delays)
			{
				if (!delayNames.Add(delay.Name))
					Errors.Add(new DelayValidationError(delay, "Дублируется название", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateDelay(XDelay delay)
		{
			if (string.IsNullOrEmpty(delay.Name))
				Errors.Add(new DelayValidationError(delay, "Пустое название", ValidationErrorLevel.CannotWrite));
		}
	}
}