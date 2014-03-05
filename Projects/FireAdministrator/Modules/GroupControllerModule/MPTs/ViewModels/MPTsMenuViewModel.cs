using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class MPTsMenuViewModel : BaseViewModel
	{
		public MPTsMenuViewModel(MPTsViewModel context)
		{
			Context = context;
		}

		public MPTsViewModel Context { get; private set; }
	}
}