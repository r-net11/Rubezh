using StrazhAPI;
using StrazhAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Designer.Adorners;
using Infrastructure.Designer.ElementProperties.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Services;

namespace Infrastructure.Designer.DesignerItems
{
	public class DesignerItemRectangle : DesignerItemBase
	{
		public DesignerItemRectangle(ElementBase element)
			: base(element)
		{
			SetResizeChrome(new ResizeChromeRectangle(this));
			if (Element.IsExactly<ElementRectangle>() || Element.IsExactly<ElementEllipse>() || Element.IsExactly<ElementTextBlock>())
			{
				Title = element.PresentationName;
				Group = LayerGroupService.ElementAlias;
                ClassName = Element.IsExactly<ElementRectangle>() ? Resources.Language.DesignerItems.DesignerItemRectangle.Rectangle : (Element.IsExactly<ElementEllipse>() ? 
                                                                    Resources.Language.DesignerItems.DesignerItemRectangle.Ellipse : 
                                                                    Resources.Language.DesignerItems.DesignerItemRectangle.Textblock);
			}
		}

		protected override SaveCancelDialogViewModel CreatePropertiesViewModel()
		{
			if (Element.IsExactly<ElementRectangle>())
				return new RectanglePropertiesViewModel(Element as ElementRectangle);
			if (Element.IsExactly<ElementEllipse>())
				return new EllipsePropertiesViewModel(Element as ElementEllipse);
			if (Element.IsExactly<ElementTextBlock>())
				return new TextBlockPropertiesViewModel(Element as ElementTextBlock);
			return base.CreatePropertiesViewModel();
		}
	}
}