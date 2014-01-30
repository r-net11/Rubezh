using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ZonesMenuViewModel : BaseViewModel
	{
		public ZonesMenuViewModel(ZonesViewModel zonesViewModel)
		{
			Context = zonesViewModel;
		}

		public ZonesViewModel Context { get; private set; }
	}
}