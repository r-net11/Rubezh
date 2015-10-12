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
				if (IsManyGK)
					ValidateDifferentGK(direction);
				ValidateEmpty(direction);
			}
		}

		void ValidateDirectionNoEquality()
		{
			var directionNos = new HashSet<int>();
			foreach (var direction in GKManager.Directions)
			{
				if (!directionNos.Add(direction.No))
					Errors.Add(new DirectionValidationError(direction, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateEmpty(GKDirection direction)
		{
			if (direction.DataBaseParent == null)
			{
				Errors.Add(new DirectionValidationError(direction, "Пустые зависимости", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateDifferentGK(GKDirection direction)
		{
			if (direction.GkParents.Count > 1)
				Errors.Add(new DirectionValidationError(direction, "Направление содержит объекты разных ГК", ValidationErrorLevel.CannotWrite));
		}
	}
}