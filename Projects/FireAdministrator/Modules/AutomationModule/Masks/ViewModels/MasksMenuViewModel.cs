using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class MasksMenuViewModel : BaseViewModel
	{
		public MasksMenuViewModel(MasksViewModel context)
		{
			Context = context;
		}

		public MasksViewModel Context { get; private set; }
	}
}

