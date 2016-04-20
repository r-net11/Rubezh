using Infrastructure.Common.Windows.ViewModels;
using PowerCalculator.Models;

namespace PowerCalculator.ViewModels
{
	public class DriverViewModel : BaseViewModel
	{
		public DriverViewModel(Driver driver)
		{
			Driver = driver;
		}

		public Driver Driver { get; private set; }
	}
}