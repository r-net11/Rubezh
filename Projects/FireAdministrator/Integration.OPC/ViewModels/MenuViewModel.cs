using Infrastructure.Common.Windows.ViewModels;

namespace Integration.OPC.ViewModels
{
	public class MenuViewModel : BaseViewModel
	{
		public MenuViewModel(ZonesOPCViewModel context)
		{
			Context = context;
		}

		public ZonesOPCViewModel Context { get; private set; }
	}
}
