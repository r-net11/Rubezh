using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.InstrumentAdorners;
using SKDModule.Plans.Designer;
using SKDModule.Plans.ViewModels;
using FiresecAPI.SKD.PassCardLibrary;
using SKDModule.PassCard.ViewModels;

namespace SKDModule.PassCard.InstrumentAdorners
{
	public class PassCardTextPropertyAdorner : BaseRectangleAdorner
	{
		public PassCardTextPropertyAdorner(CommonDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override ElementBaseRectangle CreateElement()
		{
			var element = new ElementPassCardTextProperty();
			var propertiesViewModel = new PassCardTextPropertyViewModel(element);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			return element;
		}
	}
}