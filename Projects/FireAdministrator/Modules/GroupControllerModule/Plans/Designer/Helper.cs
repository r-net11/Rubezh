using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using FiresecAPI.Models;
using FiresecClient;

namespace GKModule.Plans.Designer
{
	internal static class Helper
	{
		public static XDevice GetXDevice(ElementXDevice element)
		{
			return XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == (element.XDeviceUID));
		}
		public static string GetXDeviceTitle(ElementXDevice element)
		{
			var device = GetXDevice(element);
			return device == null ? "Неизвестное устройство" : device.DottedAddress + " " + device.Driver.ShortName;
		}
		public static XDevice SetXDevice(ElementXDevice element)
		{
			XDevice device = GetXDevice(element);
			if (device != null)
				device.PlanElementUIDs.Add(element.UID);
			return device;
		}
		public static void SetXDevice(ElementXDevice element, XDevice device)
		{
			ResetXDevice(element);
			element.XDeviceUID = device.UID;
			device.PlanElementUIDs.Add(element.UID);
		}
		public static void ResetXDevice(ElementXDevice element)
		{
			XDevice device = GetXDevice(element);
			if (device != null)
				device.PlanElementUIDs.Remove(element.UID);
		}
	}
}