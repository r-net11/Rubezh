using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	class GuardMenuViewModel : BaseViewModel
	{
		public GuardMenuViewModel(GuardViewModel context)
		{
			Context = context;
		}

		public GuardViewModel Context { get; private set; }
	}
}