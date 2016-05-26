using System.Windows.Media;
using System.Windows.Shapes;
using StrazhAPI.Models;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using StrazhAPI.Plans.Elements;
using Infrustructure.Plans.InstrumentAdorners;
using StrazhModule.Plans.Designer;
using StrazhModule.Plans.ViewModels;
using StrazhModule.ViewModels;
using StrazhAPI.SKD;

namespace StrazhModule.Plans.InstrumentAdorners
{
	public class SKDZonePolygonAdorner : BasePolygonAdorner
	{
		private ZonesViewModel _zonesViewModel;
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