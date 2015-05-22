using ChinaSKDDriverAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class AccessViewModel : BaseViewModel
	{
		public Access Access { get; private set; }

		public AccessViewModel(Access access)
		{
			Access = access;
		}
	}
}