using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GKModule.Plans.InstrumentAdorners
{
	public class DirectionPolygonAdorner : BasePolygonAdorner
	{
		private DirectionsViewModel _directionsViewModel;
		public DirectionPolygonAdorner(CommonDesignerCanvas designerCanvas, DirectionsViewModel directionsViewModel)
			: base(designerCanvas)
		{
			_directionsViewModel = directionsViewModel;
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
			var element = new ElementPolygonGKDirection();
			var propertiesViewModel = new DirectionPropertiesViewModel(element, _directionsViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			GKPlanExtension.Instance.SetItem<GKDirection>(element);
			return element;
		}
	}
}