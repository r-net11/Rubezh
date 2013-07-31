using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecClient;

namespace DevicesModule.ViewModels
{
	public static class FS2AuParametersHelper
	{
		public static void BeginGetAuParameters(List<Device> devices)
		{
			var device = devices.FirstOrDefault();
			var result = FiresecManager.FS2ClientContract.GetConfigurationParameters(device.UID);
			if (result != null && !result.HasError && result.Result != null)
			{
				foreach (var resultProperty in result.Result)
				{
					var property = device.DeviceAUProperties.FirstOrDefault(x => x.Name == resultProperty.Name);
					if (property == null)
					{
						property = new Property()
						{
							Name = resultProperty.Name
						};
						device.DeviceAUProperties.Add(property);
					}
					property.Value = resultProperty.Value;
				}
				device.OnAUParametersChanged();
			}
		}

		public static void BeginSetAuParameters(List<Device> devices)
		{

		}
	}
}