using Infrastructure.Plans.Presenter;
using RubezhAPI.Models;
using System;

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