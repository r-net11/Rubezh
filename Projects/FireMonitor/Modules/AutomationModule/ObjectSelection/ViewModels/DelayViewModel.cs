using RubezhAPI.GK;
using RubezhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class DelayViewModel : BaseViewModel
	{
		public GKDelay Delay { get; private set; }

		public DelayViewModel(GKDelay delay)
		{
			Delay = delay;
		}
	}
}