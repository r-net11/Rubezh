using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Validation;
using Infrastructure.Common;
using RubezhAPI;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateSKDZones()
		{
			ValidateSKDZoneNoEquality();

			foreach (var zone in GKManager.SKDZones)
			{
				ValidateSKDZoneMaxNo(zone);
			}
		}

		/// <summary>
		/// Валидация уникальности номеров зон СКД
		/// </summary>
		void ValidateSKDZoneNoEquality()
		{
			var nos = new HashSet<int>();
			foreach (var zone in GKManager.SKDZones)
			{
				if (!nos.Add(zone.No))
					AddError(zone, "Дублируется номер", ValidationErrorLevel.CannotWrite);
			}
		}

		/// <summary>
		/// Валидация того, что номер зоны не превышает 255
		/// </summary>
		/// <param name="zone"></param>
		void ValidateSKDZoneMaxNo(GKSKDZone zone)
		{
			if (zone.No > 255)
				AddError(zone, "Номер зоны не должен превышать 255", ValidationErrorLevel.CannotWrite);
		}
	}
}