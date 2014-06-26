using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrustructure.Plans.Elements;

namespace SKDModule.Plans.Designer
{
	internal static class Helper
	{
		static Dictionary<Guid, SKDDevice> _deviceMap;
		static Dictionary<Guid, SKDZone> _zoneMap;
		static Dictionary<Guid, Door> _doorMap;

		public static void BuildMap()
		{
			_deviceMap = SKDManager.Devices.GroupBy(item => item.UID).ToDictionary(group => group.Key, group => group.First());
			_zoneMap = SKDManager.Zones.GroupBy(item => item.UID).ToDictionary(group => group.Key, group => group.First());
			_doorMap = SKDManager.SKDConfiguration.Doors.GroupBy(item => item.UID).ToDictionary(group => group.Key, group => group.First());
		}

		public static SKDDevice GetSKDDevice(ElementSKDDevice element)
		{
			return element.DeviceUID != Guid.Empty && _deviceMap.ContainsKey(element.DeviceUID) ? _deviceMap[element.DeviceUID] : null;
		}

		public static Door GetDoor(ElementDoor element)
		{
			return element.DoorUID != Guid.Empty && _doorMap.ContainsKey(element.DoorUID) ? _doorMap[element.DoorUID] : null;
		}

		public static SKDZone GetSKDZone(IElementZone element)
		{
			return GetSKDZone(element.ZoneUID);
		}
		public static SKDZone GetSKDZone(Guid zoneUID)
		{
			return zoneUID != Guid.Empty && _zoneMap.ContainsKey(zoneUID) ? _zoneMap[zoneUID] : null;
		}

		public static MenuItem BuildMenuItem(string header, string imageSourceUri, ICommand command)
		{
			var menuItem = new MenuItem();

			Image image = new Image();
			image.Width = 16;
			image.VerticalAlignment = VerticalAlignment.Center;
			BitmapImage sourceImage = new BitmapImage();
			sourceImage.BeginInit();
			sourceImage.UriSource = new Uri(imageSourceUri);
			sourceImage.EndInit();
			image.Source = sourceImage;

			menuItem.Icon = image;
			menuItem.Header = header;
			menuItem.Command = command;

			return menuItem;
		}
	}
}