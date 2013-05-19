using FiresecAPI.Models;
using GKModule.Plans.Designer;
using GKModule.Plans.ViewModels;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;

namespace GKModule.Plans.InstrumentAdorners
{
	public class XDirectionAdorner : BaseRectangleAdorner
	{
		public XDirectionAdorner(CommonDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			var element = new ElementXDirection();
			var propertiesViewModel = new XDirectionPropertiesViewModel(element);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			Helper.SetXDirection(element);
			return element;
		}
	}
}