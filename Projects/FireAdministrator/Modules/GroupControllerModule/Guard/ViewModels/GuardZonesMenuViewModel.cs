using Infrastructure.Common.Windows.Windows.ViewModels;
namespace GKModule.ViewModels
{
	public class GuardZonesMenuViewModel : BaseViewModel
	{
		public GuardZonesMenuViewModel(GuardZonesViewModel context)
		{
			Context = context;
		}

		public GuardZonesViewModel Context { get; private set; }
	}
}