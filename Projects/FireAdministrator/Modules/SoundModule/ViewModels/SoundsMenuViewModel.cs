using Infrastructure.Common.Windows.Windows.ViewModels;

namespace SoundsModule.ViewModels
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