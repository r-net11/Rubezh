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
	public class SKDZonePolygonAdorner : BasePolygonAdorner
	{
		SKDZonesViewModel _skdZonesViewModel;

		public SKDZonePolygonAdorner(CommonDesignerCanvas designerCanvas, SKDZonesViewModel skdZonesViewModel)
			: base(designerCanvas)
		{
			_skdZonesViewModel = skdZonesViewModel;
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
			var element = new ElementPolygonGKSKDZone();
			var propertiesViewModel = new SKDZonePropertiesViewModel(element, _skdZonesViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			GKPlanExtension.Instance.SetItem<GKSKDZone>(element);
			return element;
		}
	}
}