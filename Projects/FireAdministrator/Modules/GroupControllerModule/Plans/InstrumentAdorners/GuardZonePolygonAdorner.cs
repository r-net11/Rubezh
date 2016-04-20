using System.Windows.Media;
using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows.Windows;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Shapes;

namespace GKModule.Plans.InstrumentAdorners
{
	public class GuardZonePolygonAdorner : BasePolygonAdorner
	{
		GuardZonesViewModel _guardZonesViewModel;

		public GuardZonePolygonAdorner(CommonDesignerCanvas designerCanvas, GuardZonesViewModel guardZonesViewModel)
			: base(designerCanvas)
		{
			_guardZonesViewModel = guardZonesViewModel;
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
			var element = new ElementPolygonGKGuardZone();
			var propertiesViewModel = new GuardZonePropertiesViewModel(element, _guardZonesViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			GKPlanExtension.Instance.SetItem<GKGuardZone>(element);
			return element;
		}
	}
}