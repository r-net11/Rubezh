using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Elements;
using PlansModule.Designer;
using PlansModule.ViewModels;

namespace PlansModule.InstrumentAdorners
{
	public class ZonePolygonAdorner : PolygonAdorner
	{
		public ZonePolygonAdorner(DesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
		}

		protected override PointCollection Points
		{
			get { return ((Polygon)Rubberband).Points; }
		}
		protected override ElementBaseShape CreateElement()
		{
			var element = new ElementPolygonZone();
			var propertiesViewModel = new ZonePropertiesViewModel(element);
			return DialogService.ShowModalWindow(propertiesViewModel) ? element : null;
		}
	}
}