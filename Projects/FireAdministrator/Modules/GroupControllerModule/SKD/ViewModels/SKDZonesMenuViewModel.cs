using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class SKDZonesMenuViewModel : BaseViewModel
	{
		public SKDZonesMenuViewModel(SKDZonesViewModel zonesViewModel)
		{
			Context = zonesViewModel;
		}

		public SKDZonesViewModel Context { get; private set; }
	}
}