using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Elements;
using System.Windows;
using System.Windows.Controls;
using PlansModule.Designer.Adorners;

namespace PlansModule.Designer.DesignerItems
{
	public class DesignerItemPoint : DesignerItemBase
	{
	//frameworkElement = DeviceControl.GetDefaultPicture(elementDevice.Device.Driver.UID);
		public DesignerItemPoint(ElementBase element)
			: base(element)
		{
			ResizeChrome = new ResizeChromePoint(this);
		}

		public override void SetLocation()
		{
			var rect = Element.GetRectangle();
			Canvas.SetLeft(this, rect.Left - DesignerCanvas.PointZoom / 2);
			Canvas.SetTop(this, rect.Top - DesignerCanvas.PointZoom / 2);
			ItemWidth = rect.Width + DesignerCanvas.PointZoom;
			ItemHeight = rect.Height + DesignerCanvas.PointZoom;
		}

		public override void UpdateZoomPoint()
		{
			base.UpdateZoomPoint();
			SetLocation();
		}
	}
}
