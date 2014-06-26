using System;
using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecAPI.Models;
using GKModule.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Presenter;

namespace GKModule.Plans
{
	internal class PlanMonitor : BaseMonitor<Plan>
	{
		List<XState> DeviceStates;
		List<XState> ZoneStates;
		List<XState> GuardZoneStates;
		List<XState> DirectionStates;

		public PlanMonitor(Plan plan, Action callBack)
			: base(plan, callBack)
		{
			DeviceStates = new List<XState>();
			ZoneStates = new List<XState>();
			GuardZoneStates = new List<XState>();
			DirectionStates = new List<XState>();
			Initialize();
		}
		private void Initialize()
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
			var device = Helper.GetXDevice(element);
			if (device != null)
			{
				DeviceStates.Add(device.State);
				device.State.StateChanged += CallBack;
			}
		}
		private void Initialize(IElementZone element)
		{
			if (element.ZoneUID != Guid.Empty)
			{
				var zone = Helper.GetXZone(element);
				if (zone != null)
				{
					ZoneStates.Add(zone.State);
					zone.State.StateChanged += CallBack;
				}
			}
		}
		private void InitializeGuard(IElementZone element)
		{
			if (element.ZoneUID != Guid.Empty)
			{
				var zone = Helper.GetXGuardZone(element);
				if (zone != null)
				{
					ZoneStates.Add(zone.State);
					zone.State.StateChanged += CallBack;
				}
			}
		}
		private void Initialize(IElementDirection element)
		{
			if (element.DirectionUID != Guid.Empty)
			{
				var direction = Helper.GetXDirection(element);
				if (direction != null)
				{
					DirectionStates.Add(direction.State);
					direction.State.StateChanged += CallBack;
				}
			}
		}

		public XStateClass GetState()
		{
			var result = XStateClass.No;
			foreach (var deviceState in DeviceStates)
			{
				var stateClass = deviceState.StateClass;
				if (deviceState.Device.DriverType == XDriverType.AM1_T && stateClass == XStateClass.Fire2)
				{
					stateClass = XStateClass.Info;
				}
				if (stateClass < result)
					result = stateClass;
			}
			foreach (var zoneState in ZoneStates)
			{
				if (zoneState.StateClass < result)
					result = zoneState.StateClass;
			}
			foreach (var guardZoneState in GuardZoneStates)
			{
				if (guardZoneState.StateClass < result)
					result = guardZoneState.StateClass;
			}
			foreach (var directionState in DirectionStates)
			{
				if (directionState.StateClass < result)
					result = directionState.StateClass;
			}
			return result;
		}
	}
}