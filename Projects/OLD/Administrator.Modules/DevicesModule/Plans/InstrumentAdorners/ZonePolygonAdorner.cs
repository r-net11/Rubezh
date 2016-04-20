using System.Windows.Media;
using System.Windows.Shapes;
using DevicesModule.Plans.Designer;
using DevicesModule.Plans.ViewModels;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.Elements;
using Infrastructure.Plans.InstrumentAdorners;

namespace DevicesModule.Plans.InstrumentAdorners
{
	public class ZonePolygonAdorner : BasePolygonAdorner
	{
		private ZonesViewModel _zonesViewModel;
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
			var element = new ElementPolygonZone();
			var propertiesViewModel = new ZonePropertiesViewModel(element, _zonesViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			Helper.SetZone(element);
			return element;
		}
	}
}