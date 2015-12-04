using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class OpcAdvosolMenuViewModel: BaseViewModel
	{
		public OpcAdvosolMenuViewModel(OpcAdvosolViewModel contex)
		{
			Context = contex;
		}

		public OpcAdvosolViewModel Context { get; private set; }
	}
}