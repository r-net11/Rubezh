using Infrastructure.Common.Windows.ViewModels;

namespace SkudModule.ViewModels
{
	public class DevicesMenuViewModel : BaseViewModel
	{
		public DevicesMenuViewModel(DevicesViewModel devicesViewModel)
		{
			Context = devicesViewModel;
		}

		public DevicesViewModel Context { get; private set; }
	}
}