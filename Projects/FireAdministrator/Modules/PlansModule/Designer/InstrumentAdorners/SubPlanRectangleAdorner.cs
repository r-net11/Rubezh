using RubezhAPI.Models;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Designer;
using Infrastructure.Designer.InstrumentAdorners;
using PlansModule.ViewModels;
using RubezhAPI.Plans.Elements;

namespace PlansModule.InstrumentAdorners
{
	public class SubPlanRectangleAdorner : RectangleAdorner
	{
		public SubPlanRectangleAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleSubPlan();
			var propertiesViewModel = new SubPlanPropertiesViewModel(element);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}