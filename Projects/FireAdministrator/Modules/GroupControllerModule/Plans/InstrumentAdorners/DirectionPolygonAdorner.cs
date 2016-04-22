using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
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
			var propertiesViewModel = new DirectionPropertiesViewModel(element);
			if (DialogService.ShowModalWindow(propertiesViewModel))
			{
				_directionsViewModel.UpdateDirections(element.DirectionUID);
				return element;
			}
			return null;
		}
	}
}