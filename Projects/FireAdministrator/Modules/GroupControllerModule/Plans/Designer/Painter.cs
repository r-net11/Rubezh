using System;
using System.Windows;
using DeviceControls;
using FiresecAPI.Models;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using System.Windows.Controls;
using System.Windows.Media;
using FiresecClient;

namespace GKModule.Plans.Designer
{
	public class Painter : IPainter
	{
		#region IPainter Members

		public bool RedrawOnZoom
		{
			get { return false; }
		}
		public void Draw(DrawingContext drawingContext, ElementBase element, Rect rect)
		{
			var device = Helper.GetXDevice((ElementXDevice)element);
			var brush = DevicePictureCache.GetBrush(device);
			drawingContext.DrawGeometry(brush, null, new RectangleGeometry(rect));
		}

		#endregion
	}
}