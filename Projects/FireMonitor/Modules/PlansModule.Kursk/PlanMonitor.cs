using System;
using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrustructure.Plans.Presenter;
using Infrastructure.Client.Plans.Presenter;

namespace PlansModule.Kursk
{
	internal class PlanMonitor : StateMonitor
	{
		public PlanMonitor(Plan plan, Action callBack)
			: base(plan, callBack)
		{
			Initialize();
		}
		private void Initialize()
		{
		}
	}
}