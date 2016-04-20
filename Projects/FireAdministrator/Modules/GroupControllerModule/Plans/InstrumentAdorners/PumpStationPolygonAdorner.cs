using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;
using Infrastructure.Plans;
using System.Windows.Shapes;

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
