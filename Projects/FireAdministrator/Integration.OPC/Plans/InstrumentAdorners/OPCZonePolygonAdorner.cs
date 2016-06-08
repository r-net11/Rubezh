using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;
using Integration.OPC.ViewModels;
using StrazhAPI.Integration.OPC;
using StrazhAPI.Models;
using StrazhAPI.Plans.Elements;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Integration.OPC.Plans.InstrumentAdorners
{
	public class OPCZonePolygonAdorner : BasePolygonAdorner
	{
		private readonly ZonesOPCViewModel _zonesViewModel;
		private readonly OPCPlanExtension _extension;

		public OPCZonePolygonAdorner(CommonDesignerCanvas designerCanvas, ZonesOPCViewModel zonesViewModel, OPCPlanExtension extension)
			: base(designerCanvas)
		{
			_zonesViewModel = zonesViewModel;
			_extension = extension;
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
			var element = new ElementPolygonOPCZone();
			var propertiesViewModel = new ZonePropertiesViewModel(element, _zonesViewModel.ZonesOPC, _extension);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;

			_extension.SetItem<OPCZone>(element);
			return element;
		}
	}
}