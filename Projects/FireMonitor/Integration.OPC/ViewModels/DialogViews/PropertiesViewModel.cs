using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Integration.OPC.Models;
using StrazhAPI.Enums;

namespace Integration.OPC.ViewModels
{
	public class PropertiesViewModel : DialogViewModel
	{
		public OPCZone CurrentZone { get; set; }
		public bool IsFireZone { get { return CurrentZone.Type == OPCZoneType.Fire; } }

		public PropertiesViewModel(OPCZone zone)
		{
			if (zone == null)
			{
				MessageBoxService.ShowError("Выберите зону.");
				return;
			}

			Title = zone.Name;
			CurrentZone = zone;
			GuardOffCommand = new RelayCommand(OnGuardOff);
			GuardOnCommand = new RelayCommand(OnGuardOn);
		}

		public RelayCommand GuardOffCommand { get; set; }
		public RelayCommand GuardOnCommand { get; set; }

		public void OnGuardOff()
		{
			FiresecManager.FiresecService.UnsetGuard(CurrentZone.No);
		}

		public void OnGuardOn()
		{
			FiresecManager.FiresecService.SetGuard(CurrentZone.No);
		}
	}
}
