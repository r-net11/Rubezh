using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;

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

		protected override ElementBaseRectangle CreateElement()
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