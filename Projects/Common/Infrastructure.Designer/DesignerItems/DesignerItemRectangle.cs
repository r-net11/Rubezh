using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Designer.Adorners;
using Infrastructure.Designer.ElementProperties.ViewModels;
using Infrustructure.Plans.Services;
using RubezhAPI;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

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
				ClassName = Element.IsExactly<ElementRectangle>() ? "Прямоугольник" : (Element.IsExactly<ElementEllipse>() ? "Эллипс" : "Надпись");
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
			if (Element.IsExactly<ElementTextBox>())
				return new TextBoxPropertiesViewModel(Element as ElementTextBox);
			return base.CreatePropertiesViewModel();
		}
	}
}