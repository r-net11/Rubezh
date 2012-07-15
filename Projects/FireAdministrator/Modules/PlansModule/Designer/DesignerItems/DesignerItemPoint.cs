using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Elements;
using System.Windows;
using System.Windows.Controls;
using PlansModule.Designer.Adorners;
using FiresecAPI.Models;
using PlansModule.Designer.Designer;

namespace PlansModule.Designer.DesignerItems
{
	public class DesignerItemPoint : DesignerItemBase
	{
		public DesignerItemPoint(ElementBase element)
			: base(element)
		{
			ResizeChrome = new ResizeChromePoint(this);
			if (Element is ElementDevice)
			{
				Title = Helper.GetDeviceTitle((ElementDevice)Element);
				Group = "Device";
			}
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

		public override void UpdateElementProperties()
		{
			if (Element is ElementDevice)
				Title = Helper.GetDeviceTitle((ElementDevice)Element);
			base.UpdateElementProperties();
		}
	}
}
