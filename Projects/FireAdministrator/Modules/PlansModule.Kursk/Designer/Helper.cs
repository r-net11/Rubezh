using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using XFiresecAPI;
using System.Windows.Media;
using FiresecClient;

namespace PlansModule.Kursk.Designer
{
	internal static class Helper
	{
		public static void SetXDevice(ElementRectangleTank element, XDevice xdevice)
		{
			element.XDeviceUID = xdevice == null ? Guid.Empty : xdevice.UID;
			element.BackgroundColor = GetTankColor(xdevice);
		}
		public static void SetXDevice(ElementRectangleTank element)
		{
			XDevice xdevice = GetXDevice(element);
			SetXDevice(element, xdevice);
		}
		public static XDevice GetXDevice(ElementRectangleTank element)
		{
			return element.XDeviceUID == Guid.Empty ? null : XManager.DeviceConfiguration.Devices.Where(item => item.Driver.DriverType == XDriverType.RSR2_Bush && item.UID == element.XDeviceUID).FirstOrDefault();
		}
		public static Color GetTankColor(XDevice xdevice)
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
