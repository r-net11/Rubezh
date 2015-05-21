using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;

namespace PowerCalculator.ViewModels
{
	public class DriverTypeViewModel : BaseViewModel
	{
		public DriverTypeViewModel(DriverType deviceType)
		{
			DriverType = deviceType;
		}

		public DriverType DriverType { get; private set; }
	}
}