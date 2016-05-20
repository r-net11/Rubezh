using Infrastructure.Common.Windows;
using Infrastructure.Plans.ElementProperties.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

namespace Infrastructure.Plans.InstrumentAdorners
{
	public class TextBlockAdorner : RectangleAdorner
	{
		public TextBlockAdorner(BaseDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override ElementBaseRectangle CreateElement(double left, double top)
		{
			var element = new ElementTextBlock() { Left = left, Top = top };
			var propertiesViewModel = new TextBlockPropertiesViewModel(element, DesignerCanvas);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}