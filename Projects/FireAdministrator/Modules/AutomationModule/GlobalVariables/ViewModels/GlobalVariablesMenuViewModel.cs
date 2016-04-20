using RubezhAPI.Automation;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class GlobalVariablesMenuViewModel : BaseViewModel
	{
		public GlobalVariablesMenuViewModel(GlobalVariablesViewModel context)
		{
			Context = context;
		}

		public GlobalVariablesViewModel Context { get; private set; }
	}
}