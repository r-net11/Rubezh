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

namespace DevicesModule.Plans.Designer
{
	internal class PainterCache
	{
		private static Dictionary<Guid, ImageSource> _imageSources;

		public PainterCache()
		{
			_imageSources = new Dictionary<Guid, ImageSource>();
		}

		public void LoadCache()
		{
			_imageSources.Clear();
			FiresecManager.DeviceLibraryConfiguration.Devices.ForEach(item => RegisterImageSource(item));
		}
		private void RegisterImageSource(LibraryDevice libraryDevice)
		{
			if (libraryDevice == null)
				return;
			var frameworkElement = DeviceControl.GetDefaultPicture(libraryDevice);
			frameworkElement.Width = 100;
			frameworkElement.Height = 100;
			frameworkElement.Measure(new Size(frameworkElement.Width, frameworkElement.Height));
			frameworkElement.Arrange(new Rect(new Size(frameworkElement.Width, frameworkElement.Height)));
			var imageSource = new RenderTargetBitmap(100, 100, 96, 96, PixelFormats.Pbgra32);
			imageSource.Render(frameworkElement);
			//RenderOptions.SetCachingHint(imageSource, CachingHint.Cache);
			imageSource.Freeze();
			_imageSources.Add(libraryDevice.DriverId, imageSource);
		}
		public ImageSource GetImageSource(Device device)
		{
			Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			return GetImageSource(driverUID);
		}
		public ImageSource GetImageSource(Guid driverUID)
		{
			if (!_imageSources.ContainsKey(driverUID))
			{
				var libraryDevice = FiresecManager.DeviceLibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == driverUID);
				RegisterImageSource(libraryDevice);
			}
			return _imageSources[driverUID];
		}
	}
}
