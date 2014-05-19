﻿using System;
using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrustructure.Plans.Presenter;

namespace PlansModule.Kursk
{
	internal class PlanMonitor : BaseMonitor<Plan>
	{
		public PlanMonitor(Plan plan, Action callBack)
			: base(plan, callBack)
		{
			Initialize();
		}
		private void Initialize()
		{
		}
		public XStateClass GetState()
		{
			var result = XStateClass.No;
			return result;
		}
	}
}