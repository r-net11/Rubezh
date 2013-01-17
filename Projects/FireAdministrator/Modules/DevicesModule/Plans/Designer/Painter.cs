using System.Windows;
using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;

namespace DevicesModule.Plans.Designer
{
	internal class Painter : RectanglePainter
	{
		protected override void InitializeBrushes(ElementBase element, Rect rect)
		{
			base.InitializeBrushes(element, rect);
			SolidColorBrush.Color = Colors.Transparent;
			SolidColorBrush.Freeze();
		}
		protected override void UpdateImageBrush(ElementBase element, Rect rect)
		{
			var device = Helper.GetDevice((ElementDevice)element);
			ImageBrush.ImageSource = DevicePictureCache.GetImageSource(device);
		}

		//public Visual Draw(ElementBase element)
		//{
		//    //var device = Helper.GetDevice((ElementDevice)element);
		//    //Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
		//    //var libraryDevice = FiresecManager.DeviceLibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == driverUID);
		//    //var frameworkElement = DeviceControl.GetDefaultPicture(libraryDevice);
		//    //Brush brush = new VisualBrush()
		//    //{
		//    //    Visual = frameworkElement
		//    //};

		//    //DrawingVisual visual = new DrawingVisual();
		//    //using (DrawingContext dc = visual.RenderOpen())
		//    //{
		//    //    var rect = element.GetRectangle();
		//    //    dc.DrawRectangle(brush, null, rect);
		//    //}
		//    //return visual;

		//    var device = Helper.GetDevice((ElementDevice)element);
		//    var imageSource = DevicePictureCache.GetImageSource(device);
		//    return new Image()
		//    {
		//        Source = imageSource
		//    };
		//}

		//public Visual Draw2(ElementBase element)
		//{

		//    //if (_brush == null)
		//    ////if (ellipses == null)
		//    //{
		//    //    ellipses = new GeometryGroup();
		//    //    ellipses.Children.Add(new EllipseGeometry(new Point(50, 50), 45, 20));
		//    //    ellipses.Children.Add(new EllipseGeometry(new Point(50, 50), 20, 45));
		//    //    ellipses.Freeze();
		//    //    aGeometryDrawing = new GeometryDrawing();
		//    //    aGeometryDrawing.Geometry = ellipses;
		//    //    aGeometryDrawing.Brush = new LinearGradientBrush(Colors.Blue, Color.FromRgb(204, 204, 255), new Point(0, 0), new Point(1, 1));
		//    //    aGeometryDrawing.Pen = new Pen(Brushes.Black, 10);
		//    //    aGeometryDrawing.Freeze();

		//    //    DrawingBrush myDrawingBrush = new DrawingBrush();
		//    //    myDrawingBrush.Drawing = aGeometryDrawing;
		//    //    myDrawingBrush.Freeze();
		//    //    _brush = myDrawingBrush;

		//    //    _fill = new LinearGradientBrush(Colors.Blue, Color.FromRgb(204, 204, 255), new Point(0, 0), new Point(1, 1));
		//    //    _fill.Freeze();
		//    //}
		//    ////DrawingVisual dv = new DrawingVisual();
		//    ////using (DrawingContext dc = dv.RenderOpen())
		//    ////{
		//    ////    //dc.DrawGeometry(_fill, new Pen(Brushes.Black, 10), ellipses);
		//    ////    dc.DrawDrawing(aGeometryDrawing);
		//    ////    dc.Close();
		//    ////}
		//    ////return dv;

		//    ////Path p = new Path();
		//    ////p.Data = ellipses;
		//    ////p.Fill = _fill;
		//    ////p.Stroke = Brushes.Black;
		//    ////p.StrokeThickness = 10;
		//    ////return p;

		//    //Rectangle rect = new Rectangle();
		//    //rect.Fill = _brush;
		//    //return rect;


		//    ////DrawingImage geometryImage = new DrawingImage(aGeometryDrawing);
		//    ////geometryImage.Freeze();
		//    ////Image anImage = new Image();
		//    ////anImage.Source = geometryImage;
		//    ////return anImage;




		//    //DrawingBrush drawingBrush = new DrawingBrush();

		//    //// Set the caching hint option for the brush.
		//    //RenderOptions.SetCachingHint(drawingBrush, CachingHint.Cache);

		//    //// Set the minimum and maximum relative sizes for regenerating the tiled brush. 
		//    //// The tiled brush will be regenerated and re-cached when its size is 
		//    //// 0.5x or 2x of the current cached size.
		//    //RenderOptions.SetCacheInvalidationThresholdMinimum(drawingBrush, 0.5);
		//    //RenderOptions.SetCacheInvalidationThresholdMaximum(drawingBrush, 2.0);

		//    ElementDevice elementDevice = (ElementDevice)element;

		//    //var device = FiresecManager.FiresecConfiguration.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
		//    //Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
		//    //var frameworkElement = DeviceControl.GetDefaultPicture(driverUID);
		//    //return frameworkElement;

		//    if (!_cache.ContainsKey(elementDevice.DeviceUID))
		//    {
		//        var device = FiresecManager.FiresecConfiguration.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
		//        Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
		//        var frameworkElement = DeviceControl.GetDefaultPicture(driverUID);
		//        VisualBrush visualBrush = new VisualBrush();
		//        visualBrush.Visual = frameworkElement;
		//        visualBrush.AutoLayoutContent = false;
		//        visualBrush.Stretch = Stretch.Fill;

		//        //var visual = new DrawingVisual();
		//        //DrawingContext context = visual.RenderOpen();
		//        //context.DrawRectangle(brush, null, new Rect(0, 0, frameworkElement.ActualWidth, frameworkElement.ActualHeight));
		//        //context.Close();
		//        //var bmp = new RenderTargetBitmap(10, 10, 96, 96, PixelFormats.Pbgra32);
		//        //bmp.Render(visual);
		//        //ImageBrush ib = new ImageBrush();
		//        //ib.ImageSource = bmp;

		//        _cache.Add(elementDevice.DeviceUID, visualBrush);
		//    }

		//    //DrawingVisual drawingVisual = new DrawingVisual();
		//    //using (DrawingContext drawingContext = drawingVisual.RenderOpen())
		//    //    drawingContext.DrawRectangle(_cache[elementDevice.DeviceUID], null, new Rect(new Point(), new Size(10, 10)));
		//    //return drawingVisual;
		//    return new Rectangle()
		//    {
		//        Fill = _cache[elementDevice.DeviceUID]
		//    };
		//}

		//private FrameworkElement GetFrameworkElement(ElementBase element)
		//{
		//    ElementDevice elementDevice = (ElementDevice)element;
		//    var device = FiresecManager.FiresecConfiguration.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
		//    Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
		//    return DeviceControl.GetDefaultPicture(driverUID);
		//}
	}
}