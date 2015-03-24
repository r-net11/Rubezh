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
	public class MPTRectangleAdorner : BaseRectangleAdorner
	{
		private MPTsViewModel _mptsViewModel;
		public MPTRectangleAdorner(CommonDesignerCanvas designerCanvas, MPTsViewModel mptsViewModel)
			: base(designerCanvas)
		{
			_mptsViewModel = mptsViewModel;
		}

		protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleGKMPT();
			var propertiesViewModel = new MPTPropertiesViewModel(element, _mptsViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			GKPlanExtension.Instance.SetItem<GKMPT>(element);
			return element;
		}
	}
}