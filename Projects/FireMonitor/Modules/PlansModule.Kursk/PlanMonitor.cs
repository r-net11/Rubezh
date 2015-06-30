using System;
using FiresecAPI.Models;
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