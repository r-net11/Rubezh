using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using FiresecAPI.Models;
using System.Windows;
using DeviceControls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using XFiresecAPI;

namespace DeviceControls
{
	public static class DevicePictureCache
	{
		private const int Size = 100;
		private static Dictionary<Guid, ImageSource> _imageSources = new Dictionary<Guid, ImageSource>();

		public static void LoadCache()
		{
			_imageSources.Clear();
			RegisterImageSource(null);
			FiresecManager.DeviceLibraryConfiguration.Devices.ForEach(item => RegisterImageSource(item));
		}
		private static void RegisterImageSource(LibraryDevice libraryDevice)
		{
			var frameworkElement = DeviceControl.GetDefaultPicture(libraryDevice);
			frameworkElement.Width = Size;
			frameworkElement.Height = Size;
			frameworkElement.Measure(new Size(frameworkElement.Width, frameworkElement.Height));
			frameworkElement.Arrange(new Rect(new Size(frameworkElement.Width, frameworkElement.Height)));
			var imageSource = new RenderTargetBitmap(Size, Size, 96, 96, PixelFormats.Pbgra32);
			imageSource.Render(frameworkElement);
			//RenderOptions.SetCachingHint(imageSource, CachingHint.Cache);
			imageSource.Freeze();
			_imageSources.Add(libraryDevice == null ? Guid.Empty : libraryDevice.DriverId, imageSource);
		}
		public static ImageSource GetImageSource(Device device)
		{
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetImageSource(driverUID);
		}
		public static ImageSource GetImageSource(XDevice device)
		{
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetImageSource(driverUID);
		}
		public static ImageSource GetImageSource(Guid driverUID)
		{
			if (!_imageSources.ContainsKey(driverUID))
			{
				var libraryDevice = FiresecManager.DeviceLibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == driverUID);
				if (libraryDevice == null)
				{
					if (!_imageSources.ContainsKey(Guid.Empty))
						RegisterImageSource(null);
					return _imageSources[Guid.Empty];
				}
				else
					RegisterImageSource(libraryDevice);
			}
			return _imageSources[driverUID];
		}
	}
}
