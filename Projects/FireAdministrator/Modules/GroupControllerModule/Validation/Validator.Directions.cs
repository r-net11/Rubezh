using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
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

		void ValidateEmpty(GKDirection direction)
		{
			if (direction.DataBaseParent == null)
			{
				Errors.Add(new DirectionValidationError(direction, "Пустые зависимости", ValidationErrorLevel.CannotWrite));
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

		void ValidateDifferentGK(GKDirection direction)
		{
			var devices = direction.Logic.GetObjects().Select(x => x.GkDatabaseParent);
			if (AreDevicesInSameGK(devices))
				Errors.Add(new DirectionValidationError(direction, "Направление содержит объекты устройства разных ГК", ValidationErrorLevel.CannotWrite));
		}
	}
}