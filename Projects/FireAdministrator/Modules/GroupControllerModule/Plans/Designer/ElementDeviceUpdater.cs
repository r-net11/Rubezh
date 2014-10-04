using System;
using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;

namespace GKModule.Plans.Designer
{
	internal class ElementDeviceUpdater : IDisposable
	{
		private Dictionary<Guid, ElementGKDevice> _map;

		public ElementDeviceUpdater()
		{
			_map = new Dictionary<Guid, ElementGKDevice>();
			BuildMap(FiresecManager.PlansConfiguration.Plans);
		}
		private void BuildMap(List<Plan> plans)
		{
			foreach (var plan in plans)
			{
				foreach (var elementXDevice in plan.ElementGKDevices)
					_map.Add(elementXDevice.UID, elementXDevice);
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
					var elementXDevice = _map[planElementUID];
					if (elementXDevice.DeviceUID == Guid.Empty)
						GKPlanExtension.Instance.SetItem<GKDevice>(elementXDevice, device);
				}
			foreach (var child in device.Children)
				UpdateDeviceBinding(child);
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