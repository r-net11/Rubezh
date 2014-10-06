using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;
using PlansModule.Kursk.Designer;
using PlansModule.Kursk.ViewModels;

namespace PlansModule.Kursk.InstrumentAdorners
{
	public class TankRectangleAdorner : BaseRectangleAdorner
	{
		public TankRectangleAdorner(CommonDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleTank();
			var propertiesViewModel = new TankPropertiesViewModel(element);
			DialogService.ShowModalWindow(propertiesViewModel);
			Helper.SetGKDevice(element);
			return element;
		}
	}
}