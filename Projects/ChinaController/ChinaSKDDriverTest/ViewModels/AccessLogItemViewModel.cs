using ChinaSKDDriverAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class AccessLogItemViewModel : BaseViewModel
	{
		public AccessLogItem AccessLogItem { get; private set; }

		public AccessLogItemViewModel(AccessLogItem accessLogItem)
		{
			AccessLogItem = accessLogItem;
		}
	}
}