using GKModule.Plans.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GKModule.Plans.InstrumentAdorners
{
	public class SKDZonePolygonAdorner : BasePolygonAdorner
	{
		public SKDZonePolygonAdorner(CommonDesignerCanvas designerCanvas)
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
			var element = new ElementPolygonGKSKDZone();
			var propertiesViewModel = new SKDZonePropertiesViewModel(element);
			if (DialogService.ShowModalWindow(propertiesViewModel))

				return element;
			return null;
		}
	}
}