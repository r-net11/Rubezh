using Infrastructure.Common.Windows;
using Infrastructure.Designer;
using Infrastructure.Designer.InstrumentAdorners;
using PlansModule.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

namespace PlansModule.InstrumentAdorners
{
	public class SubPlanRectangleAdorner : RectangleAdorner
	{
		public SubPlanRectangleAdorner(BaseDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override ElementBaseRectangle CreateElement(double left, double top)
		{
			var element = new ElementRectangleSubPlan() { Left = left, Top = top };
			var propertiesViewModel = new SubPlanPropertiesViewModel(element);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}