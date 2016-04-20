using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class DeviceParametersMenuViewModel : BaseViewModel
	{
		public DeviceParametersMenuViewModel(DeviceParametersViewModel context)
		{
			Context = context;
		}

		public DeviceParametersViewModel Context { get; private set; }
	}
}