using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;
using StrazhAPI.Models;
using StrazhAPI.Plans.Elements;
using StrazhAPI.SKD;
using StrazhModule.Plans.ViewModels;
using StrazhModule.ViewModels;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StrazhModule.Plans.InstrumentAdorners
{
	public class SKDZonePolygonAdorner : BasePolygonAdorner
	{
		private readonly ZonesViewModel _zonesViewModel;
		public SKDZonePolygonAdorner(CommonDesignerCanvas designerCanvas, ZonesViewModel zonesViewModel)
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
			var element = new ElementPolygonSKDZone();
			var propertiesViewModel = new ZonePropertiesViewModel(element, _zonesViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			SKDPlanExtension.Instance.SetItem<SKDZone>(element);
			return element;
		}
	}
}