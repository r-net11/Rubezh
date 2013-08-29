using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
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