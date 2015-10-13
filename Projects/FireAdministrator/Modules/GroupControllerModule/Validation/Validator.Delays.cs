using System.Collections.Generic;
using RubezhAPI.GK;
using RubezhClient;
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
				if (IsManyGK)
					ValidateDelayDifferentGK(delay);
				ValidateEmpty(delay);
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

		void ValidateEmpty(GKDelay delay)
		{
			if (delay.DataBaseParent == null)
			{
				Errors.Add(new DelayValidationError(delay, "Пустые зависимости", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateDelayDifferentGK(GKDelay delay)
		{
			if (delay.GkParents.Count > 1)
				Errors.Add(new DelayValidationError(delay, "Задержка содержит объекты разных ГК", ValidationErrorLevel.CannotWrite));
		}
	}
}