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