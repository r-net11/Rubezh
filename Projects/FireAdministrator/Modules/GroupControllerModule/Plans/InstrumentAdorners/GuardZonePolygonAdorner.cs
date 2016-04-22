using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.InstrumentAdorners;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GKModule.Plans.InstrumentAdorners
{
	public class GuardZonePolygonAdorner : BasePolygonAdorner
	{
		GuardZonesViewModel _guardZonesViewModel;

		public GuardZonePolygonAdorner(CommonDesignerCanvas designerCanvas, GuardZonesViewModel guardZonesViewModel)
			: base(designerCanvas)
		{
			_guardZonesViewModel = guardZonesViewModel;
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
			var element = new ElementPolygonGKGuardZone();
			var propertiesViewModel = new GuardZonePropertiesViewModel(element);
			if (DialogService.ShowModalWindow(propertiesViewModel))
			{
				_guardZonesViewModel.UpdateZones(element.ZoneUID);
				return element;
			}
			return null;
		}
	}
}