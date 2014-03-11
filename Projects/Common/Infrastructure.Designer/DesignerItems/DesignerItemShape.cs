using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Designer.Adorners;
using Infrastructure.Designer.ElementProperties.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Services;

namespace Infrastructure.Designer.DesignerItems
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
			if (Element.IsExactly<ElementPolygon>())
				return new PolygonPropertiesViewModel(Element as ElementPolygon);
			if (Element.IsExactly<ElementPolyline>())
				return new PolylinePropertiesViewModel(Element as ElementPolyline);
			return base.CreatePropertiesViewModel();
		}
	}
}
