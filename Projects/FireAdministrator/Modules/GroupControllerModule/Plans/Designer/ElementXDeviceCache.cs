using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecClient;
using XFiresecAPI;

namespace GKModule.Plans.Designer
{
	internal class ElementXDeviceCache : IDisposable
	{
		private Dictionary<Guid, ElementXDevice> _map;

		public ElementXDeviceCache()
		{
			_map = new Dictionary<Guid, ElementXDevice>();
			BuildMap(FiresecManager.PlansConfiguration.Plans);
		}
		private void BuildMap(List<Plan> plans)
		{
			foreach (var plan in plans)
			{
				foreach (var elementXDevice in plan.ElementXDevices)
					_map.Add(elementXDevice.UID, elementXDevice);
				BuildMap(plan.Children);
			}
		}

		public void Update(XDevice xdevice)
		{
			var planElementUIDs = xdevice.PlanElementUIDs;
			xdevice.PlanElementUIDs = new List<Guid>();
			foreach (var planElementUID in planElementUIDs)
				if (_map.ContainsKey(planElementUID))
				{
					var elementXDevice = _map[planElementUID];
					if (elementXDevice.XDeviceUID == Guid.Empty)
						Helper.SetXDevice(elementXDevice, xdevice);
				}
			foreach (var child in xdevice.Children)
				Update(child);
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
