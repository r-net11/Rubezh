using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans.Adorners;
using Infrastructure.Plans.ElementProperties.ViewModels;
using Infrastructure.Plans.Services;
using RubezhAPI;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

namespace Infrastructure.Plans.DesignerItems
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
				ClassName = Element.IsExactly<ElementPolygon>() ? "Многоугольник" : "Линия";
			}
		}

		protected override SaveCancelDialogViewModel CreatePropertiesViewModel()
		{
			if (Element.IsExactly<ElementPolygon>())
				return new PolygonPropertiesViewModel(Element as ElementPolygon, DesignerCanvas);
			if (Element.IsExactly<ElementPolyline>())
				return new PolylinePropertiesViewModel(Element as ElementPolyline, DesignerCanvas);
			return base.CreatePropertiesViewModel();
		}
	}
}