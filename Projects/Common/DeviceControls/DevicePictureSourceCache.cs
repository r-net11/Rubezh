using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrustructure.Plans.Designer;
using XFiresecAPI;

namespace DeviceControls
{
	public static class DevicePictureSourceCache
	{
		private static Dictionary<Guid, ImageSource> _imageSources = new Dictionary<Guid, ImageSource>();
		private static Dictionary<Guid, Brush> _brushes = new Dictionary<Guid, Brush>();

		public static void LoadCache()
		{
			_imageSources.Clear();
			RegisterImageSource(null);
			FiresecManager.DeviceLibraryConfiguration.Devices.ForEach(item => RegisterImageSource(item));
			_brushes.Clear();
			RegisterBrush(null);
			FiresecManager.DeviceLibraryConfiguration.Devices.ForEach(item => RegisterBrush(item));
		}

		private static void RegisterImageSource(LibraryDevice libraryDevice)
		{
			var frameworkElement = DeviceControl.GetDefaultPicture(libraryDevice);
			frameworkElement.SnapsToDevicePixels = false;
			frameworkElement.Width = DesignerItem.DefaultPointSize;
			frameworkElement.Height = DesignerItem.DefaultPointSize;
			frameworkElement.Arrange(new Rect(new Size(frameworkElement.Width, frameworkElement.Height)));
			var imageSource = new RenderTargetBitmap(DesignerItem.DefaultPointSize, DesignerItem.DefaultPointSize, EnvironmentParameters.DpiX, EnvironmentParameters.DpiY, PixelFormats.Pbgra32);
			imageSource.Render(frameworkElement);
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

		private static void RegisterBrush(LibraryDevice libraryDevice)
		{
			var imageSource = GetImageSource(libraryDevice == null ? Guid.Empty : libraryDevice.DriverId);
			var brush = new ImageBrush(imageSource);
			brush.Freeze();
			_brushes.Add(libraryDevice == null ? Guid.Empty : libraryDevice.DriverId, brush);
		}
		public static Brush GetBrush(Device device)
		{
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetBrush(driverUID);
		}
		public static Brush GetBrush(XDevice device)
		{
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetBrush(driverUID);
		}
		public static Brush GetBrush(Guid driverUID)
		{
			if (!_brushes.ContainsKey(driverUID))
			{
				var libraryDevice = FiresecManager.DeviceLibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == driverUID);
				if (libraryDevice == null)
				{
					if (!_brushes.ContainsKey(Guid.Empty))
						RegisterBrush(null);
					return _brushes[Guid.Empty];
				}
				else
					RegisterBrush(libraryDevice);
			}
			return _brushes[driverUID];
		}
	}
}
