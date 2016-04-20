using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class PumpStationsMenuViewModel : BaseViewModel
	{
		public PumpStationsMenuViewModel(PumpStationsViewModel context)
		{
			Context = context;
		}

		public PumpStationsViewModel Context { get; private set; }
	}
}