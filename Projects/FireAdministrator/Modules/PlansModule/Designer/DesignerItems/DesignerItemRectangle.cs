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
using Infrustructure.Plans.Services;
using PlansModule.Designer.Designer;

namespace PlansModule.Designer.DesignerItems
{
	public class DesignerItemRectangle : DesignerItemBase
	{
		public DesignerItemRectangle(ElementBase element)
			: base(element)
		{
			ResizeChrome = new ResizeChromeRectangle(this);
			if (Element is ElementRectangle)
			{
				Title = "Прямоугольник";
				Group = LayerGroupService.ElementAlias;
			}
			else if (Element is ElementEllipse)
			{
				Title = "Эллипс";
				Group = LayerGroupService.ElementAlias;
			}
			else if (Element is ElementTextBlock)
			{
				Title = "Надпись";
				Group = LayerGroupService.ElementAlias;
			}
			else if (Element is ElementRectangleZone)
			{
				Title = Helper.GetZoneTitle((IElementZone)Element);
				Group = LayerGroupService.ZoneAlias;
			}
			else if (Element is ElementSubPlan)
			{
				Title = Helper.GetSubPlanTitle((ElementSubPlan)Element);
				Group = LayerGroupService.SubPlanAlias;
			}
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
		public override void UpdateElementProperties()
		{
			if (Element is ElementRectangleZone)
				Title = Helper.GetZoneTitle((IElementZone)Element);
			else if (Element is ElementSubPlan)
				Title = Helper.GetSubPlanTitle((ElementSubPlan)Element);
			base.UpdateElementProperties();
		}
	}
}
