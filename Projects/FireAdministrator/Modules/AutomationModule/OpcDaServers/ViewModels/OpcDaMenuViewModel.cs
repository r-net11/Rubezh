using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class OpcDaMenuViewModel: BaseViewModel
	{
		public OpcDaMenuViewModel(OpcDaServersViewModel contex)
		{
			Context = contex;
		}

		public OpcDaServersViewModel Context { get; private set; }
	}
}