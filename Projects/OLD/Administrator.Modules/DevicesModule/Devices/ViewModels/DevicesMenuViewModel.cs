using Infrastructure.Common.Windows.Windows.ViewModels;
namespace DevicesModule.ViewModels
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