using System;
using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrastructure.Client.Plans.Presenter;
using Infrustructure.Plans.Elements;

namespace GKModule.Plans
{
	internal class PlanMonitor : StateMonitor
	{
		public PlanMonitor(Plan plan, Action callBack)
			: base(plan, callBack)
		{
			Plan.ElementXDevices.ForEach(item => Initialize(item));
			Plan.ElementRectangleXZones.ForEach(item => Initialize(item));
			Plan.ElementPolygonXZones.ForEach(item => Initialize(item));
			Plan.ElementRectangleXGuardZones.ForEach(item => InitializeGuard(item));
			Plan.ElementPolygonXGuardZones.ForEach(item => InitializeGuard(item));
			Plan.ElementRectangleXDirections.ForEach(item => Initialize(item));
			Plan.ElementPolygonXDirections.ForEach(item => Initialize(item));
		}

		private void Initialize(ElementXDevice element)
		{
			var device = PlanPresenter.Cache.Get<XDevice>(element.XDeviceUID);
			AddState(device);
		}
		private void Initialize(IElementZone element)
		{
			var zone = PlanPresenter.Cache.Get<XZone>(element.ZoneUID);
			AddState(zone);
		}
		private void InitializeGuard(IElementZone element)
		{
			var zone = PlanPresenter.Cache.Get<XGuardZone>(element.ZoneUID);
			AddState(zone);
		}
		private void Initialize(IElementDirection element)
		{
			var direction = PlanPresenter.Cache.Get<XDirection>(element.DirectionUID);
			AddState(direction);
		}
	}
}