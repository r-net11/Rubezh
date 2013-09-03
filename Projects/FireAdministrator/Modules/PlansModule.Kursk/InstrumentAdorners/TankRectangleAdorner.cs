using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.InstrumentAdorners;
using PlansModule.Kursk.ViewModels;

namespace PlansModule.Kursk.InstrumentAdorners
{
	public class TankRectangleAdorner : BaseRectangleAdorner
	{
		private TanksViewModel _tanksViewModel;
		public TankRectangleAdorner(CommonDesignerCanvas designerCanvas, TanksViewModel tankViewModel)
			: base(designerCanvas)
		{
			_tanksViewModel = tankViewModel;
		}

		protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleTank();
			var propertiesViewModel = new TankPropertiesViewModel(element, _tanksViewModel);
			DialogService.ShowModalWindow(propertiesViewModel);
			//Helper.SetTank(element);
			return element;
		}
	}
}