using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

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