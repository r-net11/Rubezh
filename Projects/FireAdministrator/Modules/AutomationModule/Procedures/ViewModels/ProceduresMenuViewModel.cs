using Infrastructure.Common.Windows.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ProceduresMenuViewModel : BaseViewModel
	{
		public ProceduresMenuViewModel(ProceduresViewModel context)
		{
			Context = context;
		}

		public ProceduresViewModel Context { get; private set; }
	}
}