using RubezhAPI.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Designer;
using Infrastructure.Designer.InstrumentAdorners;
using PlansModule.ViewModels;

namespace PlansModule.InstrumentAdorners
{
	public class SubPlanRectangleAdorner : RectangleAdorner
	{
		public SubPlanRectangleAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleSubPlan();
			var propertiesViewModel = new SubPlanPropertiesViewModel(element);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}