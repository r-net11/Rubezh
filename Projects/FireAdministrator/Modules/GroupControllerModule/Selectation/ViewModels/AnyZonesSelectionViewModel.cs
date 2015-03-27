using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class AnyZonesSelectionViewModel : SaveCancelDialogViewModel
	{
		public ZonesSelectationViewModel ZonesSelectationViewModel { get; private set; }
		public GuardZonesWithFuncSelectationViewModel GuardZonesWithFuncSelectationViewModel { get; private set; }
		GKDevice Device { get; set; }

		public AnyZonesSelectionViewModel(GKDevice device)
		{
			Title = "Выбор зон";
			Device = device;
			ZonesSelectationViewModel = new ZonesSelectationViewModel(device.Zones, true);
			GuardZonesWithFuncSelectationViewModel = new GuardZonesWithFuncSelectationViewModel(device, true);
		}

		protected override bool Save()
		{
			GKManager.ChangeDeviceZones(Device, ZonesSelectationViewModel.TargetZones.ToList());
			GKManager.ChangeDeviceGuardZones(Device, GuardZonesWithFuncSelectationViewModel.TargetZones.Select(x => x.DeviceGuardZone).ToList());
			return base.Save();
		}
	}
}
