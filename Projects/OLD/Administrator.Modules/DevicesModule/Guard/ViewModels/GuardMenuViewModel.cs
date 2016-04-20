using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
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