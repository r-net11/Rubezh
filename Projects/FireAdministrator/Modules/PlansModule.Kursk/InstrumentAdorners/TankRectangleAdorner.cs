using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using PlansModule.Kursk.Designer;
using PlansModule.Kursk.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

namespace PlansModule.Kursk.InstrumentAdorners
{
	public class TankRectangleAdorner : BaseRectangleAdorner
	{
		public TankRectangleAdorner(CommonDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override ElementBaseRectangle CreateElement(double left, double top)
		{
			var element = new ElementRectangleTank() { Left = left, Top = top };
			var propertiesViewModel = new TankPropertiesViewModel(element);
			DialogService.ShowModalWindow(propertiesViewModel);
			Helper.SetGKDevice(element);
			return element;
		}
	}
}