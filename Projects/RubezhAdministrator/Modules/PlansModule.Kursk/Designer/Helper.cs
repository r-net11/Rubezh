using Common;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using System;
using System.Linq;

namespace PlansModule.Kursk.Designer
{
	internal static class Helper
	{
		public static void SetGKDevice(ElementRectangleTank element, GKDevice device)
		{
			element.DeviceUID = device == null ? Guid.Empty : device.UID;
			element.BackgroundColor = GetTankColor(device);
		}
		public static void SetGKDevice(ElementRectangleTank element)
		{
			GKDevice device = GetGKDevice(element);
			SetGKDevice(element, device);
		}
		public static GKDevice GetGKDevice(ElementRectangleTank element)
		{
			return element.DeviceUID == Guid.Empty ? null : GKManager.DeviceConfiguration.Devices.Where(item => item.DriverType == GKDriverType.RSR2_Bush_Drenazh && item.UID == element.DeviceUID).FirstOrDefault();
		}
		public static Color GetTankColor(GKDevice device)
		{
			Color color = Colors.Black;
			if (device != null)
				color = Colors.LightCyan;
			return color;
		}
		public static string GetTankTitle(ElementRectangleTank element)
		{
			var device = GetGKDevice(element);
			return device == null ? "Бак" : "Бак " + device.DottedAddress;
		}
	}
}