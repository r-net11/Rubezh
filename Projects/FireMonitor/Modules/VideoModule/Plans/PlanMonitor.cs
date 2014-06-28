using System;
using System.Linq;
using Common;
using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrastructure.Client.Plans.Presenter;

namespace VideoModule.Plans
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
			foreach (var elementCamera in Plan.ElementExtensions.OfType<ElementCamera>())
			{
				var camera = PlanPresenter.Cache.GetItem(elementCamera.CameraUID);
				AddState((IDeviceState<XStateClass>)camera);
			}
		}
	}
}