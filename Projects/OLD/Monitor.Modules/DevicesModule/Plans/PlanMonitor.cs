using System;
using System.Collections.Generic;
using DevicesModule.Plans.Designer;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;
using Infrastructure.Plans.Elements;
using Infrastructure.Plans.Presenter;

namespace DevicesModule.Plans
{
	internal class PlanMonitor : BaseMonitor<Plan>
	{
		private List<DeviceState> _deviceStates;
		private List<ZoneState> _zoneStates;

		public PlanMonitor(Plan plan, Action callBack)
			: base(plan, callBack)
		{
			_deviceStates = new List<DeviceState>();
			_zoneStates = new List<ZoneState>();
			Initialize();
		}
		private void Initialize()
		{
			Plan.ElementDevices.ForEach(item => Initialize(item));
			Plan.ElementRectangleZones.ForEach(item => Initialize(item));
			Plan.ElementPolygonZones.ForEach(item => Initialize(item));
		}
		private void Initialize(ElementDevice element)
		{
			var device = Helper.GetDevice(element);
			if (device != null)
			{
				_deviceStates.Add(device.DeviceState);
				device.DeviceState.StateChanged += CallBack;
			}
		}
		private void Initialize(IElementZone element)
		{
			if (element.ZoneUID != Guid.Empty)
			{
				var zone = Helper.GetZone(element);
				if (zone != null)
				{
					_zoneStates.Add(zone.ZoneState);
					zone.ZoneState.StateChanged += CallBack;
				}
			}
		}

		public XStateClass GetState()
		{
			var result = StateType.No;
			foreach (var state in _deviceStates)
			{
				if (state.StateType < result)
					result = state.StateType;
			}
			foreach (var state in _zoneStates)
			{
				if (state.StateType < result)
					result = state.StateType;
			}
			return GKStatesHelper.StateTypeToXStateClass(result);
		}
	}
}
