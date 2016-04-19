using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GKModule.Plans.InstrumentAdorners
{
	public class ZonePolygonAdorner : BasePolygonAdorner
	{
		ZonesViewModel _zonesViewModel;

		public ZonePolygonAdorner(CommonDesignerCanvas designerCanvas, ZonesViewModel zonesViewModel)
			: base(designerCanvas)
		{
			_zonesViewModel = zonesViewModel;
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
			var element = new ElementPolygonGKZone();
			var propertiesViewModel = new ZonePropertiesViewModel(element, _zonesViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			GKPlanExtension.Instance.SetItem<GKZone>(element);
			return element;
		}
	}
}