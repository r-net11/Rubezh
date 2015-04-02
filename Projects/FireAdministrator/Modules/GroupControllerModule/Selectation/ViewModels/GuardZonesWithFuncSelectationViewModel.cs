using FiresecAPI.GK;

namespace GKModule.ViewModels
{
	public class GuardZonesWithFuncSelectationViewModel : GuardZonesSelectationViewModel
	{
		public GuardZonesWithFuncSelectationViewModel(GKDevice device, bool canCreateNew = false) : base(device.GuardZones, canCreateNew, device)
		{
			
		}
	}
}
