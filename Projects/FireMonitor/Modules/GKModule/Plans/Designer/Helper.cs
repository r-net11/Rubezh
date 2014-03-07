using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using FiresecAPI.Models;
using FiresecClient;
using Infrustructure.Plans.Elements;
using XFiresecAPI;

namespace GKModule.Plans.Designer
{
	internal static class Helper
	{
		static Dictionary<Guid, XZone> _xzoneMap;
		static Dictionary<Guid, XDevice> _xdeviceMap;
		static Dictionary<Guid, XDirection> _xdirectionMap;
		public static void BuildMap()
		{
			_xzoneMap = new Dictionary<Guid, XZone>();
			foreach (var zone in XManager.Zones)
			{
				if (!_xzoneMap.ContainsKey(zone.BaseUID))
					_xzoneMap.Add(zone.BaseUID, zone);
			}

			_xdeviceMap = new Dictionary<Guid, XDevice>();
			foreach (var device in XManager.Devices)
			{
				if (!_xdeviceMap.ContainsKey(device.BaseUID))
					_xdeviceMap.Add(device.BaseUID, device);
			}

			_xdirectionMap = new Dictionary<Guid, XDirection>();
			foreach (var direction in XManager.Directions)
			{
				if (!_xdirectionMap.ContainsKey(direction.BaseUID))
					_xdirectionMap.Add(direction.BaseUID, direction);
			}
		}

		public static XZone GetXZone(IElementZone element)
		{
			return GetXZone(element.ZoneUID);
		}
		public static XZone GetXZone(Guid xzoneUID)
		{
			return xzoneUID != Guid.Empty && _xzoneMap.ContainsKey(xzoneUID) ? _xzoneMap[xzoneUID] : null;
		}

		public static XDevice GetXDevice(ElementXDevice element)
		{
			return element.XDeviceUID != Guid.Empty && _xdeviceMap.ContainsKey(element.XDeviceUID) ? _xdeviceMap[element.XDeviceUID] : null;
		}
		public static XDirection GetXDirection(IElementDirection element)
		{
			return element.DirectionUID != Guid.Empty && _xdirectionMap.ContainsKey(element.DirectionUID) ? _xdirectionMap[element.DirectionUID] : null;
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