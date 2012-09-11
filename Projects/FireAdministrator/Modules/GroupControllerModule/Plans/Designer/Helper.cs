using System.Linq;
using System.Windows.Media;
using FiresecAPI.Models;
using FiresecClient;
using Infrustructure.Plans.Elements;
using XFiresecAPI;

namespace GKModule.Plans.Designer
{
	internal static class Helper
	{
		public static string GetXZoneTitle(IElementZone element)
		{
			XZone xzone = GetXZone(element);
			return xzone == null ? "Несвязанная зона" : xzone.PresentationName;
		}
		public static XZone GetXZone(IElementZone element)
		{
			return element.ZoneNo.HasValue ? XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == element.ZoneNo.Value) : null;
		}
		public static void SetXZone(IElementZone element)
		{
			XZone zone = GetXZone(element);
			element.BackgroundColor = GetXZoneColor(zone);
		}
		public static void SetXZone(IElementZone element, XZone xzone)
		{
			element.ZoneNo = xzone == null ? null : (int?)xzone.No;
			element.BackgroundColor = GetXZoneColor(xzone);
		}
		private static Color GetXZoneColor(XZone zone)
		{
			Color color = Colors.Gray;
			if (zone != null)
				color = Colors.Purple;
			return color;
		}

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