using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.InstrumentAdorners;
using PlansModule.Kursk.ViewModels;
using PlansModule.Kursk.Designer;

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
			Helper.SetXDevice(element);
			return element;
		}
	}
}