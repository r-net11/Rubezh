using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI.Models;
using GKModule.Plans.Designer;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using PlansModule.InstrumentAdorners;
using GKModule.Plans.ViewModels;

namespace GKModule.Plans.InstrumentAdorners
{
	public class XZonePolygonAdorner : BasePolygonAdorner
	{
		public XZonePolygonAdorner(CommonDesignerCanvas designerCanvas)
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
			var element = new ElementPolygonXZone();
			var propertiesViewModel = new XZonePropertiesViewModel(element);
			DialogService.ShowModalWindow(propertiesViewModel);
			Helper.SetXZone(element);
			return element;
		}
	}
}