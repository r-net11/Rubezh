using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Elements;
using System.Windows;
using System.Windows.Controls;
using PlansModule.Designer.Adorners;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using PlansModule.ViewModels;

namespace PlansModule.Designer.DesignerItems
{
	public class DesignerItemShape : DesignerItemBase
	{
		public DesignerItemShape(ElementBase element)
			: base(element)
		{
			ResizeChrome = new ResizeChromeShape(this);
		}

		protected override SaveCancelDialogViewModel CreatePropertiesViewModel()
		{
			if (Element is ElementPolygon)
				return new PolygonPropertiesViewModel(Element as ElementPolygon);
			if (Element is ElementPolyline)
				return new PolylinePropertiesViewModel(Element as ElementPolyline);
			if (Element is ElementPolygonZone)
				return new ZonePropertiesViewModel(Element as ElementPolygonZone);
			return base.CreatePropertiesViewModel();
		}
	}
}
