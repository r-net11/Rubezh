using Infrastructure.Common.Windows.ViewModels;
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