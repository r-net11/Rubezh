
using Infrastructure.Common.Windows.ViewModels;
namespace DevicesModule.ViewModels
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