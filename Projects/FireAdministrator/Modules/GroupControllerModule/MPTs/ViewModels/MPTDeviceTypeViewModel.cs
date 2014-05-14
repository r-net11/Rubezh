using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class MPTDeviceTypeViewModel : BaseViewModel
	{
		public MPTDeviceTypeViewModel(MPTDeviceType mptDeviceType)
		{
			MPTDeviceType = mptDeviceType;
		}

		public MPTDeviceType MPTDeviceType { get; private set; }
	}
}