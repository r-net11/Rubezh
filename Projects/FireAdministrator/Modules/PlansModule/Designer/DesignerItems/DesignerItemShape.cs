using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Services;
using PlansModule.Designer.Adorners;
using PlansModule.Designer.Designer;
using PlansModule.ViewModels;

namespace PlansModule.Designer.DesignerItems
{
	public class DesignerItemShape : DesignerItemBase
	{
		public DesignerItemShape(ElementBase element)
			: base(element)
		{
			ResizeChrome = new ResizeChromeShape(this);
			if (Element is ElementPolygon)
			{
				Title = "Многоугольник";
				Group = LayerGroupService.ElementAlias;
			}
			else if (Element is ElementPolyline)
			{
				Title = "Линия";
				Group = LayerGroupService.ElementAlias;
			}
			else if (Element is ElementPolygonZone)
			{
				Title = Helper.GetZoneTitle((IElementZone)Element);
				Group = LayerGroupService.ZoneAlias;
			}
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

		public override void UpdateElementProperties()
		{
			if (Element is ElementPolygonZone)
				Title = Helper.GetZoneTitle((IElementZone)Element);
			base.UpdateElementProperties();
		}
	}
}
