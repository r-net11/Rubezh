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
	public class PumpStationPolygonAdorner : BasePolygonAdorner
	{
		private PumpStationsViewModel _pumpStationsViewModel = null;
		public PumpStationPolygonAdorner(CommonDesignerCanvas designerCanvas, PumpStationsViewModel pumpStationsViewModel)
			: base(designerCanvas)
		{
			_pumpStationsViewModel = pumpStationsViewModel;
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
			var element = new ElementPolygonGKPumpStation();
			var propertiesViewModel = new PumpStationPropertiesViewModel(element);
			if (DialogService.ShowModalWindow(propertiesViewModel))
			{
				_pumpStationsViewModel.UpdatePumpStations(element.PumpStationUID);
				return element;
			}
			return null;
		}
	}
}
