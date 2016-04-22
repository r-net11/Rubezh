using System;
using System.Linq;
using Common;
using RubezhAPI.GK;
using RubezhAPI.Models;
using Infrastructure.Client.Plans.Presenter;
using RubezhAPI;

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
				var camera = PlanPresenter.Cache.Get<Camera>(elementCamera.CameraUID);
				AddState((IDeviceState)camera.CameraState);
			}
		}
	}
}