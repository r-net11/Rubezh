using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using PlansModule.ViewModels;
using Infrastructure.Designer.InstrumentAdorners;
using Infrastructure.Designer;

namespace PlansModule.InstrumentAdorners
{
	public class SubPlanAdorner : RectangleAdorner
	{
		public SubPlanAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			var element = new ElementSubPlan();
			var propertiesViewModel = new SubPlanPropertiesViewModel(element);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}