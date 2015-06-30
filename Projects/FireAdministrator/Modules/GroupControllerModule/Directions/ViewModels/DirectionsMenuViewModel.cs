using Infrastructure.Common.Windows.ViewModels;
namespace GKModule.ViewModels
{
	public class DirectionsMenuViewModel : BaseViewModel
	{
		public DirectionsMenuViewModel(DirectionsViewModel context)
		{
			Context = context;
		}

		public DirectionsViewModel Context { get; private set; }
	}
}