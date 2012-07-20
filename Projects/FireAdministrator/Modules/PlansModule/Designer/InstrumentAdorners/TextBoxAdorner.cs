using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using PlansModule.Designer;
using PlansModule.ViewModels;

namespace PlansModule.InstrumentAdorners
{
	public class TextBoxAdorner : RectangleAdorner
	{
		public TextBoxAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			var element = new ElementTextBlock();
			var propertiesViewModel = new TextBlockPropertiesViewModel(element);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}