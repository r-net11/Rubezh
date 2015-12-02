using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class OpcDaServersMenuViewModel: BaseViewModel
	{
		public OpcDaServersMenuViewModel(OpcDaServersViewModel contex)
		{
			Context = contex;
		}

		public OpcDaServersViewModel Context { get; private set; }
	}
}