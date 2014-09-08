using FiresecAPI.SKD;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.InstrumentAdorners;
using SKDModule.PassCardDesigner.ViewModels;
using SKDModule.ViewModels;

namespace SKDModule.PassCardDesigner.InstrumentAdorners
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