using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class StateViewModel : BaseViewModel
	{
		public DriverState DriverState { get; set; }
	}
}