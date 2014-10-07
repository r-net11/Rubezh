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
			Plan.ElementRectangleGKZones.ForEach(item => Initialize(item));
			Plan.ElementPolygonGKZones.ForEach(item => Initialize(item));
			Plan.ElementRectangleGKGuardZones.ForEach(item => InitializeGuard(item));
			Plan.ElementPolygonGKGuardZones.ForEach(item => InitializeGuard(item));
			Plan.ElementRectangleGKDirections.ForEach(item => Initialize(item));
			Plan.ElementPolygonGKDirections.ForEach(item => Initialize(item));
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
		private void Initialize(IElementZone element)
		{
			var zone = PlanPresenter.Cache.Get<GKZone>(element.ZoneUID);
			AddState(zone);
		}
		private void InitializeGuard(IElementZone element)
		{
			var zone = PlanPresenter.Cache.Get<GKGuardZone>(element.ZoneUID);
			AddState(zone);
		}
		private void Initialize(IElementDirection element)
		{
			var direction = PlanPresenter.Cache.Get<GKDirection>(element.DirectionUID);
			AddState(direction);
		}
	}
}