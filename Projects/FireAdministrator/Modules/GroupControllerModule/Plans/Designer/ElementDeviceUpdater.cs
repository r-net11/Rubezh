﻿using System;
using System.Collections.Generic;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;

namespace GKModule.Plans.Designer
{
	internal class ElementDeviceUpdater : IDisposable
	{
		private Dictionary<Guid, ElementGKDevice> _map;

		public ElementDeviceUpdater()
		{
			_map = new Dictionary<Guid, ElementGKDevice>();
			BuildMap(ClientManager.PlansConfiguration.Plans);
		}
		private void BuildMap(List<Plan> plans)
		{
			foreach (var plan in plans)
			{
				foreach (var elementGKDevice in plan.ElementGKDevices)
					_map.Add(elementGKDevice.UID, elementGKDevice);
				BuildMap(plan.Children);
			}
		}

		public void UpdateDeviceBinding(GKDevice device)
		{
			var planElementUIDs = device.PlanElementUIDs;
			device.PlanElementUIDs = new List<Guid>();
			foreach (var planElementUID in planElementUIDs)
				if (_map.ContainsKey(planElementUID))
				{
					var elementGKDevice = _map[planElementUID];
					if (elementGKDevice.DeviceUID == Guid.Empty)
						GKPlanExtension.Instance.SetItem<GKDevice>(elementGKDevice, device);
				}
			foreach (var child in device.Children)
				UpdateDeviceBinding(child);
		}
		public void ResetDevices(List<GKDevice> devices)
		{
			foreach (var device in devices)
				foreach (var planElementUID in device.PlanElementUIDs)
					if (_map.ContainsKey(planElementUID))
					{
						var elementDevice = _map[planElementUID];
						elementDevice.DeviceUID = Guid.Empty;
					}
		}

		#region IDisposable Members

		public void Dispose()
		{
			_map.Clear();
			_map = null;
		}

		#endregion
	}
}