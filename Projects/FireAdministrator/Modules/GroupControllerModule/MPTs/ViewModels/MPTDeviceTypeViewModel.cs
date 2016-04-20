using RubezhAPI.GK;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class MPTDeviceTypeViewModel : BaseViewModel
	{
		public MPTDeviceTypeViewModel(GKMPTDeviceType mptDeviceType)
		{
			MPTDeviceType = mptDeviceType;
		}

		public GKMPTDeviceType MPTDeviceType { get; private set; }
	}
}