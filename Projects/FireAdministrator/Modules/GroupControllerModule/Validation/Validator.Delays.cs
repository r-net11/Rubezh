﻿using System.Collections.Generic;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Validation;
using System;

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