using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class SoundsMenuViewModel : BaseViewModel
	{
		public SoundsMenuViewModel(SoundsViewModel context)
		{
			Context = context;
		}

		public SoundsViewModel Context { get; private set; }
	}
}