using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class SimulationMenuViewModel : BaseViewModel
	{
		public SimulationMenuViewModel(SimulationViewModel context)
		{
			Context = context;
		}

		public SimulationViewModel Context { get; private set; }
	}
}