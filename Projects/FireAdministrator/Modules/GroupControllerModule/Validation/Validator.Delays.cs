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
				ValidateDirectionOnlyOnOneGK(delay);
			}
		}

		/// <summary>
		/// Валидация уникальности номеров задержек
		/// </summary>
		void ValidateDelayNoEquality()
		{
			var nos = new HashSet<int>();
			foreach (var delay in GKManager.Delays)
			{
				if (!nos.Add(delay.No))
					Errors.Add(new DelayValidationError(delay, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		/// <summary>
		/// Валидация непустого названия задержки
		/// </summary>
		/// <param name="delay"></param>
		void ValidateDelay(GKDelay delay)
		{
			if (string.IsNullOrEmpty(delay.Name))
				Errors.Add(new DelayValidationError(delay, "Пустое название", ValidationErrorLevel.CannotWrite));
		}

		/// <summary>
		/// Направление должно зависеть от объектов, присутствующие на одном и только на одном ГК
		/// </summary>
		/// <param name="code"></param>
		bool ValidateDirectionOnlyOnOneGK(GKDelay delay)
		{
			if (delay.GkParents.Count == 0)
			{
				Errors.Add(new DelayValidationError(delay, "Пустые зависимости", ValidationErrorLevel.CannotWrite));
				return false;
			}

			if (delay.GkParents.Count > 1)
			{
				Errors.Add(new DelayValidationError(delay, "Задержка содержит объекты разных ГК", ValidationErrorLevel.CannotWrite));
				return false;
			}
			return true;
		}
	}
}