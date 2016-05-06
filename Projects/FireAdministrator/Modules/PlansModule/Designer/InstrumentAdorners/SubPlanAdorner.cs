using StrazhAPI.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Designer;
using Infrastructure.Designer.InstrumentAdorners;
using PlansModule.ViewModels;

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