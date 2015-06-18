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
			Plan.ElementGKDevices.ForEach(item => Initialize(item));
			Plan.ElementRectangleGKSKDZones.ForEach(item => InitializeSKD(item));
			Plan.ElementPolygonGKSKDZones.ForEach(item => InitializeSKD(item));
			Plan.ElementGKDoors.ForEach(item => Initialize(item));
		}

		private void Initialize(ElementGKDevice element)
		{
			var device = PlanPresenter.Cache.Get<GKDevice>(element.DeviceUID);
			AddState(device);
		}

		private void Initialize(ElementGKDoor element)
		{
			var door = PlanPresenter.Cache.Get<GKDoor>(element.DoorUID);
			AddState(door);
		}
		private void InitializeSKD(IElementZone element)
		{
			var zone = PlanPresenter.Cache.Get<GKSKDZone>(element.ZoneUID);
			AddState(zone);
		}
	}
}