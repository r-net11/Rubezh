using FiresecAPI.Models;
using GKModule.Plans.Designer;
using GKModule.Plans.ViewModels;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;
using GKModule.ViewModels;

namespace GKModule.Plans.InstrumentAdorners
{
	public class XDirectionAdorner : BaseRectangleAdorner
	{
		private DirectionsViewModel _directionsViewModel;
		public XDirectionAdorner(CommonDesignerCanvas designerCanvas, DirectionsViewModel directionsViewModel)
			: base(designerCanvas)
		{
			_directionsViewModel = directionsViewModel;
		}

		protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			var element = new ElementXDirection();
			var propertiesViewModel = new XDirectionPropertiesViewModel(element, _directionsViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			Helper.SetXDirection(element);
			return element;
		}
	}
}