using System.Windows.Media;
using System.Windows.Shapes;
using RubezhAPI.GK;
using RubezhAPI.Models;
using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.InstrumentAdorners;

namespace GKModule.Plans.InstrumentAdorners
{
	public class PumpStationPolygonAdorner : BasePolygonAdorner
	{
		public PumpStationPolygonAdorner(CommonDesignerCanvas designerCanvas, PumpStationsViewModel pumpStationsViewModel)
			: base(designerCanvas)
		{
			this.pumpStationsViewModel = pumpStationsViewModel;
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
			var propertiesViewModel = new PumpStationPropertiesViewModel(element, this.pumpStationsViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			GKPlanExtension.Instance.SetItem<GKPumpStation>(element);
			return element;
		}

		#region Fields
		private PumpStationsViewModel pumpStationsViewModel = null;

		#endregion
	}
}
