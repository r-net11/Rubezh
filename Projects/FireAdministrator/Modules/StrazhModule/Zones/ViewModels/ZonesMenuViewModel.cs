using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
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