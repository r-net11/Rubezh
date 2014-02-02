using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using Infrustructure.Plans.Elements;
using FiresecAPI.Models;
using FiresecClient;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using FiresecAPI;

namespace SKDModule.Plans.Designer
{
	internal static class Helper
	{
		static Dictionary<Guid, SKDDevice> _deviceMap;
		static Dictionary<Guid, SKDZone> _zoneMap;

		public static void BuildMap()
		{
			_deviceMap = new Dictionary<Guid, SKDDevice>();
			foreach (var device in SKDManager.Devices)
			{
				if (!_deviceMap.ContainsKey(device.UID))
					_deviceMap.Add(device.UID, device);
			}

			_zoneMap = new Dictionary<Guid, SKDZone>();
			foreach (var zone in SKDManager.Zones)
			{
				if (!_zoneMap.ContainsKey(zone.UID))
					_zoneMap.Add(zone.UID, zone);
			}
		}

		public static SKDDevice GetSKDDevice(ElementSKDDevice element)
		{
			return element.DeviceUID != Guid.Empty && _deviceMap.ContainsKey(element.DeviceUID) ? _deviceMap[element.DeviceUID] : null;
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