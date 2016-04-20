using RubezhAPI.GK;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class MPTDeviceViewModel : BaseViewModel
	{
		public DeviceViewModel DeviceViewModel { get; private set; }
		public GKMPTDeviceType MPTDeviceType { get; private set; }

		public MPTDeviceViewModel(DeviceViewModel deviceViewModel, GKMPTDeviceType mptDeviceType)
		{
			DeviceViewModel = deviceViewModel;
			MPTDeviceType = mptDeviceType;
		}
	}
}
