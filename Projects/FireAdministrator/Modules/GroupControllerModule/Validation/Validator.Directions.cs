using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Validation;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateDirections()
		{
			ValidateDirectionNoEquality();

			foreach (var direction in GKManager.Directions)
			{
				ValidateDirectionOnlyOnOneGK(direction);
			}
		}

		/// <summary>
		/// Валидация уникальности номеров направлений
		/// </summary>
		void ValidateDirectionNoEquality()
		{
			var nos = new HashSet<int>();
			foreach (var direction in GKManager.Directions)
			{
				if (!nos.Add(direction.No))
					Errors.Add(new DirectionValidationError(direction, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		/// <summary>
		/// Направление должно зависеть от объектов, присутствующие на одном и только на одном ГК
		/// </summary>
		/// <param name="code"></param>
		bool ValidateDirectionOnlyOnOneGK(GKDirection direction)
		{
			if (direction.GkParents.Count == 0)
			{
				Errors.Add(new DirectionValidationError(direction, "Пустые зависимости", ValidationErrorLevel.CannotWrite));
				return false;
			}

			if (direction.GkParents.Count > 1)
			{
				Errors.Add(new DirectionValidationError(direction, "Направление содержит объекты разных ГК", ValidationErrorLevel.CannotWrite));
				return false;
			}
			return true;
		}
	}
}