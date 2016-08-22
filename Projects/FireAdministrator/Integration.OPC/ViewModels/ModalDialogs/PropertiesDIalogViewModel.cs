using Infrastructure.Common.Windows.ViewModels;
using Integration.OPC.Models;
using Localization.IntegrationOPC.ViewModels;
using StrazhAPI.Enums;

namespace Integration.OPC.ViewModels
{
	public class PropertiesDialogViewModel : DialogViewModel
	{
		public OPCZone CurrentZone { get; private set; }

		public bool IsGuardSectionVisible { get { return CurrentZone.Type == OPCZoneType.Guard; } }

		public PropertiesDialogViewModel(OPCZone currentZone)
		{
			Title = string.Format(CommonViewModels.ZoneProperties, currentZone.Name);
			CurrentZone = currentZone;
		}
	}
}
