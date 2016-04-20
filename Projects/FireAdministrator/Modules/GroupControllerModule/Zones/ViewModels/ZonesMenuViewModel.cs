using Infrastructure.Common.Windows.ViewModels;
namespace GKModule.ViewModels
{
	public class ZonesMenuViewModel : BaseViewModel
	{
		public ZonesMenuViewModel(ZonesViewModel context)
		{
			Context = context;
		}

		public ZonesViewModel Context { get; private set; }
	}
}