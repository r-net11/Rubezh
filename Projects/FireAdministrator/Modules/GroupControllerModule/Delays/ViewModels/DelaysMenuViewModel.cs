using Infrastructure.Common.Windows.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DelaysMenuViewModel : BaseViewModel
	{
		public DelaysMenuViewModel(DelaysViewModel context)
		{
			Context = context;
		}

		public DelaysViewModel Context { get; private set; }
	}
}