using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Validation;
using RubezhAPI;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateDirections()
		{
			ValidateCommon(GKManager.Directions);

			foreach (var direction in GKManager.Directions)
			{
			}
		}
	}
}