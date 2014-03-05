using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class FilterStateClassViewModel : BaseViewModel
	{
		public FilterStateClassViewModel(XStateClass stateClass)
		{
			StateClass = stateClass;
		}

		public XStateClass StateClass { get; private set; }
		public bool IsChecked { get; set; }
	}
}