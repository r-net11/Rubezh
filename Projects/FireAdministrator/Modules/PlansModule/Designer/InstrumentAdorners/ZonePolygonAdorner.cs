using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Elements;
using PlansModule.Designer;
using PlansModule.ViewModels;
using PlansModule.Designer.Designer;

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
			DialogService.ShowModalWindow(propertiesViewModel);
			Helper.SetZone(element);
			return element;
		}
	}
}