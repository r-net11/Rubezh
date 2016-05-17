using GKModule.Plans.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

namespace GKModule.Plans.InstrumentAdorners
{
	public class ZoneRectangleAdorner : BaseRectangleAdorner
	{
		public ZoneRectangleAdorner(CommonDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}
		protected override ElementBaseRectangle CreateElement(double left, double top)
		{
			var element = new ElementRectangleGKZone() { Left = left, Top = top };
			var propertiesViewModel = new ZonePropertiesViewModel(element);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}