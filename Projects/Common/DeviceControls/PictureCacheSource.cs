using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using StrazhAPI.Plans.Devices;

namespace DeviceControls
{
	public static class PictureCacheSource
	{
		public static FrameworkElement EmptyPicture { get; private set; }
		public static Brush EmptyBrush { get; private set; }
		public static SKDDevicePicture SKDDevicePicture { get; private set; }
		public static CameraPicture CameraPicture { get; private set; }
		public static DoorPicture DoorPicture { get; private set; }

		static PictureCacheSource()
		{
			EmptyPicture = new TextBlock()
			{
				Text = "?",
				Background = Brushes.Transparent,
				SnapsToDevicePixels = false
			};
			EmptyBrush = new VisualBrush(EmptyPicture);

			SKDDevicePicture = new SKDDevicePicture();
			CameraPicture = new CameraPicture();
			DoorPicture = new DoorPicture();
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