using System;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using DeviceControls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.Generic;

namespace DevicesModule.Plans.Designer
{
	public class Painter : IPainter
	{
		private static Dictionary<Guid, VisualBrush> _cache = new Dictionary<Guid, VisualBrush>();

		public Visual Draw(ElementBase element)
		{
			ElementDevice elementDevice = (ElementDevice)element;

			//var device = FiresecManager.FiresecConfiguration.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
			//Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
			//var frameworkElement = DeviceControl.GetDefaultPicture(driverUID);
			//return frameworkElement;


			if (!_cache.ContainsKey(elementDevice.DeviceUID))
			{
				var device = FiresecManager.FiresecConfiguration.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
				Guid driverUID = device == null ? Guid.Empty : device.DriverUID;
				var frameworkElement = DeviceControl.GetDefaultPicture(driverUID);
				VisualBrush visualBrush = new VisualBrush();
				visualBrush.Visual = frameworkElement;
				visualBrush.AutoLayoutContent = false;
				visualBrush.Stretch = Stretch.Fill;
				_cache.Add(elementDevice.DeviceUID, visualBrush);
			}

			//DrawingVisual drawingVisual = new DrawingVisual();
			//using (DrawingContext drawingContext = drawingVisual.RenderOpen())
			//    drawingContext.DrawRectangle(_cache[elementDevice.DeviceUID], null, new Rect(new Point(), new Size(10, 10)));
			//return drawingVisual;
			return new Rectangle()
			{
				Fill = _cache[elementDevice.DeviceUID]
			};
		}
	}
}