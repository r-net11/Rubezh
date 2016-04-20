using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class OpcDaClientMenuViewModel : BaseViewModel
	{
		public OpcDaClientMenuViewModel(OpcDaClientViewModel contex)
		{
			Context = contex;
		}

		public OpcDaClientViewModel Context { get; private set; }
	}
}
