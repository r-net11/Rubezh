using AutomationModule.ViewModels;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class VariablesMenuViewModel : BaseViewModel
	{
		public VariablesMenuViewModel(VariablesViewModel context)
		{
			Context = context;
		}

		public VariablesViewModel Context { get; private set; }
	}
}