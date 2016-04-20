using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	class GuardMenuViewModel : BaseViewModel
	{
		public GuardMenuViewModel(CodesViewModel context)
		{
			Context = context;
		}

		public CodesViewModel Context { get; private set; }
	}
}