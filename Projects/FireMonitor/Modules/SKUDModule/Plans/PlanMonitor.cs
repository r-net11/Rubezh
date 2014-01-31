using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrustructure.Plans.Elements;
using XFiresecAPI;
using SKDModule.Plans.Designer;

namespace SKDModule.Plans
{
	internal class PlanMonitor
	{
		Plan Plan;
		Action CallBack;
		List<SKDDeviceState> DeviceStates;
		List<SKDZoneState> ZoneStates;

		public PlanMonitor(Plan plan, Action callBack)
		{
			Plan = plan;
			CallBack = callBack;
			DeviceStates = new List<SKDDeviceState>();
			ZoneStates = new List<SKDZoneState>();
			Initialize();
		}
		private void Initialize()
		{
			Plan.ElementSKDDevices.ForEach(item => Initialize(item));
			Plan.ElementRectangleSKDZones.ForEach(item => Initialize(item));
			Plan.ElementPolygonSKDZones.ForEach(item => Initialize(item));
		}
		private void Initialize(ElementSKDDevice element)
		{
			var device = Helper.GetSKDDevice(element);
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
				var zone = Helper.GetSKDZone(element);
				if (zone != null)
				{
					ZoneStates.Add(zone.State);
					zone.State.StateChanged += CallBack;
				}
			}
		}

		public XStateClass GetState()
		{
			var result = XStateClass.No;
			foreach (var deviceState in DeviceStates)
			{
				var stateClass = deviceState.StateClass;
				if (stateClass < result)
					result = stateClass;
			}
			foreach (var zoneState in ZoneStates)
			{
				if (zoneState.StateClass < result)
					result = zoneState.StateClass;
			}
			return result;
		}
	}
}