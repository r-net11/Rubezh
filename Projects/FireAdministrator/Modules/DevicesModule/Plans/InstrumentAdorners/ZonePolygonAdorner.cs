using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Designer;
using DevicesModule.Plans.ViewModels;
using DevicesModule.Plans.Designer;
using PlansModule.InstrumentAdorners;

namespace DevicesModule.Plans.InstrumentAdorners
{
	public class ZonePolygonAdorner : BasePolygonAdorner
	{
		public ZonePolygonAdorner(CommonDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
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
			var propertiesViewModel = new ZonePropertiesViewModel(element);
			DialogService.ShowModalWindow(propertiesViewModel);
			Helper.SetZone(element);
			return element;
		}
	}
}