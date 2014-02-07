using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DeviceControls.Device;
using DeviceControls.SKDDevice;
using DeviceControls.XDevice;
using Infrustructure.Plans.Devices;

namespace DeviceControls
{
	public static class PictureCacheSource
	{
		public static FrameworkElement EmptyPicture { get; private set; }
		public static Brush EmptyBrush { get; private set; }
		public static DevicePicture DevicePicture { get; private set; }
		public static XDevicePicture XDevicePicture { get; private set; }
		public static SKDDevicePicture SKDDevicePicture { get; private set; }

		static PictureCacheSource()
		{
			EmptyPicture = new TextBlock()
			{
				Text = "?",
				Background = Brushes.Transparent,
				SnapsToDevicePixels = false
			};
			EmptyBrush = new VisualBrush(EmptyPicture);
			DevicePicture = new DevicePicture();
			XDevicePicture = new XDevicePicture();
			SKDDevicePicture = new SKDDevicePicture();
		}

		public static Brush CreateDynamicBrush<TLibraryFrame>(List<TLibraryFrame> frames)
			where TLibraryFrame : ILibraryFrame
		{
			var visualBrush = new VisualBrush();
			visualBrush.Visual = new FramesControl(frames.Cast<ILibraryFrame>());
			return visualBrush;
		}
	}
}