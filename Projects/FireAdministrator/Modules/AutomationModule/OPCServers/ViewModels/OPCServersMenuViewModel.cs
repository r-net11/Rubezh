using Infrastructure.Common.Windows.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class OPCServersMenuViewModel : BaseViewModel
	{
		public OPCServersMenuViewModel(OPCServersViewModel context)
		{
			Context = context;
		}

		public OPCServersViewModel Context { get; private set; }
	}
}