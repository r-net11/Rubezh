using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Infrustructure.Plans.Devices;

namespace DeviceControls
{
	public static class PictureCacheSource
	{
		public static FrameworkElement EmptyPicture { get; private set; }
		public static Brush EmptyBrush { get; private set; }
		public static Brush CameraBrush { get; private set; }
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
			CameraBrush = new DrawingBrush()
			{
				Drawing = new GeometryDrawing()
				{
					Pen = new Pen(Brushes.Black, 1),
					Geometry = new CombinedGeometry()
					{
						Geometry1 = new RectangleGeometry(new Rect(0, 0, 15, 10), 1, 1),
						Geometry2 = new PathGeometry()
						{
							Figures = new PathFigureCollection()
							{
								new PathFigure()
								{
									StartPoint = new Point(15,3),
									IsClosed = false,
									Segments = new PathSegmentCollection()
									{
										new LineSegment(new Point(20,1),true),
										new LineSegment(new Point(20,9),true),
										new LineSegment(new Point(15,7),true),
									}
								}
							}
						}
					}
				}
			};
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