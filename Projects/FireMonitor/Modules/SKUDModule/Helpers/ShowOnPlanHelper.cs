using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure;
using FiresecClient;
using Infrastructure.Events;
using Infrustructure.Plans.Events;

namespace SKDModule
{
	public static class ShowOnPlanHelper
	{
		public static void ShowDevice(SKDDevice device, Plan plan)
		{
			var element = plan == null ? null : plan.ElementSKDDevices.FirstOrDefault(item => item.DeviceUID == device.UID);
			if (plan == null || element == null)
				ShowDevice(device);
			else
				ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
		}
		public static void ShowDevice(SKDDevice device)
		{
			ServiceFactory.Events.GetEvent<ShowSKDDeviceOnPlanEvent>().Publish(device);
		}
		public static bool CanShowDevice(SKDDevice device)
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				if (plan.ElementSKDDevices.Any(x => x.DeviceUID == device.UID))
				{
					return true;
				}
			}
			return false;
		}
	}
}