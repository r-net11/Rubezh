using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.InstrumentAdorners;
using SKDModule.Plans.Designer;
using SKDModule.Plans.ViewModels;
using SKDModule.ViewModels;
using FiresecAPI.SKD.PassCardLibrary;
using SKDModule.PassCard.ViewModels;

namespace SKDModule.PassCard.InstrumentAdorners
{
	public class PassCardImagePropertyAdorner : BaseRectangleAdorner
	{
		private ZonesViewModel _zonesViewModel;
		public PassCardImagePropertyAdorner(CommonDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override ElementBaseRectangle CreateElement()
		{
			var element = new ElementPassCardImageProperty();
			var propertiesViewModel = new PassCardImagePropertyViewModel(element);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			return element;
		}
	}
}