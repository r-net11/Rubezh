using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Services;
using PlansModule.Designer.Adorners;
using PlansModule.ViewModels;

namespace PlansModule.Designer.DesignerItems
{
	public class DesignerItemShape : DesignerItemBase
	{
		public DesignerItemShape(ElementBase element)
			: base(element)
		{
			SetResizeChrome(new ResizeChromeShape(this));
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
		}

		protected override SaveCancelDialogViewModel CreatePropertiesViewModel()
		{
			if (Element is ElementPolygon)
				return new PolygonPropertiesViewModel(Element as ElementPolygon);
			if (Element is ElementPolyline)
				return new PolylinePropertiesViewModel(Element as ElementPolyline);
			return base.CreatePropertiesViewModel();
		}
	}
}
