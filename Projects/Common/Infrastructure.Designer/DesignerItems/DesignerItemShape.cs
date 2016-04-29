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
			if (Element.IsExactly<ElementPolygon>() || Element.IsExactly<ElementPolyline>())
			{
				Title = Element.PresentationName;
				Group = LayerGroupService.ElementAlias;
				ClassName = Element.IsExactly<ElementPolygon>() ? Resources.Language.DesignerItems.DesignerShapes.Polygon : Resources.Language.DesignerItems.DesignerShapes.Line;
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