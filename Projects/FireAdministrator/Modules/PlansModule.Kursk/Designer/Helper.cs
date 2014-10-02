using System;
using System.Linq;
using System.Windows.Media;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;

namespace PlansModule.Kursk.Designer
{
	internal static class Helper
	{
		public static void SetXDevice(ElementRectangleTank element, GKDevice device)
		{
			element.DeviceUID = device == null ? Guid.Empty : device.UID;
			element.BackgroundColor = GetTankColor(device);
		}
		public static void SetXDevice(ElementRectangleTank element)
		{
			GKDevice xdevice = GetXDevice(element);
			SetXDevice(element, xdevice);
		}
		public static GKDevice GetXDevice(ElementRectangleTank element)
		{
			return element.DeviceUID == Guid.Empty ? null : GKManager.DeviceConfiguration.Devices.Where(item => item.DriverType == GKDriverType.RSR2_Bush && item.UID == element.DeviceUID).FirstOrDefault();
		}
		public static Color GetTankColor(GKDevice xdevice)
		{
			Color color = Colors.Black;
			if (xdevice != null)
				color = Colors.LightCyan;
			return color;
		}
		public static string GetTankTitle(ElementRectangleTank element)
		{
			var device = GetXDevice(element);
			return device == null ? "Бак" : "Бак " + device.DottedAddress;
		}
	}
}