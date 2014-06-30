using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrustructure.Plans.Events;
using SKDModule.Events;

namespace SKDModule
{
	public static class ShowOnPlanHelper
	{
		public static void ShowDevice(SKDDevice device, Plan plan)
		{
			var element = plan == null ? null : plan.ElementSKDDevices.FirstOrDefault(item => item.DeviceUID == device.UID);
			if (plan == null || element == null)
				ServiceFactory.OnPublishEvent<SKDDevice, ShowSKDDeviceOnPlanEvent>(device);
			else
				ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
		}
		public static bool CanShowDevice(SKDDevice device)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				if (plan.ElementSKDDevices.Any(x => x.DeviceUID == device.UID))
					return true;
			return false;
		}
	}
}