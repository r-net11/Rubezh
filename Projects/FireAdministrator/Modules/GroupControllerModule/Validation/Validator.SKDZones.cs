using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using RubezhClient;
using Infrastructure.Common.Validation;
using Infrastructure.Common;
using FiresecAPI;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateSKDZones()
		{
			ValidateSKDZoneNoEquality();

			foreach (var zone in GKManager.SKDZones)
			{
			}
		}

		void ValidateSKDZoneNoEquality()
		{
			var zoneNos = new HashSet<int>();
			foreach (var zone in GKManager.SKDZones)
			{
				if (!zoneNos.Add(zone.No))
					Errors.Add(new SKDZoneValidationError(zone, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}
	}
}