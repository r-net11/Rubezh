using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using FiresecAPI.Models;
using FiresecClient;

namespace VideoModule.Plans.Designer
{
	internal static class Helper
	{
		static Dictionary<Guid, Camera> _cameraMap;
		public static void BuildMap()
		{
			_cameraMap = new Dictionary<Guid, Camera>();
			foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
			{
				if (!_cameraMap.ContainsKey(camera.UID))
					_cameraMap.Add(camera.UID, camera);
			}
		}

		public static Camera GetCamera(ElementCamera element)
		{
			return element.CameraUID != Guid.Empty && _cameraMap.ContainsKey(element.CameraUID) ? _cameraMap[element.CameraUID] : null;
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