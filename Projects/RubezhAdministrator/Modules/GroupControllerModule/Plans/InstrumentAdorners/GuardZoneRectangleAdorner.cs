using GKModule.Plans.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

namespace GKModule.Plans.InstrumentAdorners
{
	public class GuardZoneRectangleAdorner : BaseRectangleAdorner
	{
		public GuardZoneRectangleAdorner(CommonDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}
		protected override ElementBaseRectangle CreateElement(double left, double top)
		{
			var element = new ElementRectangleGKGuardZone() { Left = left, Top = top };
			var propertiesViewModel = new GuardZonePropertiesViewModel(element, DesignerCanvas);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}