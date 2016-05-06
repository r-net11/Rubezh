using GKModule.Plans.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

namespace GKModule.Plans.InstrumentAdorners
{
	public class DirectionRectangleAdorner : BaseRectangleAdorner
	{
		public DirectionRectangleAdorner(CommonDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleGKDirection();
			var propertiesViewModel = new DirectionPropertiesViewModel(element);
			if (DialogService.ShowModalWindow(propertiesViewModel))
				return element;
			return null;
		}
	}
}