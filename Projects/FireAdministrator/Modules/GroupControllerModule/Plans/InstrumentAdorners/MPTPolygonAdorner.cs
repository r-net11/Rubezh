using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;
using System.Windows.Shapes;

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