using Infrastructure.Common.Windows;
using Infrastructure.Designer.ElementProperties.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

namespace Infrastructure.Designer.InstrumentAdorners
{
	public class TextBoxAdorner : RectangleAdorner
	{
		public TextBoxAdorner(BaseDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override ElementBaseRectangle CreateElement(double left, double top)
		{
			var element = new ElementTextBox() { Left = left, Top = top };
			var propertiesViewModel = new TextBoxPropertiesViewModel(element);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}