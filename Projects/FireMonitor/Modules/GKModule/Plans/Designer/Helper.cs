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
			XManager.Zones.ForEach(item => _xzoneMap.Add(item.UID, item));
			_xdeviceMap = new Dictionary<Guid, XDevice>();
			XManager.Devices.ForEach(item => _xdeviceMap.Add(item.UID, item));
			_xdirectionMap = new Dictionary<Guid, XDirection>();
			XManager.Directions.ForEach(item => _xdirectionMap.Add(item.UID, item));
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