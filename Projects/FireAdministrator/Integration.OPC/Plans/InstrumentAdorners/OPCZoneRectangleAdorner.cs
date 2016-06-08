using Integration.OPC.ViewModels;
using StrazhAPI.Integration.OPC;
using StrazhAPI.Models;
using StrazhAPI.Plans.Elements;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;

namespace Integration.OPC.Plans.InstrumentAdorners
{
	public class OPCZoneRectangleAdorner : BaseRectangleAdorner
	{
		private readonly ZonesOPCViewModel _zonesViewModel;
		private readonly OPCPlanExtension _extension;
		public OPCZoneRectangleAdorner(CommonDesignerCanvas designerCanvas, ZonesOPCViewModel zonesViewModel, OPCPlanExtension extension)
			: base(designerCanvas)
		{
			_zonesViewModel = zonesViewModel;
			_extension = extension;
		}

		protected override ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleOPCZone();
			var propertiesViewModel = new ZonePropertiesViewModel(element, _zonesViewModel.ZonesOPC, _extension);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;

			_extension.SetItem<OPCZone>(element);
			return element;
		}
	}
}