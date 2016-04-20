using System.Collections.Generic;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows.Validation;
using System;
using RubezhAPI;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateDelays()
		{
			ValidateCommon(GKManager.Delays);

			foreach (var delay in GKManager.Delays)
			{
			}
		}
	}
}