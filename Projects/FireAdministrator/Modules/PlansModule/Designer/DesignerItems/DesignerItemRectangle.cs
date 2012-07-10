using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Elements;
using System.Windows;
using System.Windows.Controls;
using PlansModule.Designer.Adorners;
using FiresecAPI.Models;
using PlansModule.ViewModels;

namespace PlansModule.Designer.DesignerItems
{
	public class DesignerItemRectangle : DesignerItemBase
	{
		public DesignerItemRectangle(ElementBase element)
			:base (element)
		{
			ResizeChrome = new ResizeChromeRectangle(this);
		}
		protected override Infrastructure.Common.Windows.ViewModels.SaveCancelDialogViewModel CreatePropertiesViewModel()
		{
			if (Element is ElementRectangle)
				return new RectanglePropertiesViewModel(Element as ElementRectangle);
			if (Element is ElementEllipse)
				return new EllipsePropertiesViewModel(Element as ElementEllipse);
			if (Element is ElementTextBlock)
				return new TextBlockPropertiesViewModel(Element as ElementTextBlock);
			if (Element is ElementRectangleZone)
				return new ZonePropertiesViewModel(Element as ElementRectangleZone);
			if (Element is ElementSubPlan)
				return new SubPlanPropertiesViewModel(Element as ElementSubPlan);
			return base.CreatePropertiesViewModel();
		}
	}
}
