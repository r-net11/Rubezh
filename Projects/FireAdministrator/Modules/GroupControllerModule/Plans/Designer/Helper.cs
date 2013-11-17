using System;
using System.Collections.Generic;
using System.Windows.Media;
using FiresecAPI.Models;
using FiresecClient;
using Infrustructure.Plans.Elements;
using XFiresecAPI;

namespace GKModule.Plans.Designer
{
	internal static class Helper
	{
		private static Dictionary<Guid, XZone> _xzoneMap;
		private static Dictionary<Guid, XDevice> _xdeviceMap;
		private static Dictionary<Guid, XDirection> _xdirectionMap;
		public static void BuildMap()
		{
			BuildXDeviceMap();
			BuildXDirectionMap();
			BuildXZoneMap();
		}
		public static void BuildXDeviceMap()
		{
			_xdeviceMap = new Dictionary<Guid, XDevice>();
			foreach (var xdevice in XManager.Devices)
			{
				if (!_xdeviceMap.ContainsKey(xdevice.UID))
					_xdeviceMap.Add(xdevice.UID, xdevice);
			}
		}
		public static void BuildXZoneMap()
		{
			_xzoneMap = new Dictionary<Guid, XZone>();
			foreach (var xzone in XManager.Zones)
			{
				if (!_xzoneMap.ContainsKey(xzone.UID))
					_xzoneMap.Add(xzone.UID, xzone);
			}
		}
		public static void BuildXDirectionMap()
		{
			_xdirectionMap = new Dictionary<Guid, XDirection>();
			foreach (var xdirection in XManager.Directions)
			{
				if (!_xdirectionMap.ContainsKey(xdirection.UID))
					_xdirectionMap.Add(xdirection.UID, xdirection);
			}
		}
	
		public static string GetXZoneTitle(IElementZone element)
		{
			XZone xzone = GetXZone(element);
			return GetXZoneTitle(xzone);
		}
		public static string GetXZoneTitle(XZone xzone)
		{
			return xzone == null ? "Несвязанная зона" : xzone.PresentationName;
		}
		public static XZone GetXZone(IElementZone element)
		{
			return GetXZone(element.ZoneUID);
		}
		public static XZone GetXZone(Guid xzoneUID)
		{
			return xzoneUID != Guid.Empty && _xzoneMap.ContainsKey(xzoneUID) ? _xzoneMap[xzoneUID] : null;
		}
		public static void SetXZone(IElementZone element)
		{
			XZone zone = GetXZone(element);
			SetXZone(element, zone);
		}
		public static void SetXZone(IElementZone element, XZone xzone)
		{
			ResetXZone(element);
			element.ZoneUID = xzone == null ? Guid.Empty : xzone.UID;
			element.BackgroundColor = GetXZoneColor(xzone);
			if (xzone != null)
				xzone.PlanElementUIDs.Add(element.UID);
		}
		public static void ResetXZone(IElementZone element)
		{
			XZone xzone = GetXZone(element);
			if (xzone != null)
				xzone.PlanElementUIDs.Remove(element.UID);
		}
		public static Color GetXZoneColor(XZone zone)
		{
			Color color = Colors.Black;
			if (zone != null)
				color = Colors.Green;
			return color;
		}

		public static string GetXDirectionTitle(IElementDirection element)
		{
			XDirection xdirection = GetXDirection(element);
			return GetXDirectionTitle(xdirection);
		}
		public static string GetXDirectionTitle(XDirection xdirection)
		{
			return xdirection == null ? "Несвязанное направление" : xdirection.PresentationName;
		}
		public static XDirection GetXDirection(IElementDirection element)
		{
			return GetXDirection(element.DirectionUID);
		}
		public static XDirection GetXDirection(Guid xdirectionUID)
		{
			return xdirectionUID != Guid.Empty && _xdirectionMap.ContainsKey(xdirectionUID) ? _xdirectionMap[xdirectionUID] : null;
		}
		public static void SetXDirection(IElementDirection element)
		{
			XDirection direction = GetXDirection(element);
			SetXDirection(element, direction);
		}
		public static void SetXDirection(IElementDirection element, XDirection xdirection)
		{
			ResetXDirection(element);
			element.DirectionUID = xdirection == null ? Guid.Empty : xdirection.UID;
			element.BackgroundColor = GetXDirectionColor(xdirection);
			if (xdirection != null)
				xdirection.PlanElementUIDs.Add(element.UID);
		}
		public static void ResetXDirection(IElementDirection element)
		{
			XDirection xdirection = GetXDirection(element);
			if (xdirection != null)
				xdirection.PlanElementUIDs.Remove(element.UID);
		}
		public static Color GetXDirectionColor(XDirection direction)
		{
			Color color = Colors.Black;
			if (direction != null)
				color = Colors.LightBlue;
			return color;
		}

		public static XDevice GetXDevice(ElementXDevice element)
		{
			return element.XDeviceUID != Guid.Empty && _xdeviceMap.ContainsKey(element.XDeviceUID) ? _xdeviceMap[element.XDeviceUID] : null;
		}
		public static string GetXDeviceTitle(ElementXDevice element)
		{
			var device = GetXDevice(element);
			return device == null ? "Неизвестное устройство" : device.DottedAddress + " " + device.Driver.ShortName;
		}
		public static string GetXDeviceImageSource(ElementXDevice element)
		{
			var device = GetXDevice(element);
			return device == null ? null : device.Driver.ImageSource;
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
			element.XDeviceUID = device == null ? Guid.Empty : device.UID;
			if (device != null)
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