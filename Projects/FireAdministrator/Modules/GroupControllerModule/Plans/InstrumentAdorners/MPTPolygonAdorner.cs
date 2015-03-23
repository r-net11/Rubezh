using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI.GK;
using FiresecAPI.Models;
using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.InstrumentAdorners;

namespace GKModule.Plans.InstrumentAdorners
{
	public class MPTPolygonAdorner : BasePolygonAdorner
	{
		private MPTsViewModel _mptsViewModel;
		public MPTPolygonAdorner(CommonDesignerCanvas designerCanvas, MPTsViewModel mptsViewModel)
			: base(designerCanvas)
		{
			_mptsViewModel = mptsViewModel;
		}

		protected override Shape CreateRubberband()
		{
			return new Polygon();
		}
		protected override PointCollection Points
		{
			get { return ((Polygon)Rubberband).Points; }
		}
		protected override ElementBaseShape CreateElement()
		{
			var element = new ElementPolygonGKMPT();
			var propertiesViewModel = new MPTPropertiesViewModel(element, _mptsViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			GKPlanExtension.Instance.SetItem<GKMPT>(element);
			return element;
		}
	}
}