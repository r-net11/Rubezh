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

		public PlanMonitor(Plan plan, Action callBack)
		{
			Plan = plan;
			CallBack = callBack;
			DeviceStates = new List<SKDDeviceState>();
			Initialize();
		}
		private void Initialize()
		{
			Plan.ElementSKDDevices.ForEach(item => Initialize(item));
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

		public XStateClass GetState()
		{
			var result = XStateClass.No;
			foreach (var deviceState in DeviceStates)
			{
				var stateClass = deviceState.StateClass;
				if (stateClass < result)
					result = stateClass;
			}
			return result;
		}
	}
}