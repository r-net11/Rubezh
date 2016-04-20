using Infrastructure.Common.Windows.Windows.ViewModels;
namespace DevicesModule.ViewModels
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