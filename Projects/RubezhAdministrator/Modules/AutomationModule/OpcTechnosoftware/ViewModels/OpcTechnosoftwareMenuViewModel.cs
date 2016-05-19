using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class OpcTechnosoftwareMenuViewModel : BaseViewModel
	{
		public OpcTechnosoftwareMenuViewModel(OpcTechnosoftwareViewModel contex)
		{
			Context = contex;
		}

		public OpcTechnosoftwareViewModel Context { get; private set; }
	}
}
