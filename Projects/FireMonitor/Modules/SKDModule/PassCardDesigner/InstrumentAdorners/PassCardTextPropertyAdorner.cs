using FiresecAPI.SKD;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.InstrumentAdorners;
using SKDModule.PassCardDesigner.ViewModels;

namespace SKDModule.PassCardDesigner.InstrumentAdorners
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