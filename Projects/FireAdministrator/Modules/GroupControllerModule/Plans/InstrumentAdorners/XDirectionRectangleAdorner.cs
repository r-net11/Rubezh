using FiresecAPI.GK;
using FiresecAPI.Models;
using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;
using Infrustructure.Plans.Elements;

namespace GKModule.Plans.InstrumentAdorners
{
	public class XDirectionRectangleAdorner : BaseRectangleAdorner
	{
		private DirectionsViewModel _directionsViewModel;
		public XDirectionRectangleAdorner(CommonDesignerCanvas designerCanvas, DirectionsViewModel directionsViewModel)
			: base(designerCanvas)
		{
			_directionsViewModel = directionsViewModel;
		}

		protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleGKDirection();
			var propertiesViewModel = new DirectionPropertiesViewModel(element, _directionsViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			GKPlanExtension.Instance.SetItem<GKDirection>(element);
			return element;
		}
	}
}