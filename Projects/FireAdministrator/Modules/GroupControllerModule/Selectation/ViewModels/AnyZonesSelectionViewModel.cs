using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;

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

		public bool IsSelectedZone { get { return Device.Zones.Count > 0; } }
		public bool IsSelectedGuardZone { get { return Device.GuardZones.Count > 0; } }

		protected override bool Save()
		{
			GKManager.ChangeDeviceZones(Device, ZonesSelectationViewModel.TargetZones.ToList());
			GKManager.ChangeDeviceGuardZones(Device, GuardZonesWithFuncSelectationViewModel.TargetZones.Select(x => x.DeviceGuardZone).ToList());
			return base.Save();
		}
	}
}
