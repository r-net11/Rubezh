using Infrastructure.Plans.Presenter;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System;

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
			Plan.ElementRectangleGKSKDZones.ForEach(item => InitializeSKD(item));
			Plan.ElementPolygonGKSKDZones.ForEach(item => InitializeSKD(item));
			Plan.ElementRectangleGKDelays.ForEach(item => Initialize(item));
			Plan.ElementPolygonGKDelays.ForEach(item => Initialize(item));
			Plan.ElementRectangleGKDirections.ForEach(item => Initialize(item));
			Plan.ElementPolygonGKDirections.ForEach(item => Initialize(item));
			Plan.ElementRectangleGKMPTs.ForEach(item => Initialize(item));
			Plan.ElementPolygonGKMPTs.ForEach(item => Initialize(item));
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
		private void InitializeSKD(IElementZone element)
		{
			var zone = PlanPresenter.Cache.Get<GKSKDZone>(element.ZoneUID);
			AddState(zone);
		}
		private void Initialize(IElementDelay element)
		{
			var delay = PlanPresenter.Cache.Get<GKDelay>(element.DelayUID);
			AddState(delay);
		}
		private void Initialize(IElementDirection element)
		{
			var direction = PlanPresenter.Cache.Get<GKDirection>(element.DirectionUID);
			AddState(direction);
		}
		private void Initialize(IElementMPT element)
		{
			var mpt = PlanPresenter.Cache.Get<GKMPT>(element.MPTUID);
			AddState(mpt);
		}
	}
}