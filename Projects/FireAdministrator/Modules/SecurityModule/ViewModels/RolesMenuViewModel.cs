using Infrastructure.Common.Windows.Windows.ViewModels;

namespace SecurityModule.ViewModels
{
	public class RolesMenuViewModel : BaseViewModel
	{
		public RolesMenuViewModel(RolesViewModel context)
		{
			Context = context;
		}

		public RolesViewModel Context { get; private set; }
	}
}